using System.Linq.Expressions;
using System.Reflection;
using FSH.Framework.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace FSH.Modules.Auditing.Persistence;

public sealed class AuditDbContext : BaseDbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) { }

    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditDbContext).Assembly);

        var textMapping = this.GetService<IRelationalTypeMappingSource>().FindMapping(typeof(string))!;
        var asTextMethod = typeof(AuditJsonbFunctions)
            .GetMethod(nameof(AuditJsonbFunctions.AsText), BindingFlags.Public | BindingFlags.Static)!;
        modelBuilder
            .HasDbFunction(asTextMethod)
            .HasTranslation(args => new SqlUnaryExpression(
                ExpressionType.Convert,
                args[0],
                typeof(string),
                textMapping));

        base.OnModelCreating(modelBuilder);
    }
}
