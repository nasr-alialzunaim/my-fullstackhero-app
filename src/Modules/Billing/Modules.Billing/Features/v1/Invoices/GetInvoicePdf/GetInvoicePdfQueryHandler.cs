using FSH.Framework.Core.Exceptions;
using FSH.Framework.Shared.Installation;
using FSH.Modules.Billing.Data;
using FSH.Modules.Billing.Services;
using Microsoft.EntityFrameworkCore;

using Mediator;

namespace FSH.Modules.Billing.Features.v1.Invoices.GetInvoicePdf;

public sealed class GetInvoicePdfQueryHandler(
    BillingDbContext db,
    IInvoicePdfRenderer renderer)
    : IQueryHandler<GetInvoicePdfQuery, InvoicePdfResult>
{
    public async ValueTask<InvoicePdfResult> Handle(GetInvoicePdfQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var invoice = await db.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(
                i => i.Id == query.InvoiceId && i.TenantId == InstallationConstants.Id,
                cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Invoice {query.InvoiceId} not found.");

        var bytes = renderer.Render(invoice);
        return new InvoicePdfResult(bytes, $"invoice-{invoice.InvoiceNumber}.pdf");
    }
}
