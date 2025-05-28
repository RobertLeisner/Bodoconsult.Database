// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore.Migrations.Operations;

// ReSharper disable LocalizableElement

namespace Bodoconsult.Database.Ef.MigrationTools.Operations
{

    /// <summary>
    /// Creates a check constraint
    /// </summary>
    /// <remarks>Based on https://www.codeproject.com/tips/786331/creating-a-custom-migration-operation-in-entity-fr </remarks>
    /// <example>this.CreateCheckConstraint("Products", "SKU", "SKU LIKE '[A-Z][A-Z]-[0-9][0-9]%'");</example>
    public class CreateCheckConstraintOperation : MigrationOperation
    {
        private string _table;
        private string _column;
        private string _checkConstraint;
        private string _checkConstraintName;

        public CreateCheckConstraintOperation()
        {
        }

        public string Table
        {
            get => _table;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(
                        "Argument is null or contains only white spaces.",
                        nameof(value));
                }

                _table = value;
            }
        }

        public string Column
        {
            get => _column;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(
                        "Argument is null or contains only white spaces.",
                        nameof(value));
                }

                _column = value;
            }
        }

        public string CheckConstraint
        {
            get => _checkConstraint;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(
                        "Argument is null or contains only white spaces.",
                        nameof(value));
                }

                _checkConstraint = value;
            }
        }

        public string CheckConstraintName
        {
            get => _checkConstraintName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(
                        "Argument is null or contains only white spaces.",
                        nameof(value));
                }

                _checkConstraintName = value;
            }
        }

        public override bool IsDestructiveChange => false;

        public string BuildDefaultName()
        {
            return $"CK_{Table}_{Column}";
        }
    }
}