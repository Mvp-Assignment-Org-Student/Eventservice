using Azure.Core;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class EventService(IEventRepository eventRepository) : IEventService
{
    private readonly IEventRepository _eventRepository = eventRepository;

    // Hjälp av chatgpt för att skapa packages direkt vid skapande
    public async Task<EventResult> CreateEventAsync(CreateEventRequest request)
    {
        try
        {
            var eventEntity = new EventEntity
            {
                Image = request.Image,
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                EventDate = request.EventDate,
                Packages = new List<EventPackageEntity>() 
            };

            var defaultPackages = CreateDefaultPackagesForEvent(eventEntity);
            foreach (var package in defaultPackages)
            {
                eventEntity.Packages.Add(package);
            }

            var result = await _eventRepository.AddAsync(eventEntity);

            return result.Success
                ? new EventResult { Success = true }
                : new EventResult { Success = false, Error = result.Error };
        }
        catch (Exception ex)
        {
            return new EventResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }


    public async Task<EventResult<IEnumerable<Event>>> GetAllEventsAsync()
    {
        var result = await _eventRepository.GetAllAsync();
        var events = result.Result.Select(x => new Event
        {
            Id = x.Id,
            Image = x.Image,
            Title = x.Title,
            Description = x.Description,
            Location = x.Location,
            EventDate = x.EventDate
        });

        return new EventResult<IEnumerable<Event>> { Success = true, Result = events };
    }

    public async Task<EventResult<Event?>> GetEventAsync(string eventId)
    {
        var result = await _eventRepository.GetAsync(x => x.Id == eventId, include: e => e.Include(ev => ev.Packages).ThenInclude(ep => ep.Package));

        if (result.Success && result.Result != null)
        {
            var currentEvent = new Event
            {
                Id = result.Result.Id,
                Image = result.Result.Image,
                Title = result.Result.Title,
                Description = result.Result.Description,
                Location = result.Result.Location,
                EventDate = result.Result.EventDate,
                Packages = result.Result.Packages.Select(ep => new Package
                {
                    Id = ep.Package.Id,
                    Title = ep.Package.Title,
                    SeatingArrangement = ep.Package.SeatingArrangement,
                    Placement = ep.Package.Placement,
                    Price = ep.Package.Price,
                    Currency = ep.Package.Currency
                })
            };
            return new EventResult<Event?> { Success = true, Result = currentEvent };
        }
        return new EventResult<Event?> { Success = false, Error = "Event Not found" };
    }


    public async Task<EventResult> DeleteEventAsync(string id)
    {
        try
        {
            var eventEntity = new EventEntity { Id = id };
            var result = await _eventRepository.DeleteAsync(eventEntity);

            return new EventResult
            {
                Success = result.Success,
                Error = result.Error
            };
        }
        catch (Exception ex)
        {
            return new EventResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }



    private List<EventPackageEntity> CreateDefaultPackagesForEvent(EventEntity eventEntity)
    {
        var standardPackage = new PackageEntity
        {
            Title = "Standard",
            Price = 199,
            Currency = "SEK"
        };

        var vipPackage = new PackageEntity
        {
            Title = "VIP",
            Price = 499,
            Currency = "SEK"
        };

        return new List<EventPackageEntity>
    {
        new EventPackageEntity
        {
            Event = eventEntity,
            Package = standardPackage
        },
        new EventPackageEntity
        {
            Event = eventEntity,
            Package = vipPackage
        }
    };
    }


}
