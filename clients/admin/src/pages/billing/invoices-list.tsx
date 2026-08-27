import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useQuery, keepPreviousData } from "@tanstack/react-query";
import {
  ChevronLeft,
  ChevronRight,
  FileText,
  Filter,
  X,
} from "lucide-react";
import { listInvoices, type InvoiceDto, type InvoiceStatus } from "@/api/billing";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select } from "@/components/list";
import { KpiTile } from "@/components/kpi-tile";
import { ApiRequestError } from "@/lib/api-client";
import { cn } from "@/lib/cn";
import { useTranslation } from "react-i18next";

const PAGE_SIZE = 20;

const STATUSES: InvoiceStatus[] = ["Draft", "Issued", "Paid", "Void"];

// ─── helpers ─────────────────────────────────────────────────────────

function formatMoney(amount: number, currency: string) {
  try {
    return new Intl.NumberFormat(undefined, { style: "currency", currency }).format(amount);
  } catch {
    return `${amount.toFixed(2)} ${currency}`;
  }
}

const dateShort = new Intl.DateTimeFormat(undefined, {
  month: "short",
  day: "2-digit",
  year: "numeric",
});

function formatDate(iso?: string | null) {
  if (!iso) return "—";
  const d = new Date(iso);
  return Number.isNaN(d.getTime()) ? iso : dateShort.format(d);
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

function describe(err: unknown): string {
  if (err instanceof ApiRequestError) return err.problem?.detail ?? err.problem?.title ?? err.message;
  if (err instanceof Error) return err.message;
  return "Failed to load invoices.";
}

// ─── component ───────────────────────────────────────────────────────

export function InvoicesListPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(1);

  const [tenantFilter, setTenantFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState<InvoiceStatus | "">("");
  const [periodYear, setPeriodYear] = useState("");
  const [periodMonth, setPeriodMonth] = useState("");

  const filters = useMemo(
    () => ({
      tenantId: tenantFilter.trim() || undefined,
      status: statusFilter || undefined,
      periodYear: periodYear ? Number(periodYear) : undefined,
      periodMonth: periodMonth ? Number(periodMonth) : undefined,
    }),
    [tenantFilter, statusFilter, periodYear, periodMonth],
  );

  const query = useQuery({
    queryKey: ["billing", "invoices", { pageNumber, ...filters }],
    queryFn: () => listInvoices({ pageNumber, pageSize: PAGE_SIZE, ...filters }),
    placeholderData: keepPreviousData,
  });

  const data = query.data;
  // useMemo dependencies need a stable reference; wrap the optional list once
  // so both the page render and the totals memo derive from the same value.
  const items = useMemo<InvoiceDto[]>(() => data?.items ?? [], [data]);

  const totals = useMemo(() => {
    let totalBilled = 0;
    let outstanding = 0;
    let paid = 0;
    let paidCount = 0;
    const firstCurrency = items[0]?.currency ?? "USD";
    for (const inv of items) {
      totalBilled += inv.subtotalAmount;
      if (inv.status === "Paid") {
        paid += inv.subtotalAmount;
        paidCount += 1;
      } else if (inv.status === "Issued") {
        outstanding += inv.subtotalAmount;
      }
    }
    return { totalBilled, outstanding, paid, paidCount, currency: firstCurrency };
  }, [items]);

  const filtersDirty =
    !!tenantFilter || !!statusFilter || !!periodYear || !!periodMonth;

  const clearFilters = () => {
    setTenantFilter("");
    setStatusFilter("");
    setPeriodYear("");
    setPeriodMonth("");
    setPageNumber(1);
  };

  return (
    <div className="space-y-6">
      {/* KPI strip — page-scope (current page, not all-time) */}
      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4">
        <KpiTile
          label={t("billing.pageInvoices", { defaultValue: "Page invoices" })}
          value={query.isLoading ? <Skeleton className="h-7 w-16" /> : data?.items.length ?? 0}
          subtitle={data ? `${data.totalCount.toLocaleString()} ${t("billing.total", { defaultValue: "total" })}` : t("billing.loading", { defaultValue: "loading…" })}
        />
        <KpiTile
          label={t("billing.billed", { defaultValue: "Billed" })}
          value={
            query.isLoading ? (
              <Skeleton className="h-7 w-24" />
            ) : (
              formatMoney(totals.totalBilled, totals.currency)
            )
          }
          subtitle={t("billing.thisPage", { defaultValue: "this page" })}
        />
        <KpiTile
          label={t("billing.outstanding", { defaultValue: "Outstanding" })}
          value={
            query.isLoading ? (
              <Skeleton className="h-7 w-24" />
            ) : (
              formatMoney(totals.outstanding, totals.currency)
            )
          }
          subtitle={t("billing.awaitingPayment", { defaultValue: "issued, awaiting payment" })}
        />
        <KpiTile
          label={t("billing.paid", { defaultValue: "Paid" })}
          value={
            query.isLoading ? (
              <Skeleton className="h-7 w-24" />
            ) : (
              formatMoney(totals.paid, totals.currency)
            )
          }
          subtitle={`${totals.paidCount} ${t("billing.invoice", { defaultValue: "invoice" })}${totals.paidCount === 1 ? "" : "s"}`}
        />
      </div>

      {/* Filter panel */}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between gap-3">
          <div>
            <CardTitle className="flex items-center gap-2">
              <Filter className="h-4 w-4 text-[var(--color-muted-foreground)]" />
              <span>{t("billing.filters", { defaultValue: "Filters" })}</span>
            </CardTitle>
            <CardDescription>
              {t("billing.filtersDescription", { defaultValue: "All filters are AND-combined. Period is matched exactly (year + month)." })}
            </CardDescription>
          </div>
          {filtersDirty && (
            <Button variant="ghost" size="sm" onClick={clearFilters}>
              <X className="mr-1 h-3.5 w-3.5" /> {t("billing.clear", { defaultValue: "Clear" })}
            </Button>
          )}
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4">
          <div className="space-y-1.5">
            <Label htmlFor="filter-tenant">{t("billing.tenant", { defaultValue: "Tenant" })}</Label>
            <Input
              id="filter-tenant"
              placeholder={t("billing.tenantPlaceholder", { defaultValue: "tenant identifier" })}
              value={tenantFilter}
              onChange={(e) => {
                setTenantFilter(e.target.value);
                setPageNumber(1);
              }}
              autoComplete="off"
            />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="filter-status">{t("billing.status", { defaultValue: "Status" })}</Label>
            <Select
              id="filter-status"
              value={statusFilter}
              onValueChange={(v) => {
                setStatusFilter(v as InvoiceStatus | "");
                setPageNumber(1);
              }}
              options={STATUSES.map((s) => ({ value: s, label: t(`billing.invoiceStatus.${s}`, { defaultValue: s }) }))}
              emptyLabel={t("billing.all", { defaultValue: "All" })}
            />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="filter-year">{t("billing.year", { defaultValue: "Year" })}</Label>
            <Input
              id="filter-year"
              inputMode="numeric"
              placeholder="2026"
              value={periodYear}
              onChange={(e) => {
                setPeriodYear(e.target.value.replace(/[^0-9]/g, "").slice(0, 4));
                setPageNumber(1);
              }}
            />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="filter-month">{t("billing.month", { defaultValue: "Month" })}</Label>
            <Input
              id="filter-month"
              inputMode="numeric"
              placeholder="1–12"
              value={periodMonth}
              onChange={(e) => {
                setPeriodMonth(e.target.value.replace(/[^0-9]/g, "").slice(0, 2));
                setPageNumber(1);
              }}
            />
          </div>
        </CardContent>
      </Card>

      {/* List */}
      <Card>
        <CardHeader>
          <CardTitle>{t("billing.invoiceListTitle", { defaultValue: "Invoices" })}</CardTitle>
          <CardDescription>
            {data ? (
              <>
                {t("billing.page", { defaultValue: "Page" })} <span className="tabular-nums">{data.pageNumber}</span> {t("billing.of", { defaultValue: "of" })}{" "}
                <span className="tabular-nums">{Math.max(data.totalPages, 1)}</span> ·{" "}
                <span className="tabular-nums">{data.totalCount.toLocaleString()}</span> {t("billing.total", { defaultValue: "total" })}
              </>
            ) : (
              t("billing.loading", { defaultValue: "Loading…" })
            )}
          </CardDescription>
        </CardHeader>
        <CardContent className="p-0">
          {query.isError && (
            <div className="border-t border-[var(--color-border)] px-6 py-4 text-sm text-[var(--color-destructive)]">
              {describe(query.error)}
            </div>
          )}

          {query.isLoading && items.length === 0 ? (
            <ul className="divide-y divide-[var(--color-border)]">
              {Array.from({ length: 5 }).map((_, i) => (
                <li key={i} className="px-6 py-4">
                  <div className="flex items-center justify-between">
                    <div className="space-y-2">
                      <Skeleton className="h-4 w-40" />
                      <Skeleton className="h-3 w-64" />
                    </div>
                    <Skeleton className="h-5 w-20" />
                  </div>
                </li>
              ))}
            </ul>
          ) : items.length === 0 ? (
            <div className="px-6 py-10 text-center text-sm text-[var(--color-muted-foreground)]">
              {t("billing.noInvoiceMatches", { defaultValue: "No invoices match the current filters." })}
            </div>
          ) : (
            <ul>
              {items.map((inv, i) => (
                <li key={inv.id} className="border-t border-[var(--color-border)] first:border-t-0">
                  <button
                    type="button"
                    onClick={() => navigate(`/billing/invoices/${inv.id}`)}
                    className={cn(
                      "fsh-enter grid w-full grid-cols-[1fr_auto] items-center gap-x-6 gap-y-1 px-6 py-4 text-left transition-colors hover:bg-[var(--color-muted)] cursor-pointer",
                    )}
                    style={{ animationDelay: `${Math.min(i, 8) * 25}ms` }}
                  >
                  {/* Identity column */}
                  <div className="flex min-w-0 items-center gap-3">
                    <span
                      aria-hidden
                      className="grid h-9 w-9 shrink-0 place-items-center rounded-md bg-[var(--color-surface-2)] text-[var(--color-muted-foreground)] ring-1 ring-inset ring-[var(--color-border)]"
                    >
                      <FileText className="h-4 w-4" />
                    </span>
                    <div className="min-w-0">
                      <div className="flex flex-wrap items-center gap-2">
                        <code className="rounded bg-[var(--color-surface-2)] px-1.5 py-0.5 font-mono text-[11px] font-medium tracking-tight">
                          {inv.invoiceNumber}
                        </code>
                        <Badge variant={statusVariant(inv.status)}>{t(`billing.invoiceStatus.${inv.status}`, { defaultValue: inv.status })}</Badge>
                        {inv.purpose && (
                          <Badge variant="outline">
                            {inv.purpose === "Subscription" ? t("billing.subscription", { defaultValue: "Subscription" }) : t("billing.usage", { defaultValue: "Usage" })}
                          </Badge>
                        )}
                      </div>
                      <div className="mt-1 truncate font-mono text-[11px] tracking-tight text-[var(--color-muted-foreground)]">
                        {t("billing.tenant", { defaultValue: "tenant" })} <span className="text-[var(--color-foreground)]">{inv.tenantId}</span> ·
                        {t("billing.period", { defaultValue: "period" })} {formatPeriod(inv.periodYear, inv.periodMonth)} ·
                        {t("billing.createdOn", { defaultValue: "created" })} {formatDate(inv.createdAtUtc)}
                        {inv.paidAtUtc && (
                          <>
                            {" · "}
                            <span className="text-[var(--color-success)]">
                              {t("billing.paidOn", { defaultValue: "paid" })} {formatDate(inv.paidAtUtc)}
                            </span>
                          </>
                        )}
                        {inv.voidedAtUtc && (
                          <>
                            {" · "}
                            <span className="text-[var(--color-destructive)]">
                              {t("billing.voidedOn", { defaultValue: "voided" })} {formatDate(inv.voidedAtUtc)}
                            </span>
                          </>
                        )}
                      </div>
                    </div>
                  </div>

                  {/* Amount column */}
                  <div className="text-right">
                    <div className="text-display text-base font-semibold tabular-nums">
                      {formatMoney(inv.subtotalAmount, inv.currency)}
                    </div>
                    {inv.dueAtUtc && inv.status === "Issued" && (
                      <div className="font-mono text-[11px] text-[var(--color-warning)]">
                        {t("billing.due", { defaultValue: "due" })} {formatDate(inv.dueAtUtc)}
                      </div>
                    )}
                  </div>
                  </button>
                </li>
              ))}
            </ul>
          )}
        </CardContent>
      </Card>

      {/* Pagination */}
      <div className="flex items-center justify-between text-sm">
        <div className="font-mono text-[11px] uppercase tracking-[0.18em] text-[var(--color-muted-foreground)]">
          {data ? `Page ${data.pageNumber} / ${Math.max(data.totalPages, 1)}` : ""}
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            disabled={!data?.hasPrevious || query.isFetching}
            onClick={() => setPageNumber((p) => Math.max(1, p - 1))}
          >
            <ChevronLeft className="mr-1 h-4 w-4" /> Previous
          </Button>
          <Button
            variant="outline"
            size="sm"
            disabled={!data?.hasNext || query.isFetching}
            onClick={() => setPageNumber((p) => p + 1)}
          >
            Next <ChevronRight className="ml-1 h-4 w-4" />
          </Button>
        </div>
      </div>
    </div>
  );
}
