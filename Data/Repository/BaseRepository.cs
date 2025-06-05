using Data.Contexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Data.Repository;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<RepositoryResult> AddAsync(TEntity entity);
    Task<RepositoryResult> AlreadyExists(Expression<Func<TEntity, bool>> expression);
    Task<RepositoryResult> DeleteAsync(TEntity entity);
    Task<RepositoryResult<IEnumerable<TEntity>>> GetAllAsync();
    Task<RepositoryResult<TEntity?>> GetAsync(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
    Task<RepositoryResult> UpdateAsync(TEntity entity);
}

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly DataContext _context;
    protected readonly DbSet<TEntity> _table;

    protected BaseRepository(DataContext context)
    {
        _context = context;
        _table = _context.Set<TEntity>();
    }

    public virtual async Task<RepositoryResult> AddAsync(TEntity entity)
    {
        try
        {
            _table.Add(entity);

            await _context.SaveChangesAsync();

            return new RepositoryResult
            {
                Success = true,

            };
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

    public virtual async Task<RepositoryResult<IEnumerable<TEntity>>> GetAllAsync()
    {
        try
        {
            var entities = await _table.ToListAsync();

            return new RepositoryResult<IEnumerable<TEntity>>
            {
                Success = true,
                Result = entities
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<IEnumerable<TEntity>>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public virtual async Task<RepositoryResult<TEntity?>> GetAsync(
     Expression<Func<TEntity, bool>> expression,
     Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
    {
        try
        {
            IQueryable<TEntity> query = _table;

            if (include != null)
            {
                query = include(query);
            }

            var entity = await query.FirstOrDefaultAsync(expression);

            if (entity == null)
                throw new Exception("Not Found.");

            return new RepositoryResult<TEntity?>
            {
                Success = true,
                Result = entity
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<TEntity?>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }


    public virtual async Task<RepositoryResult> AlreadyExists(Expression<Func<TEntity, bool>> expression)
    {
        var result = await _table.AnyAsync(expression);

        return result
        ? new RepositoryResult { Success = true }
        : new RepositoryResult { Success = false, Error = "Exists" };

    }
    public virtual async Task<RepositoryResult> UpdateAsync(TEntity entity)
    {
        try
        {
            _table.Update(entity);

            await _context.SaveChangesAsync();

            return new RepositoryResult
            {
                Success = true,

            };
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

    public virtual async Task<RepositoryResult> DeleteAsync(TEntity entity)
    {
        try
        {
            _table.Attach(entity); // Viktigt!
            _table.Remove(entity);

            await _context.SaveChangesAsync();

            return new RepositoryResult
            {
                Success = true,

            };
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
