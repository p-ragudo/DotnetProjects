using Microsoft.AspNetCore.SignalR;
using QueueServer.Dtos;
using QueueServer.Services;

namespace QueueServer.Hubs;

public interface ITicketStatusClient
{
    Task SendTransactionStatus(TransactionStatusDto dto);
    Task SendQueueStatus(QueueInfoDto dto);
    Task SendInitialProgramStatus(InitialProgramStatusDto dto);
}

public sealed class TicketStatusHub : Hub<ITicketStatusClient>
{
    private readonly QueueService _queueService;
    private readonly TicketHandoffEngine _ticketHandoffEngineService;

    public TicketStatusHub(QueueService queueService, TicketHandoffEngine ticketHandoffEngineService)
    {
        _queueService = queueService;
        _ticketHandoffEngineService = ticketHandoffEngineService;
    }
    public override async Task OnConnectedAsync()
    {
        var queue = _queueService.Queue.ToArray();
        var doneTickets = _queueService.DoneTickets.ToArray();
        var windowWorkers = _ticketHandoffEngineService.WindowWorkerServices
            .Select(service => service.WindowWorker)
            .ToList();
        var totalWaiting = queue.Length;

        InitialProgramStatusDto initialProgramStatusDto = new()
        {
          Queue = queue,
          DoneTickets = doneTickets,
          WindowWorkers = windowWorkers,
          TotalWaiting = totalWaiting  
        };

        await Clients.Caller.SendInitialProgramStatus(initialProgramStatusDto);
        await base.OnConnectedAsync();
    }
}