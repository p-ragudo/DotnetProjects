using QueueServer.Models;

namespace QueueServer.Dtos;

public record TransactionStatusDto
{
    public Ticket? Ticket { get; set; }
    public required WindowWorker WindowWorker { get; set; }
}