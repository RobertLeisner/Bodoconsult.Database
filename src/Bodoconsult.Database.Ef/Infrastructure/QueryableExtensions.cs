// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//namespace Bodoconsult EDV-Dienstleistungen GmbH.StSys.SQL.StSysDB.Infrastructure
//{
    ///// <summary>
    ///// Useful extension methods for use with Entity Framework LINQ queries.
    ///// </summary
    //public static class QueryableExtensions 
    //{
    //    /// <summary>
    //    /// Specifies the related objects to include in the query results.
    //    /// </summary>
    //    /// <typeparam name="T"> The type of entity being queried. </typeparam>
    //    /// <param name="source">
    //    /// The source <see cref="IQueryable" /> on which to call Include.
    //    /// </param>
    //    /// <param name="path"> The dot-separated list of related objects to return in the query results. </param>
    //    /// <returns>
    //    /// A new <see cref="IQueryable" /> with the defined query path.
    //    /// </returns>
    //    public static IQueryable<T> Include<T>(this IQueryable<T> source, string path) where T : class, IEntityRequirements, new()
    //    {
    //        return EntityFrameworkQueryableExtensions.Include(source, path);
    //    }



    //    /// <summary>
    //    /// Specifies the related objects to include in the query results.
    //    /// </summary>
    //    /// <typeparam name="T"> The type of entity being queried. </typeparam>
    //    /// <typeparam name="TProperty"> The type of navigation property being included. </typeparam>
    //    /// <param name="source"> The source IQueryable on which to call Include. </param>
    //    /// <param name="path"> A lambda expression representing the path to include. </param>
    //    /// <returns>
    //    /// A new IQueryable&lt;T&gt; with the defined query path.
    //    /// </returns>
    //    public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> source, Expression<Func<T, TProperty>> path) where T : class, IEntityRequirements, new()
    //    {
    //        return EntityFrameworkQueryableExtensions.Include(source, path);
    //    }

    //    /// <summary>
    //    /// Returns a new query where the entities returned will not be cached in the <see cref="DbContext" />
    //    /// .  This method works by calling the AsNoTracking method of the
    //    /// underlying query object.  If the underlying query object does not have an AsNoTracking method,
    //    /// then calling this method will have no affect.
    //    /// </summary>
    //    /// <typeparam name="T"> The element type. </typeparam>
    //    /// <param name="source"> The source query. </param>
    //    /// <returns> A new query with NoTracking applied, or the source query if NoTracking is not supported. </returns>
    //    public static IQueryable<T> AsNoTracking<T>(this IQueryable<T> source) where T : class, IEntityRequirements, new()
    //    {
    //        return EntityFrameworkQueryableExtensions.AsNoTracking(source);
    //    }



    //    /// <summary>
    //    /// Load a list asyncron
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="query"></param>
    //    /// <returns></returns>
    //    public static async Task<IEnumerable<T>> ToListAsync<T>(IQueryable<T> query)
    //    {

    //        return await Task.Run(query.ToList).ConfigureAwait(false);

    //    }


    //    /// <summary>
    //    /// Load a list asyncron
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="query"></param>
    //    /// <returns></returns>
    //    public static async Task<int> Count<T>(IQueryable<T> query)
    //    {

    //        return await Task.Run(query.Count).ConfigureAwait(false);

    //    }

    //}
//}
