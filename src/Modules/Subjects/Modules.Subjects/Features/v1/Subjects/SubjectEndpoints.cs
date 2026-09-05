using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Web.Idempotency;
using FSH.Modules.Subjects.Contracts.Authorization;
using FSH.Modules.Subjects.Contracts.v1.Subjects;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Subjects.Features.v1.Subjects;

public static class SubjectEndpoints
{
    internal static void MapSubjectEndpoints(this IEndpointRouteBuilder group)
    {
        group.MapPost("", async (CreateSubjectCommand command, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(command, ct))).WithName("CreateSubject").RequirePermission(SubjectsPermissions.Create).WithIdempotency();
        group.MapGet("{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(new GetSubjectByIdQuery(id), ct))).WithName("GetSubjectById").RequirePermission(SubjectsPermissions.View);
        group.MapGet("", async (string? search, string? subjectType, string? status, int? pageNumber, int? pageSize, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(new SearchSubjectsQuery(search, subjectType, status, pageNumber ?? 1, pageSize ?? 20), ct))).WithName("SearchSubjects").RequirePermission(SubjectsPermissions.View);
        group.MapPut("{id:guid}/identity", async (Guid id, UpsertPersonIdentityCommand command, IMediator mediator, CancellationToken ct) => { if (command.SubjectId != id) return Results.BadRequest(); await mediator.Send(command, ct); return Results.NoContent(); }).WithName("UpsertPersonIdentity").RequirePermission(SubjectsPermissions.Update).WithIdempotency();
    }
}
