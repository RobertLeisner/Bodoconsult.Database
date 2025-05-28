// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Bodoconsult.Database.Ef.MigrationTools.Operations
{
    /// <summary>
    /// Alter a view
    /// </summary>
    public class AlterViewOperation : MigrationOperation
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="viewBody">SQL body of the view</param>
        public AlterViewOperation(string viewName, string viewBody)
        {
            ViewName = viewName;
            ViewBody = viewBody;
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="viewBody">SQL body of the view</param>
        /// <param name="comment">Comment for the view</param>
        public AlterViewOperation(string viewName, string viewBody, string comment)
        {
            ViewName = viewName;
            ViewBody = viewBody;
            Comment = comment;
        }

        /// <summary>
        /// View name
        /// </summary>
        public string ViewName { get; }

        /// <summary>
        /// SQL body of the view
        /// </summary>
        public string ViewBody { get; }

        /// <summary>
        /// Comment for the view (optional)
        /// </summary>
        public string Comment { get; }


        /// <summary>
        ///     Indicates whether or not the operation might result in loss of data in the database.
        /// </summary>
        public override bool IsDestructiveChange => false;
    }
}