using Application.Common;
using Domain.Exceptions;
using Domain.Primitives;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Persistence.Extensions;


namespace Persistence
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : Entity<TKey> where TKey : notnull
    {
        private readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;


        public GenericRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T?> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task UpsertAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existing = await _dbSet.FindAsync(entity.Id);

            if (existing == null)
            {
                await _dbSet.AddAsync(entity);
            }
            else
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                _dbSet.Update(existing);
            }
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<PagedResult<T>> GetPagedAsync(QueryParameters<T> queryParameters)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            foreach (var filter in queryParameters.Filters)
            {
                query = FilterProcessor.ApplyFilter(query, filter);
            }

            if (queryParameters.SortBy.Any())
            {
                query = FilterProcessor.ApplySorting(queryParameters, query);
            }

            int totalRecords = await query.CountAsync();
            var items = await query
                .Skip((queryParameters.Page - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync();

            return new PagedResult<T>(items, totalRecords, queryParameters.Page, queryParameters.PageSize);
        }

        public async Task<ErrorOr<Unit>> SaveChangesAsync(bool entityEvents = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
            catch (DbUpdateException ex)
            {
                return Error.Unexpected(
                    code: "DbUpdateException",
                    description: "errors occure when update data: " + ex.Message
                );
            }
            catch (Exception ex)
            {
                return Error.Failure(
                    code: "UnhandledException",
                    description: "Unknown error: " + ex.Message
                );
            }


        }


    }
}
