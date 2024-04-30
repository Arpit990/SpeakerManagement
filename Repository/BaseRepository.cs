using Microsoft.EntityFrameworkCore;
using SpeakerManagement.DatabaseContext;
using System.Linq.Expressions;

namespace SpeakerManagement.Repository
{
    #region Interface
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        int SaveChanges();

        Task<int> SaveChangesAsync();

        Task<List<TEntity>> GetAll();

        Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate = null);

        Task<TEntity> Get(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetFirst();

        Task<bool> Add(TEntity entity);

        Task AddRange(IEnumerable<TEntity> entities);

        Task<bool> Update(TEntity entity);

        Task Remove(TEntity entity);

        Task<bool> Remove(Expression<Func<TEntity, bool>> predicate);

        Task<bool> Remove<T>(Expression<Func<T, bool>> predicate) where T : class;

        Task RemoveAll(Expression<Func<TEntity, bool>> predicate);
    }
    #endregion

    #region Class
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        #region Protected
        protected DataContext _context { get; private set; }
        protected DbSet<TEntity> _dataTable { get; private set; }
        #endregion

        #region Constructor
        public BaseRepository(DataContext context)
        {
            _context = context;
            _dataTable = _context.Set<TEntity>();
        }
        #endregion

        #region Public
        public async Task<List<TEntity>> GetAll() => await GetList();

        public async Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate = null)
        {
            var query = _dataTable.AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);
            return await query.ToListAsync();
        }

        public async Task<TEntity> Get(Expression<Func<TEntity, bool>> predicate) => await _dataTable.FirstOrDefaultAsync(predicate);

        public async Task<TEntity> GetFirst() => await _dataTable.FirstOrDefaultAsync();

        public async Task<bool> Add(TEntity entity)
        {
            await _dataTable.AddAsync(entity);
            return true;
        }

        public async Task AddRange(IEnumerable<TEntity> entities) => await _dataTable.AddRangeAsync(entities);

        public async Task<bool> Update(TEntity entity)
        {
            _dataTable.Update(entity);
            return await Task.FromResult(true);
        }

        public async Task Remove(TEntity entity)
        {
            _dataTable.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<bool> Remove(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var item in _dataTable.Where(predicate))
                _dataTable.Remove(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> Remove<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var dataSet = _context.Set<T>();

            foreach (var item in dataSet.Where(predicate))
                dataSet.Remove(item);
            return await Task.FromResult(true);
        }

        public async Task RemoveAll(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = await GetList(predicate);
            if (entities != null)
                _dataTable.RemoveRange(entities);
        }

        public int SaveChanges() => _context.SaveChanges();

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        #endregion
    }
    #endregion
}
