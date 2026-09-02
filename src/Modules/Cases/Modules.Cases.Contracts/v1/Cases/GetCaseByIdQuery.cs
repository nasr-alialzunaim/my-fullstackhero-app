using FSH.Modules.Cases.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Cases.Contracts.v1.Cases;

public sealed record GetCaseByIdQuery(Guid CaseId) : IQuery<CaseDto>;