import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowLeft, Ban, CheckCircle2, Download, FileText, Send } from "lucide-react";
import { toast } from "sonner";
import {
  downloadInvoicePdf,
  getInvoice,
  issueInvoice,
  markInvoicePaid,
  voidInvoice,
  type InvoiceStatus,
  type InvoiceLineItemDto,
} from "@/api/billing";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { Input } from "@/components/ui/input";
import { EntityPageHeader, SettingsSection, Field } from "@/components/list";
import { ApiRequestError } from "@/lib/api-client";
import { cn } from "@/lib/cn";
import { useAuth } from "@/auth/use-auth";
import { BillingPermissions } from "@/lib/permissions";
import { useTranslation } from "react-i18next";

// ─── helpers ─────────────────────────────────────────────────────────

function formatMoney(amount: number, currency: string) {
  try {
    return new Intl.NumberFormat(undefined, { style: "currency", currency }).format(amount);
  } catch {
    return `${amount.toFixed(2)} ${currency}`;
  }
}

const dateLong = new Intl.DateTimeFormat(undefined, {
  month: "long",
  day: "numeric",
  year: "numeric",
});
function formatDate(iso?: string | null) {
  if (!iso) return "—";
  const d = new Date(iso);
  return Number.isNaN(d.getTime()) ? iso : dateLong.format(d);
}

function formatPeriod(year: number, month: number) {
  return `${year}-${String(month).padStart(2, "0")}`;
}

function statusVariant(status: InvoiceStatus): React.ComponentProps<typeof Badge>["variant"] {
  switch (status) {
    case "Paid":
      return "success";
    case "Issued":
      return "info";
    case "Draft":
      return "warning";
    case "Void":
      return "danger";
    default:
      return "default";
  }
}

function describe(err: unknown, fallback: string): string {
  if (err instanceof ApiRequestError) return err.problem?.detail ?? err.problem?.title ?? err.message;
  if (err instanceof Error) return err.message;
  return fallback;
}

// ─── component ───────────────────────────────────────────────────────

export function InvoiceDetailPage() {
  const { t } = useTranslation();
  const { invoiceId = "" } = useParams<{ invoiceId: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { user: currentUser } = useAuth();
  // Issue / mark-paid / void and PDF download all require Billing.Manage on the server.
  const canManageBilling = (currentUser?.permissions ?? []).includes(BillingPermissions.Manage);

  const query = useQuery({
    queryKey: ["billing", "invoice", invoiceId],
    queryFn: () => getInvoice(invoiceId),
    enabled: !!invoiceId,
  });
  const invoice = query.data;

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ["billing", "invoice", invoiceId] });
    queryClient.invalidateQueries({ queryKey: ["billing", "invoices"] });
  };

  // ── state-machine mutations ────────────────────────────────────────

  const [dueAt, setDueAt] = useState("");
  const [voidReason, setVoidReason] = useState("");

  // Pass id + number via mutate(arg) — never close over invoice state, which
  // could be stale if the query refetched between render and click.
  const downloadMutation = useMutation({
    mutationFn: ({ id, number }: { id: string; number: string }) => downloadInvoicePdf(id, number),
    onError: (err) => toast.error(t("billing.downloadFailed", { defaultValue: "Download failed" }), { description: describe(err, t("billing.downloadInvoiceFailed", { defaultValue: "Could not download the invoice PDF." })) }),
  });

  const issueMutation = useMutation({
    mutationFn: () => issueInvoice(invoiceId, dueAt ? new Date(dueAt).toISOString() : null),
    onSuccess: () => {
      toast.success(t("billing.invoiceIssued", { defaultValue: "Invoice issued" }), { description: t("billing.statusMovedIssued", { defaultValue: "Status moved to Issued." }) });
      setDueAt("");
      invalidate();
    },
    onError: (err) => toast.error(t("billing.issueFailed", { defaultValue: "Issue failed" }), { description: describe(err, t("billing.issueInvoiceFailed", { defaultValue: "Could not issue invoice." })) }),
  });

  const payMutation = useMutation({
    mutationFn: () => markInvoicePaid(invoiceId),
    onSuccess: () => {
      toast.success(t("billing.markedPaid", { defaultValue: "Marked paid" }));
      invalidate();
    },
    onError: (err) => toast.error(t("billing.markPaidFailed", { defaultValue: "Mark-paid failed" }), { description: describe(err, t("billing.markPaidInvoiceFailed", { defaultValue: "Could not mark paid." })) }),
  });

  const voidMutation = useMutation({
    mutationFn: () => voidInvoice(invoiceId, voidReason.trim() ? voidReason.trim() : null),
    onSuccess: () => {
      toast.success(t("billing.invoiceVoided", { defaultValue: "Invoice voided" }));
      setVoidReason("");
      invalidate();
    },
    onError: (err) => toast.error(t("billing.voidFailed", { defaultValue: "Void failed" }), { description: describe(err, t("billing.voidInvoiceFailed", { defaultValue: "Could not void invoice." })) }),
  });

  // ── render ─────────────────────────────────────────────────────────

  return (
    <div className="space-y-6">
      <div>
        <Button variant="ghost" size="sm" onClick={() => navigate("/billing/invoices")} className="-ml-2 mb-4">
          <ArrowLeft className="mr-1 h-4 w-4" /> {t("billing.allInvoices", { defaultValue: "All invoices" })}
        </Button>

        {query.isLoading ? (
          <div className="space-y-2">
            <Skeleton className="h-7 w-72" />
            <Skeleton className="h-4 w-96" />
          </div>
        ) : query.isError ? (
          <div className="text-sm text-[var(--color-destructive)]">
            {describe(query.error, "Failed to load invoice.")}
          </div>
        ) : invoice ? (
          <EntityPageHeader
            icon={FileText}
            tone="saffron"
            title={formatMoney(invoice.subtotalAmount, invoice.currency)}
            description={
              <span className="flex flex-wrap items-center gap-2">
                <code className="rounded bg-[var(--color-surface-2)] px-1.5 py-0.5 font-mono text-[11px] font-medium tracking-tight">
                  {invoice.invoiceNumber}
                </code>
                <Badge variant={statusVariant(invoice.status)}>{t(`billing.invoiceStatus.${invoice.status}`, { defaultValue: invoice.status })}</Badge>
                {invoice.purpose && (
                  <Badge variant="outline">
                    {invoice.purpose === "Subscription" ? t("billing.subscription", { defaultValue: "Subscription" }) : t("billing.usage", { defaultValue: "Usage" })}
                  </Badge>
                )}
                <span className="font-mono text-[11px] text-[var(--color-muted-foreground)]">
                  tenant {invoice.tenantId} · period {formatPeriod(invoice.periodYear, invoice.periodMonth)} · created {formatDate(invoice.createdAtUtc)}
                  {invoice.periodStartUtc && invoice.periodEndUtc && (
                    ` · term ${formatDate(invoice.periodStartUtc)} – ${formatDate(invoice.periodEndUtc)}`
                  )}
                  {invoice.issuedAtUtc && ` · issued ${formatDate(invoice.issuedAtUtc)}`}
                  {invoice.dueAtUtc && invoice.status === "Issued" && (
                    <span className="text-[var(--color-warning)]"> · due {formatDate(invoice.dueAtUtc)}</span>
                  )}
                  {invoice.paidAtUtc && (
                    <span className="text-[var(--color-success)]"> · paid {formatDate(invoice.paidAtUtc)}</span>
                  )}
                  {invoice.voidedAtUtc && (
                    <span className="text-[var(--color-destructive)]"> · voided {formatDate(invoice.voidedAtUtc)}</span>
                  )}
                </span>
              </span>
            }
          >
            {canManageBilling && (
              <Button
                variant="outline"
                size="sm"
                onClick={() =>
                  downloadMutation.mutate({ id: invoice.id, number: invoice.invoiceNumber })
                }
                disabled={downloadMutation.isPending}
                title={t("billing.downloadInvoice", { defaultValue: "Download this invoice as a PDF" })}
              >
                <Download className="mr-1.5 h-3.5 w-3.5" />
                {downloadMutation.isPending ? t("billing.preparing", { defaultValue: "Preparing…" }) : t("billing.downloadPdf", { defaultValue: "Download PDF" })}
              </Button>
            )}
          </EntityPageHeader>
        ) : null}
      </div>

      <div className="grid gap-6 lg:grid-cols-[1fr_320px]">
        {/* Line items */}
        <SettingsSection
          title={t("billing.lineItems", { defaultValue: "Line items" })}
          description={
            invoice
              ? t("billing.lineCount", { count: invoice.lineItems.length, defaultValue: `${invoice.lineItems.length} line${invoice.lineItems.length === 1 ? "" : "s"}` })
              : query.isError
                ? t("billing.unavailable", { defaultValue: "Unavailable" })
                : t("billing.loading", { defaultValue: "Loading…" })
          }
        >
          {query.isError ? (
            <div className="py-8 text-center text-sm text-[var(--color-destructive)]">
              {describe(query.error, "Failed to load line items.")}
            </div>
          ) : query.isLoading ? (
            <ul className="-mx-5 divide-y divide-[var(--color-border)] border-t border-[var(--color-border)]">
              {Array.from({ length: 2 }).map((_, i) => (
                <li key={i} className="px-5 py-4">
                  <Skeleton className="h-4 w-1/2" />
                  <Skeleton className="mt-2 h-3 w-1/4" />
                </li>
              ))}
            </ul>
          ) : invoice && invoice.lineItems.length === 0 ? (
            <div className="py-8 text-center text-sm text-[var(--color-muted-foreground)]">
              {t("billing.noLineItems", { defaultValue: "No line items." })}
            </div>
          ) : invoice ? (
            <ul className="-mx-5 border-t border-[var(--color-border)]">
              {invoice.lineItems.map((li, i) => (
                <LineItemRow key={li.id} item={li} currency={invoice.currency} delayIndex={i} />
              ))}
              <li className="grid grid-cols-[1fr_auto] items-baseline gap-x-6 border-t-2 border-[var(--color-border-strong)] px-5 py-4">
                <div className="font-mono text-[11px] uppercase tracking-[0.18em] text-[var(--color-muted-foreground)]">
                  {t("billing.subtotal", { defaultValue: "subtotal" })}
                </div>
                <div className="text-display text-xl font-semibold tabular-nums">
                  {formatMoney(invoice.subtotalAmount, invoice.currency)}
                </div>
              </li>
            </ul>
          ) : null}
        </SettingsSection>

        {/* Actions side panel */}
        <div className="space-y-4">
          {invoice && (
            <>
              {/* Issue / Mark-paid / Void all mutate invoice state — gated behind
                  Billing.Manage. View-only users still see read-only Notes below. */}
              {canManageBilling && (
                <>
              {/* Issue */}
              <SettingsSection
                icon={Send}
                title={t("billing.issue", { defaultValue: "Issue" })}
                description={t("billing.issueDescription", { defaultValue: "Transition from Draft to Issued status." })}
              >
                <div className={cn("space-y-3", invoice.status !== "Draft" && "opacity-60")}>
                  <Field id="dueAt" label={t("billing.dueDate", { defaultValue: "Due date" })} hint={t("billing.dueDateHint", { defaultValue: "Leave blank for server default (+14 days)." })}>
                    <Input
                      id="dueAt"
                      type="date"
                      value={dueAt}
                      onChange={(e) => setDueAt(e.target.value)}
                      disabled={invoice.status !== "Draft" || issueMutation.isPending}
                    />
                  </Field>
                  <Button
                    size="sm"
                    disabled={invoice.status !== "Draft" || issueMutation.isPending}
                    onClick={() => issueMutation.mutate()}
                    className="w-full"
                  >
                    {issueMutation.isPending ? t("billing.issuing", { defaultValue: "Issuing…" }) : t("billing.issueInvoice", { defaultValue: "Issue invoice" })}
                  </Button>
                </div>
              </SettingsSection>

              {/* Mark paid */}
              <SettingsSection
                icon={CheckCircle2}
                title={t("billing.markPaid", { defaultValue: "Mark paid" })}
                description={t("billing.markPaidDescription", { defaultValue: "Records manual payment receipt. Idempotent." })}
              >
                <div className={cn(invoice.status !== "Issued" && "opacity-60")}>
                  <Button
                    size="sm"
                    disabled={invoice.status !== "Issued" || payMutation.isPending}
                    onClick={() => payMutation.mutate()}
                    className="w-full"
                  >
                    {payMutation.isPending ? t("billing.saving", { defaultValue: "Saving…" }) : t("billing.markAsPaid", { defaultValue: "Mark as paid" })}
                  </Button>
                </div>
              </SettingsSection>

              {/* Void */}
              <SettingsSection
                icon={Ban}
                title={t("billing.void", { defaultValue: "Void" })}
                description={t("billing.voidDescription", { defaultValue: "Cancel from Draft or Issued. Irreversible." })}
              >
                <div
                  className={cn(
                    "space-y-3",
                    (invoice.status === "Paid" || invoice.status === "Void") && "opacity-60",
                  )}
                >
                  <Field id="voidReason" label={t("billing.reason", { defaultValue: "Reason" })} hint={t("billing.reasonHint", { defaultValue: "Optional — appended to notes." })}>
                    <Input
                      id="voidReason"
                      placeholder={t("billing.voidReasonPlaceholder", { defaultValue: "duplicate · disputed · …" })}
                      value={voidReason}
                      onChange={(e) => setVoidReason(e.target.value)}
                      disabled={
                        invoice.status === "Paid" ||
                        invoice.status === "Void" ||
                        voidMutation.isPending
                      }
                    />
                  </Field>
                  <Button
                    variant="destructive"
                    size="sm"
                    disabled={
                      invoice.status === "Paid" ||
                      invoice.status === "Void" ||
                      voidMutation.isPending
                    }
                    onClick={() => voidMutation.mutate()}
                    className="w-full"
                  >
                    {voidMutation.isPending ? t("billing.voiding", { defaultValue: "Voiding…" }) : t("billing.voidInvoice", { defaultValue: "Void invoice" })}
                  </Button>
                </div>
              </SettingsSection>
                </>
              )}

              {invoice.notes && (
                <SettingsSection title={t("billing.notes", { defaultValue: "Notes" })}>
                  <p className="whitespace-pre-line text-xs text-[var(--color-foreground)]">
                    {invoice.notes}
                  </p>
                </SettingsSection>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
}

// ─── subcomponents ───────────────────────────────────────────────────

function LineItemRow({
  item,
  currency,
  delayIndex,
}: {
  item: InvoiceLineItemDto;
  currency: string;
  delayIndex: number;
}) {
  return (
    <li
      className="fsh-enter grid grid-cols-[1fr_auto] items-baseline gap-x-6 border-b border-[var(--color-border)] last:border-b-0 px-5 py-3"
      style={{ animationDelay: `${Math.min(delayIndex, 6) * 30}ms` }}
    >
      <div className="min-w-0">
        <div className="flex flex-wrap items-center gap-2">
          <span className="text-sm font-medium">{item.description}</span>
          <Badge variant={item.kind === "BaseFee" ? "default" : item.kind === "Overage" ? "warning" : "muted"}>
            {item.kind}
          </Badge>
          {item.resource && (
            <span className="font-mono text-[10.5px] uppercase tracking-[0.18em] text-[var(--color-muted-foreground)]">
              {item.resource}
            </span>
          )}
        </div>
        <div className="mt-1 font-mono text-[11px] text-[var(--color-muted-foreground)] tabular-nums">
          {item.quantity.toLocaleString()} × {formatMoney(item.unitPrice, currency)}
        </div>
      </div>
      <div className="text-right text-sm font-semibold tabular-nums">
        {formatMoney(item.amount, currency)}
      </div>
    </li>
  );
}

