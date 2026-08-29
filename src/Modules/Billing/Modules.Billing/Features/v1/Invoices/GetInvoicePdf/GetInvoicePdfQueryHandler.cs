using FSH.Framework.Core.Exceptions;
using FSH.Framework.Shared.Installation;
using FSH.Modules.Billing.Contracts;
using FSH.Modules.Billing.Contracts.v1.Invoices.GetInvoicePdf;
using FSH.Modules.Billing.Data;
using FSH.Modules.Billing.Services;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Billing.Features.v1.Invoices.GetInvoicePdf;

public sealed class GetInvoicePdfQueryHandler(
    BillingDbContext db,
    IInvoicePdfRenderer renderer)
    : IQueryHandler<GetInvoicePdfQuery, InvoicePdfResponse>
{
    public async ValueTask<InvoicePdfResponse> Handle(GetInvoicePdfQuery query, CancellationToken cancellationToken)
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
        return new InvoicePdfResponse(bytes, $"invoice-{invoice.InvoiceNumber}.pdf");
    }
}
