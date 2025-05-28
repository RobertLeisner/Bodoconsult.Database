// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Bodoconsult.Database.Ef.MigrationTools.Operations
{
    /// <summary>
    /// Turn database based creation of new values for a primary key column on or off
    /// </summary>
    /// <remarks>Turns Identity on/off for a int primary key column</remarks>
    public class ChangeIdentityOperation : MigrationOperation
    {
        public ChangeIdentityOperation()
        {
            DependentColumns = new List<DependentColumn>();
        }

        public IdentityChange Change { get; set; }
        public string PrincipalTable { get; set; }
        public string PrincipalColumn { get; set; }
        public List<DependentColumn> DependentColumns { get; }

        public override bool IsDestructiveChange => false;


        #region Helper objects and enums

        public enum IdentityChange
        {
            SwitchIdentityOn,
            SwitchIdentityOff
        }



        #endregion
    }

    public class DependentColumn
    {
        public string DependentTable { get; set; }
        public string ForeignKeyColumn { get; set; }
    }
}