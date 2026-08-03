using Microsoft.AspNetCore.SignalR;
using QueueServer.Hubs;
using QueueServer.Models;

namespace QueueServer.Services;

public class TicketHandoffEngine : BackgroundService
{
    private readonly QueueService _schedulerService;
    public List<WindowWorkerService> WindowWorkerServices {get; }
    private readonly ILogger<TicketHandoffEngine> _handoffEngineLogger;
    public readonly ILogger<WindowWorkerService> _windowWorkerLogger;

    public TicketHandoffEngine(
        QueueService schedulerService, 
        ILoggerFactory handoffEngineLogger, 
        ILoggerFactory windowWorkerLogger,
        IHubContext<TicketStatusHub, ITicketStatusClient> hubContext)
    {
        _schedulerService = schedulerService;
        _handoffEngineLogger = handoffEngineLogger.CreateLogger<TicketHandoffEngine>();
        _windowWorkerLogger = windowWorkerLogger.CreateLogger<WindowWorkerService>();

        WindowWorkerServices = [
            new WindowWorkerService(WindowWorker.Create(windowNumber: 1), _windowWorkerLogger, hubContext, _schedulerService),
            new WindowWorkerService(WindowWorker.Create(windowNumber: 2), _windowWorkerLogger, hubContext, _schedulerService),
            new WindowWorkerService(WindowWorker.Create(windowNumber: 3), _windowWorkerLogger, hubContext, _schedulerService)
        ];
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _handoffEngineLogger.LogInformation("Handoff engine started. Monitoring queue...");

        while(!stoppingToken.IsCancellationRequested)
        {
            if(_schedulerService.Queue.IsEmpty) { 
                await Task.Delay(500, stoppingToken); 
                continue;
            }

            var freeWindowWorkerService = WindowWorkerServices
                .FirstOrDefault(ww => ww.WindowWorker.IsAvailable && ww.WindowWorker.CurrentTicket == null);
            
            if(freeWindowWorkerService == null) { 
                await Task.Delay(200, stoppingToken); 
                continue;
            }

            var freeWindowWorker = freeWindowWorkerService.WindowWorker;

            if(_schedulerService.Queue.TryDequeue(out var ticket))
            {
                _ = freeWindowWorkerService.AcceptAndProcessTicketAsync(ticket);
                await _schedulerService.SendQueueStatus();

                _handoffEngineLogger.LogInformation($"\nTicket {ticket.Id} handed off to {freeWindowWorker.WindowNumber}\n");
            }
        }
    }
}