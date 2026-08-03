using QueueServer.Models;

namespace QueueServer.Dtos;

public record InitialProgramStatusDto
{
    public IEnumerable<Ticket> Queue { get; set; } = [];
    public IEnumerable<Ticket> DoneTickets { get; set; } = [];
    public IEnumerable<WindowWorker> WindowWorkers { get; set; } = [];
    public int TotalWaiting { get; set; }
}