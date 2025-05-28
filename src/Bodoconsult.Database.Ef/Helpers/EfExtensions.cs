// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.Helpers
{

    public static partial class EfExtensions
    {

        public static IQueryable<T> MultipleInclude<T>(this IQueryable<T> source, IEnumerable<string> navigationPropertyPaths)
            where T : class
        {
            return navigationPropertyPaths == null ? source : navigationPropertyPaths.Aggregate(source, (query, path) => query.Include(path));
        }

        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> source, IEnumerable<string> navigationPropertyPaths)
            where T : class
        {
            return navigationPropertyPaths == null ? source : navigationPropertyPaths.Aggregate(source, (query, path) => query.Include(path));
        }


        //public static IEnumerable<string> GetIncludePaths(this DbContext context, Type clrEntityType)
        //{
        //    var entityType = context.Model.FindEntityType(clrEntityType);
        //    var includedNavigations = new HashSet<INavigation>();
        //    var stack = new Stack<IEnumerator<INavigation>>();
        //    while (true)
        //    {
        //        var entityNavigations = new List<INavigation>();
        //        foreach (var navigation in entityType.GetNavigations())
        //        {
        //            if (includedNavigations.Add(navigation))
        //                entityNavigations.Add(navigation);
        //        }
        //        if (entityNavigations.Count == 0)
        //        {
        //            if (stack.Count > 0)
        //                yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
        //        }
        //        else
        //        {
        //            foreach (var navigation in entityNavigations)
        //            {
        //                var inverseNavigation = navigation.FindInverse();
        //                if (inverseNavigation != null)
        //                    includedNavigations.Add(inverseNavigation);
        //            }
        //            stack.Push(entityNavigations.GetEnumerator());
        //        }
        //        while (stack.Count > 0 && !stack.Peek().MoveNext())
        //            stack.Pop();
        //        if (stack.Count == 0) break;
        //        entityType = stack.Peek().Current.GetTargetType();
        //    }
        //}

    }
}