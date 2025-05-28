using EfConsoleApp1.Model.DatabaseModel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfConsoleApp1.Model.DatabaseModel.EntityConfiguration
{

    /// <summary>
    /// Entity configuration for <see cref="AppSettings"/> entity
    /// </summary>
    public class AppSettingsConfig : IEntityTypeConfiguration<AppSettings>
    {

        public void Configure(EntityTypeBuilder<AppSettings> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }


            builder.HasKey(x => x.ID);
            builder.ToTable("AppSettings");
            builder.Property(x => x.ID).ValueGeneratedOnAdd();

            builder.Property(e => e.Key)
                    .HasMaxLength(255)
                    .IsRequired();

            builder.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken().IsRequired();

            builder.HasIndex(u => u.Key)
                //.HasName("AppSettingsKeyUnique") // Old
                .HasDatabaseName("AppSettingsKeyUnique") // New
                .IsUnique();
        }
    }
}