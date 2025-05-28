// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using EfConsoleApp1.Model.DatabaseModel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfConsoleApp1.Model.DatabaseModel.EntityConfiguration
{
    /// <summary>
    /// Configures entity type Users
    /// </summary>
    public class UsersConfig : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.HasKey(x => x.ID);
            builder.ToTable("Users");
            builder.Property(x => x.ID).ValueGeneratedOnAdd();

            builder.Property(e => e.Username).HasMaxLength(30);

            builder.Property(e => e.Realname).HasMaxLength(40);

            builder.Property(e => e.Password).HasMaxLength(15);

            builder.Property(p => p.RowVersion).IsConcurrencyToken().IsRowVersion().IsRequired();
        }
    }
}