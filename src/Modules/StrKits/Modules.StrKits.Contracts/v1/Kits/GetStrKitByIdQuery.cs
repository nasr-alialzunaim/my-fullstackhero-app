using FSH.Modules.StrKits.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.StrKits.Contracts.v1.Kits;

public sealed record GetStrKitByIdQuery(Guid KitId) : IQuery<StrKitDto>;
