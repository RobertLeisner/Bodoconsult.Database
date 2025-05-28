// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using EfConsoleApp1.Model.DatabaseModel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfConsoleApp1.Model.DatabaseModel.EntityConfiguration
{
    /// <summary>
    /// Configures entity type UserType
    /// </summary>
    public class UserTypeConfig : IEntityTypeConfiguration<UserType>
    {
        public void Configure(EntityTypeBuilder<UserType> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.HasKey(x => x.ID);
            builder.ToTable("UserType");
            builder.Property(x => x.ID).ValueGeneratedOnAdd();

            builder.Property(e => e.UserTypeName).HasMaxLength(30);

            builder.Property(p => p.RowVersion).IsConcurrencyToken().IsRowVersion().IsRequired();
        }
    }
}