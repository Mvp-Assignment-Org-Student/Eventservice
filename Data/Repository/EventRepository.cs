using Data.Contexts;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repository;

public class EventRepository(DataContext context) : BaseRepository<EventEntity>(context), IEventRepository
{
    public override async Task<RepositoryResult<IEnumerable<EventEntity>>> GetAllAsync()
    {
        try
        {
            var entities = await _table.Include(x => x.Packages).ToListAsync();

            return new RepositoryResult<IEnumerable<EventEntity>>
            {
                Success = true,
                Result = entities
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<IEnumerable<EventEntity>>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public  async override Task<RepositoryResult<EventEntity?>> GetAsync(Expression<Func<EventEntity, bool>> expression, Func<IQueryable<EventEntity>, IQueryable<EventEntity>>? include = null)
    {
        try
        {

            var entity = await _table
                .Include(x => x.Packages)
                    .ThenInclude(ep => ep.Package)
                .FirstOrDefaultAsync(expression);

            if (entity == null)
            {
                throw new Exception("Not Found.");
            }

            return new RepositoryResult<EventEntity?>
            {
                Success = true,
                Result = entity
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<EventEntity?>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }
    public override async Task<RepositoryResult> DeleteAsync(EventEntity entity)
    {
        try
        {
            // Hämta från databasen först!
            var existingEntity = await _context.Events.FindAsync(entity.Id);

            if (existingEntity == null)
            {
                return new RepositoryResult
                {
                    Success = false,
                    Error = "Event not found"
                };
            }

            _context.Events.Remove(existingEntity);
            await _context.SaveChangesAsync();

            return new RepositoryResult { Success = true };
        }
        catch (Exception ex)
        {
            return new RepositoryResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

}

public interface IEventRepository : IBaseRepository<EventEntity>
{
    
}