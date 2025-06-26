// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using EfConsoleApp1.Model.DatabaseModel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfConsoleApp1.Model.DatabaseModel.EntityConfiguration;

/// <summary>
/// Configures entity type Article
/// </summary>
public class ArticleConfig : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.HasKey(x => x.ID);
        builder.ToTable("Article");
        builder.Property(x => x.ID).ValueGeneratedOnAdd();

        builder.Property(p => p.RowVersion).IsConcurrencyToken().IsRowVersion().IsRequired();

        builder.Property(e => e.ArticleName).HasMaxLength(255);

    }
}