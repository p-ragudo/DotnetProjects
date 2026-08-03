namespace QueueServer.Models;

public class WindowWorker
{
    public int WindowNumber { get; set; }
    public bool IsAvailable { get; set; } = true;
    public Ticket? CurrentTicket { get; set; }

    public static WindowWorker Create(int windowNumber, bool isAvailable = true, Ticket? ticket = null)
    {
        return new()
        {
            WindowNumber = windowNumber,
            IsAvailable = isAvailable,
            CurrentTicket = ticket
        };
    }
}