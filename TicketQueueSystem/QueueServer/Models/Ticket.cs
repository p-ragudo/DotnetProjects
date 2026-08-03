namespace QueueServer.Models;

public class Ticket
{
    public required string Id { get; set;}
    public string? UserId { get; set; }
    public int MillisToComplete { get; set; } = 3000;
    public int? WindowNumber { get; set; } = null;

    public string Status { get; set; } = TicketStatus.InQueue.ToString();
}

public enum TicketStatus
{
    InQueue,
    Processing,
    Done,
    Denied
}