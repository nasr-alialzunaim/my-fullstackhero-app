using FSH.Framework.Shared.Constants;
using FSH.Modules.Identity.Contracts.DTOs;
using FSH.Modules.Identity.Contracts.v1.Permissions.GetPermissionCatalog;
using Mediator;

namespace FSH.Modules.Identity.Features.v1.Permissions.GetPermissionCatalog;

public sealed class GetPermissionCatalogQueryHandler
    : IQueryHandler<GetPermissionCatalogQuery, IReadOnlyList<PermissionCatalogEntryDto>>
{
    public ValueTask<IReadOnlyList<PermissionCatalogEntryDto>> Handle(
        GetPermissionCatalogQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        // A single installation exposes the full installation-admin permission catalog.
        var source = PermissionConstants.Admin
            .Concat(PermissionConstants.Root)
            .DistinctBy(p => p.Name);

        IReadOnlyList<PermissionCatalogEntryDto> result =
        [
            .. source.Select(p => new PermissionCatalogEntryDto(
                p.Name,
                p.Description,
                p.Resource,
                p.Action,
                p.IsBasic,
                p.IsRoot))
        ];

        return ValueTask.FromResult(result);
    }
}
