using Business.Models;

namespace Business.Interfaces
{
    public interface IEventService
    {
        Task<EventResult> CreateEventAsync(CreateEventRequest request);
        Task<EventResult> DeleteEventAsync(string id);
        Task<EventResult<IEnumerable<Event>>> GetAllEventsAsync();
        Task<EventResult<Event?>> GetEventAsync(string eventId);
    }
}