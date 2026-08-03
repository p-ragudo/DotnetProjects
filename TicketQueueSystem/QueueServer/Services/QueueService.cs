using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using QueueServer.Common;
using QueueServer.Dtos;
using QueueServer.Hubs;
using QueueServer.Models;

namespace QueueServer.Services;

public class QueueService
{
    public ConcurrentQueue<Ticket> Queue {get; } = new();
    public List<Ticket> DoneTickets { get; } = new();
    private int _ticketCounter = 0;
    private readonly ILogger<WindowWorkerService> _logger;
    private readonly IHubContext<TicketStatusHub, ITicketStatusClient> _hubContext;
    private QueueInfoDto queueInfoDto = new();
    public QueueService(ILoggerFactory logger, IHubContext<TicketStatusHub, ITicketStatusClient> hubContext)
    {
        _logger = logger.CreateLogger<WindowWorkerService>();
        _hubContext = hubContext;
    }

    private Ticket? CreateTicket(int secondsToComplete = 0)
    {
        int nextId = Interlocked.Increment(ref _ticketCounter);
    
        Ticket ticket = new() { 
            Id = $"T-{nextId:D3}" ,
            MillisToComplete = secondsToComplete == 0
                ? Utils.GenerateRandomMiliseconds(3000, 8000, 1000)
                : secondsToComplete
        };

        return ticket;
    }

    private void AddTicketToQueue(Ticket ticket)
    {
        Queue.Enqueue(ticket);
    } 

    public async Task<Ticket?> RequestTicket()
    {
        Ticket? ticket = CreateTicket();
        AddTicketToQueue(ticket);

        await SendQueueStatus();

        return ticket;
    }

    public Ticket? RequestTicket(int secondsToComplete)
    {
        Ticket? ticket = CreateTicket(secondsToComplete);
        AddTicketToQueue(ticket);

        return ticket;
    }

    public async Task SendQueueStatus()
    {
        queueInfoDto.Queue = Queue.ToArray();
        queueInfoDto.TotalWaiting = Queue.Count();

        await _hubContext.Clients.All.SendQueueStatus(queueInfoDto);
    }

    public void AddTicketToDoneList(Ticket ticket)
    {
        DoneTickets.Add(ticket);
    }
}