using LinqKit;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpeakerManagement.Data;
using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Helper;
using SpeakerManagement.Migrations;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;

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

        GridResult PredicateSearch(GridSearch gridSearch, IQueryable<TEntity> query);
    }
    #endregion

    #region Class
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        #region Protected
        protected DataContext _context { get; private set; }
        protected DbSet<TEntity> _dataTable { get; private set; }

        protected IHttpContextAccessor? _httpContextAccessor;
        #endregion

        #region Constructor
        public BaseRepository(DataContext context) : this(context, null)
        {
        }

        public BaseRepository(
            DataContext context,
            IHttpContextAccessor? httpContextAccessor
        )
        {
            _context = context;
            _dataTable = _context.Set<TEntity>();
            _httpContextAccessor = httpContextAccessor;
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

        public string? GetCurrentUserId()
        {
            return _httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public GridResult PredicateSearchExt<T>(GridSearch gridSearch, IQueryable<T> query)
        {
            Expression<Func<T, bool>> predicate = PredicateBuilder.New<T>(true);

            /*if (!string.IsNullOrWhiteSpace(gridSearch.searchColumn))
            {
                predicate = WhereBuilder(gridSearch, predicate, UserLocalTimeZoneOffSet);
            }*/

            var result = query.Where(predicate);

            if (!string.IsNullOrEmpty(gridSearch.order))
            {
                result = OrderBuilderExt(gridSearch, result);
            }

            if (!string.IsNullOrEmpty(gridSearch.search))
            {
                result = GlobalWhereBuilderExt(gridSearch, result);
            }

            int totalCount = result.Count();

            if (gridSearch.length > 0 && gridSearch.start >= 0)
            {
                result = result.Skip(gridSearch.start).Take(gridSearch.length);
            }

            var data = result.ToList();

            var gridResult = new GridResult()
            {
                draw = gridSearch.draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = data,
                query = result.ToQueryString()
            };

            return gridResult;
        }

        private static IQueryable<T> OrderBuilderExt<T>(GridSearch gridSearch, IQueryable<T> result)
        {
            //multiple columns allowed, 1 is default
            var columns = gridSearch.order.Split(',');
            string command = gridSearch.orderDir == "asc" ? "OrderBy" : "OrderByDescending";

            for (int i = 0; i < columns.Length; i++)
            {
                var columnName = columns[i];

                if (i == 1) // Set "Then" only after an IOrderedQueryable returned
                    command = gridSearch.orderDir == "asc" ? "ThenBy" : "ThenByDescending";

                result = CreateSingleLambdaQueryExt(columnName, command, result);
            }

            return result;
        }

        private static IQueryable<T> CreateSingleLambdaQueryExt<T>(string columnName, string command, IQueryable<T> result)
        {
            columnName = char.ToUpper(columnName[0]) + columnName.Substring(1);

            var parameter = Expression.Parameter(typeof(T), "p");
            var property = typeof(T).GetProperty(columnName);
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var propertySelector = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { typeof(T), property.PropertyType },
                result.Expression, Expression.Quote(propertySelector));

            return result.Provider.CreateQuery<T>(resultExpression);
        }

        private IQueryable<T> GlobalWhereBuilderExt<T>(GridSearch gridSearch, IQueryable<T> result)
        {
            Expression<Func<T, bool>> globalPredicate = PredicateBuilder.New<T>();
            var tableType = typeof(T);
            var properties = tableType.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                if (property.CustomAttributes.FirstOrDefault(p => p.AttributeType.Name == "NotMappedAttribute") == null)
                {
                    var columnInfo = typeof(T).GetProperty(property.Name);
                    if (columnInfo == null) throw new Exception("No such grid search property");

                    var objectType = typeof(T);
                    var expressionParameters = Expression.Parameter(objectType, @"x");
                    var expressionProperties = Expression.PropertyOrField(expressionParameters, property.Name);
                    var propertyExpression = Expression.Property(expressionParameters, columnInfo);

                    if (IsNullableType(columnInfo.PropertyType) && expressionProperties.Type.GetGenericArguments().Length > 0)
                    {
                        ConstantExpression expressionConstant;

                        switch (expressionProperties.Type.GetGenericArguments()[0].Name)
                        {
                            case nameof(Int32):
                                if (Utility.IsInt32(gridSearch.search))
                                {
                                    expressionConstant = Expression.Constant(Convert.ChangeType(gridSearch.search, expressionProperties.Type.GetGenericArguments()[0]));
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(Expression.Equal(expressionProperties, Expression.Convert(expressionConstant, expressionProperties.Type)), expressionParameters));
                                }
                                break;
                            case nameof(Int64):
                                if (Utility.IsInt64(gridSearch.search))
                                {
                                    expressionConstant = Expression.Constant(Convert.ChangeType(gridSearch.search, expressionProperties.Type.GetGenericArguments()[0]));
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(Expression.Equal(expressionProperties, Expression.Convert(expressionConstant, expressionProperties.Type)), expressionParameters));
                                }
                                break;
                            case nameof(Double):
                                if (Utility.IsDouble(gridSearch.search))
                                {
                                    expressionConstant = Expression.Constant(Convert.ChangeType(gridSearch.search, expressionProperties.Type.GetGenericArguments()[0]));
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(Expression.Equal(expressionProperties, Expression.Convert(expressionConstant, expressionProperties.Type)), expressionParameters));
                                }
                                break;
                            case nameof(Decimal):
                                if (Utility.IsDecimal(gridSearch.search))
                                {
                                    expressionConstant = Expression.Constant(Convert.ChangeType(gridSearch.search, expressionProperties.Type.GetGenericArguments()[0]));
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(Expression.Equal(expressionProperties, Expression.Convert(expressionConstant, expressionProperties.Type)), expressionParameters));
                                }
                                break;
                            case nameof(DateTime):
                                if (Utility.IsDate(gridSearch.search))
                                {
                                    expressionConstant = Expression.Constant(Convert.ChangeType(gridSearch.search, expressionProperties.Type.GetGenericArguments()[0]));
                                    expressionProperties = Expression.Property(expressionProperties, "Value");
                                    expressionProperties = Expression.Property(expressionProperties, "Date");
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(Expression.Equal(expressionProperties, Expression.Convert(expressionConstant, expressionProperties.Type)), expressionParameters));
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (property.PropertyType.Name)
                        {
                            case nameof(Int32):
                                if (Utility.IsInt32(gridSearch.search))
                                {
                                    var intValue = int.Parse(gridSearch.search);
                                    MethodInfo intMethodInfo = typeof(int).GetMethod("Equals", new[] { typeof(int) });
                                    var intExpressionValue = Expression.Constant(intValue, typeof(int));
                                    var equalsMethodExpression = Expression.Call(propertyExpression, intMethodInfo, intExpressionValue);
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(equalsMethodExpression, expressionParameters));
                                }
                                break;
                            case nameof(Int64):
                                if (Utility.IsInt64(gridSearch.search))
                                {
                                    var intValue = long.Parse(gridSearch.search);
                                    MethodInfo intMethodInfo = typeof(long).GetMethod("Equals", new[] { typeof(long) });
                                    var intExpressionValue = Expression.Constant(intValue, typeof(long));
                                    var equalsMethodExpression = Expression.Call(propertyExpression, intMethodInfo, intExpressionValue);
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(equalsMethodExpression, expressionParameters));
                                }
                                break;
                            case nameof(Double):
                                if (Utility.IsNumeric(gridSearch.search))
                                {
                                    var doubleValue = double.Parse(gridSearch.search);
                                    MethodInfo doubleMethodInfo = typeof(double).GetMethod("Equals", new[] { typeof(double) });
                                    var doubleExpressionValue = Expression.Constant(doubleValue, typeof(double));
                                    var equalsMethodExpression = Expression.Call(propertyExpression, doubleMethodInfo, doubleExpressionValue);
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(equalsMethodExpression, expressionParameters));
                                }
                                break;
                            case nameof(Decimal):
                                if (Utility.IsDecimal(gridSearch.search))
                                {
                                    var doubleValue = decimal.Parse(gridSearch.search);
                                    MethodInfo doubleMethodInfo = typeof(decimal).GetMethod("Equals", new[] { typeof(decimal) });
                                    var doubleExpressionValue = Expression.Constant(doubleValue, typeof(decimal));
                                    var equalsMethodExpression = Expression.Call(propertyExpression, doubleMethodInfo, doubleExpressionValue);
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(equalsMethodExpression, expressionParameters));
                                }
                                break;
                            case nameof(DateTime):
                                if (Utility.IsDate(gridSearch.search))
                                {
                                    var dateTimeValue = DateTime.Parse(gridSearch.search);
                                    MethodInfo dateTimeMethodInfo = typeof(DateTime).GetMethod("Equals", new[] { typeof(DateTime) });
                                    var dateTimeExpressionValue = Expression.Constant(dateTimeValue, typeof(DateTime));
                                    var equalsMethodExpression = Expression.Call(propertyExpression, dateTimeMethodInfo, dateTimeExpressionValue);
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(equalsMethodExpression, expressionParameters));
                                }
                                break;
                            case nameof(String):
                                MethodInfo stringMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                var stringConstant = Expression.Constant(gridSearch.search.Trim(), typeof(string));
                                var containsMethodExpression = Expression.Call(expressionProperties, stringMethod, stringConstant);
                                globalPredicate = globalPredicate.Or(Expression.Lambda<Func<T, bool>>(containsMethodExpression, expressionParameters));
                                break;
                        }
                    }
                }
            }
            return result.Where(globalPredicate);
        }

        public GridResult PredicateSearch(GridSearch gridSearch, IQueryable<TEntity> query)
        {
            Expression<Func<TEntity, bool>> predicate = PredicateBuilder.New<TEntity>(true);

            /*if (!string.IsNullOrWhiteSpace(gridSearch.searchColumn))
            {
                predicate = WhereBuilder(gridSearch, predicate, UserLocalTimeZoneOffSet);
            }*/

            var result = query.Where(predicate);

            if (!string.IsNullOrEmpty(gridSearch.order))
            {
                result = OrderBuilder(gridSearch, result);
            }

            if (!string.IsNullOrEmpty(gridSearch.search))
            {
                result = GlobalWhereBuilder(gridSearch, result);
            }

            int totalCount = result.Count();

            if (gridSearch.length > 0 && gridSearch.start >= 0)
            {
                result = result.Skip(gridSearch.start).Take(gridSearch.length);
            }

            var data = result.ToList();

            var gridResult = new GridResult()
            {
                draw = gridSearch.draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = data,
                query = result.ToQueryString()
            };

            return gridResult;
        }

        private static IQueryable<TEntity> OrderBuilder(GridSearch gridSearch, IQueryable<TEntity> result)
        {
            //multiple columns allowed, 1 is default
            var columns = gridSearch.order.Split(',');
            string command = gridSearch.orderDir == "asc" ? "OrderBy" : "OrderByDescending";

            for (int i = 0; i < columns.Length; i++)
            {
                var columnName = columns[i];

                if (i == 1) // Set "Then" only after an IOrderedQueryable returned
                    command = gridSearch.orderDir == "asc" ? "ThenBy" : "ThenByDescending";

                result = CreateSingleLambdaQuery(columnName, command, result);
            }

            return result;
        }

        private static IQueryable<TEntity> CreateSingleLambdaQuery<TEntity>(string columnName, string command, IQueryable<TEntity> result)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "p");
            var property = typeof(TEntity).GetProperty(columnName);
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var propertySelector = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { typeof(TEntity), property.PropertyType },
                result.Expression, Expression.Quote(propertySelector));

            return result.Provider.CreateQuery<TEntity>(resultExpression);
        }

        private IQueryable<TEntity> GlobalWhereBuilder(GridSearch gridSearch, IQueryable<TEntity> result)
        {
            Expression<Func<TEntity, bool>> globalPredicate = PredicateBuilder.New<TEntity>();
            var tableType = typeof(TEntity);
            var properties = tableType.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                if (property.CustomAttributes.FirstOrDefault(p => p.AttributeType.Name == "NotMappedAttribute") == null)
                {
                    var columnInfo = typeof(TEntity).GetProperty(property.Name);
                    if (columnInfo == null) throw new Exception("No such grid search property");

                    var objectType = typeof(TEntity);
                    var expressionParameters = Expression.Parameter(objectType, @"x");
                    var expressionProperties = Expression.PropertyOrField(expressionParameters, property.Name);
                    var propertyExpression = Expression.Property(expressionParameters, columnInfo);

                    if (IsNullableType(columnInfo.PropertyType) && expressionProperties.Type.GetGenericArguments().Length > 0)
                    {
                        ConstantExpression expressionConstant;

                        switch (expressionProperties.Type.GetGenericArguments()[0].Name)
                        {
                            case nameof(Int32):
                                if (Utility.IsInt32(gridSearch.search))
                                {
                                    expressionConstant = Expression.Constant(Convert.ChangeType(gridSearch.search, expressionProperties.Type.GetGenericArguments()[0]));
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(expressionProperties, Expression.Convert(expressionConstant, expressionProperties.Type)), expressionParameters));
                                }
                                break;
                            case nameof(Int64):
                                if (Utility.IsInt64(gridSearch.search))
                                {
                                    expressionConstant = Expression.Constant(Convert.ChangeType(gridSearch.search, expressionProperties.Type.GetGenericArguments()[0]));
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(expressionProperties, Expression.Convert(expressionConstant, expressionProperties.Type)), expressionParameters));
                                }
                                break;
                            case nameof(Double):
                                if (Utility.IsDouble(gridSearch.search))
                                {
                                    expressionConstant = Expression.Constant(Convert.ChangeType(gridSearch.search, expressionProperties.Type.GetGenericArguments()[0]));
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(expressionProperties, Expression.Convert(expressionConstant, expressionProperties.Type)), expressionParameters));
                                }
                                break;
                            case nameof(Decimal):
                                if (Utility.IsDecimal(gridSearch.search))
                                {
                                    expressionConstant = Expression.Constant(Convert.ChangeType(gridSearch.search, expressionProperties.Type.GetGenericArguments()[0]));
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(expressionProperties, Expression.Convert(expressionConstant, expressionProperties.Type)), expressionParameters));
                                }
                                break;
                            case nameof(DateTime):
                                if (Utility.IsDate(gridSearch.search))
                                {
                                    expressionConstant = Expression.Constant(Convert.ChangeType(gridSearch.search, expressionProperties.Type.GetGenericArguments()[0]));
                                    expressionProperties = Expression.Property(expressionProperties, "Value");
                                    expressionProperties = Expression.Property(expressionProperties, "Date");
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(expressionProperties, Expression.Convert(expressionConstant, expressionProperties.Type)), expressionParameters));
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (property.PropertyType.Name)
                        {
                            case nameof(Int32):
                                if (Utility.IsInt32(gridSearch.search))
                                {
                                    var intValue = int.Parse(gridSearch.search);
                                    MethodInfo intMethodInfo = typeof(int).GetMethod("Equals", new[] { typeof(int) });
                                    var intExpressionValue = Expression.Constant(intValue, typeof(int));
                                    var equalsMethodExpression = Expression.Call(propertyExpression, intMethodInfo, intExpressionValue);
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(equalsMethodExpression, expressionParameters));
                                }
                                break;
                            case nameof(Int64):
                                if (Utility.IsInt64(gridSearch.search))
                                {
                                    var intValue = long.Parse(gridSearch.search);
                                    MethodInfo intMethodInfo = typeof(long).GetMethod("Equals", new[] { typeof(long) });
                                    var intExpressionValue = Expression.Constant(intValue, typeof(long));
                                    var equalsMethodExpression = Expression.Call(propertyExpression, intMethodInfo, intExpressionValue);
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(equalsMethodExpression, expressionParameters));
                                }
                                break;
                            case nameof(Double):
                                if (Utility.IsNumeric(gridSearch.search))
                                {
                                    var doubleValue = double.Parse(gridSearch.search);
                                    MethodInfo doubleMethodInfo = typeof(double).GetMethod("Equals", new[] { typeof(double) });
                                    var doubleExpressionValue = Expression.Constant(doubleValue, typeof(double));
                                    var equalsMethodExpression = Expression.Call(propertyExpression, doubleMethodInfo, doubleExpressionValue);
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(equalsMethodExpression, expressionParameters));
                                }
                                break;
                            case nameof(Decimal):
                                if (Utility.IsDecimal(gridSearch.search))
                                {
                                    var doubleValue = decimal.Parse(gridSearch.search);
                                    MethodInfo doubleMethodInfo = typeof(decimal).GetMethod("Equals", new[] { typeof(decimal) });
                                    var doubleExpressionValue = Expression.Constant(doubleValue, typeof(decimal));
                                    var equalsMethodExpression = Expression.Call(propertyExpression, doubleMethodInfo, doubleExpressionValue);
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(equalsMethodExpression, expressionParameters));
                                }
                                break;
                            case nameof(DateTime):
                                if (Utility.IsDate(gridSearch.search))
                                {
                                    var dateTimeValue = DateTime.Parse(gridSearch.search);
                                    MethodInfo dateTimeMethodInfo = typeof(DateTime).GetMethod("Equals", new[] { typeof(DateTime) });
                                    var dateTimeExpressionValue = Expression.Constant(dateTimeValue, typeof(DateTime));
                                    var equalsMethodExpression = Expression.Call(propertyExpression, dateTimeMethodInfo, dateTimeExpressionValue);
                                    globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(equalsMethodExpression, expressionParameters));
                                }
                                break;
                            case nameof(String):
                                MethodInfo stringMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                var stringConstant = Expression.Constant(gridSearch.search.Trim(), typeof(string));
                                var containsMethodExpression = Expression.Call(expressionProperties, stringMethod, stringConstant);
                                globalPredicate = globalPredicate.Or(Expression.Lambda<Func<TEntity, bool>>(containsMethodExpression, expressionParameters));
                                break;
                        }
                    }
                }
            }
            return result.Where(globalPredicate);
        }

        private static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        #endregion
    }
    #endregion
}
