using QueueServer.Models;

namespace QueueServer.Dtos;

public record QueueInfoDto
{
    public IEnumerable<Ticket> Queue { get; set; } = [];
    public int TotalWaiting { get; set; }
}

