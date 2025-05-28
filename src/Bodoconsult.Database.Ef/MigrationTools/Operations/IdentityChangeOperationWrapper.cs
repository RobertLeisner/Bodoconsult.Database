// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.Ef.MigrationTools.Operations
{
    /// <summary>
    /// A wrapper class for the operation.
    /// Provides a method <see cref="IdentityChangeOperationWrapper.WithDependentColumn"/> to add foreign key columns
    /// related to the identity column to change.
    /// </summary>
    public class IdentityChangeOperationWrapper
    {
        private readonly ChangeIdentityOperation _operation;

        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="operation"></param>
        public IdentityChangeOperationWrapper(ChangeIdentityOperation operation)
        {
            _operation = operation;
        }

        /// <summary>
        /// Add foreign key columns related to the idenity column to change 
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="foreignKeyColumn">Name of the foreign key column</param>
        /// <returns>this object</returns>
        public IdentityChangeOperationWrapper WithDependentColumn(
            string table,
            string foreignKeyColumn)
        {
            _operation.DependentColumns.Add(new DependentColumn
            {
                DependentTable = table,
                ForeignKeyColumn = foreignKeyColumn
            });

            return this;
        }
    }
}