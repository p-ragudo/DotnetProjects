using Microsoft.AspNetCore.SignalR;
using QueueServer.Dtos;
using QueueServer.Hubs;
using QueueServer.Models;

namespace QueueServer.Services;

public class WindowWorkerService
{
    public WindowWorker WindowWorker;

    private readonly ILogger<WindowWorkerService> _logger;
    private readonly IHubContext<TicketStatusHub, ITicketStatusClient> _hubContext;
    private readonly QueueService _queueService;

    public WindowWorkerService(
        WindowWorker windowWorker, 
        ILogger<WindowWorkerService> logger, 
        IHubContext<TicketStatusHub, ITicketStatusClient> hubContext,
        QueueService queueService)
    {
        WindowWorker = windowWorker;
        _logger = logger;
        _hubContext = hubContext;
        _queueService = queueService;
    }

    public async Task AcceptAndProcessTicketAsync(Ticket ticket)
    {
        // update ticket and window worker status before sending processing status
        ticket.WindowNumber = WindowWorker.WindowNumber;
        ticket.Status = TicketStatus.Processing.ToString();

        WindowWorker.IsAvailable = false;
        WindowWorker.CurrentTicket = ticket;

        TransactionStatusDto transactionStatus = new()
        {
            Ticket = ticket,
            WindowWorker = WindowWorker
        };

        await _hubContext.Clients.All.SendTransactionStatus(transactionStatus);
        _logger.LogInformation($"\nTicket {ticket.Id} handed off to Window {WindowWorker.WindowNumber}\n");

        await Task.Delay(ticket.MillisToComplete);

        // update ticket and window worker status before sending complete status
        transactionStatus.Ticket.Status = TicketStatus.Done.ToString();
        transactionStatus.Ticket.WindowNumber = null;

        transactionStatus.WindowWorker.IsAvailable = WindowWorker.IsAvailable = true;
        transactionStatus.WindowWorker.CurrentTicket = WindowWorker.CurrentTicket = null;

        await _hubContext.Clients.All.SendTransactionStatus(transactionStatus);
        _logger.LogInformation($"\nWindow {WindowWorker.WindowNumber}: Done processing ticket {ticket.Id}\n");
        _queueService.AddTicketToDoneList(ticket);
    }
}