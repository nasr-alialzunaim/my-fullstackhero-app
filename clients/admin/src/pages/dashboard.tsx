import { Link } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import {
  ArrowRight,
  FileText,
  LayoutDashboard,
  Receipt,
  UsersRound,
} from "lucide-react";
import { listInvoices, getPlans } from "@/api/billing";
import { Skeleton } from "@/components/ui/skeleton";
import { EntityPageHeader, Stat, StatStrip, ToneIconTile, type ToneIconTileTone } from "@/components/list";
import { useAuth } from "@/auth/use-auth";
import { cn } from "@/lib/cn";
import { useTranslation } from "react-i18next";

/**
 * DashboardPage — the operator overview. EntityPageHeader greeting,
 * four KPI stat tiles drawing from real data, then pivot cards into
 * the rest of the app. No fake "Coming soon" filler.
 */
export function DashboardPage() {
  const { t } = useTranslation();
  const { user } = useAuth();

  const plansQuery = useQuery({
    queryKey: ["billing", "plans", { includeInactive: true }],
    queryFn: () => getPlans(true),
  });
  const invoicesQuery = useQuery({
    queryKey: ["billing", "invoices", { pageNumber: 1, pageSize: 50 }],
    queryFn: () => listInvoices({ pageNumber: 1, pageSize: 50 }),
  });

  const plans = plansQuery.data ?? [];
  const activePlans = plans.filter((p) => p.isActive).length;
  const invoicesPage = invoicesQuery.data;
  const outstandingCount =
    invoicesPage?.items.filter((i) => i.status === "Issued").length ?? 0;

  const firstName = user?.name?.split(" ")[0];

  return (
    <div className="space-y-6">
      {/* ── Page header ──────────────────────────────────────────────── */}
      <div className="fsh-enter">
        <EntityPageHeader
          icon={LayoutDashboard}
          title={
            <>
              {t("dashboard.overview", { defaultValue: "Overview" })}{firstName ? (
                <span className="text-[var(--color-muted-foreground)]">, {firstName}</span>
              ) : null}
            </>
          }
          tone="primary"
          description={t("dashboard.description", { defaultValue: "Operate this installation — identity, billing, auditing, and the rest of the system surface." })}
        />
      </div>

      {/* ── KPI stat strip ───────────────────────────────────────────── */}
      <StatStrip cols={3} className="fsh-enter fsh-enter-2">
        <Stat
          label={t("dashboard.plans", { defaultValue: "Plans" })}
          value={
            plansQuery.isLoading ? (
              <Skeleton className="h-7 w-16" />
            ) : (
              plans.length.toLocaleString()
            )
          }
          hint={`${activePlans} ${t("dashboard.active", { defaultValue: "active" })}`}
        />
        <Stat
          label={t("billing.invoices", { defaultValue: "Invoices" })}
          value={
            invoicesQuery.isLoading ? (
              <Skeleton className="h-7 w-16" />
            ) : (
              invoicesPage?.items.length.toLocaleString() ?? "—"
            )
          }
          hint={
            invoicesPage
              ? `${invoicesPage.totalCount.toLocaleString()} total ledger`
              : t("dashboard.loading", { defaultValue: "loading…" })
          }
        />
        <Stat
          label={t("billing.outstanding", { defaultValue: "Outstanding" })}
          value={
            invoicesQuery.isLoading ? (
              <Skeleton className="h-7 w-16" />
            ) : (
              outstandingCount.toLocaleString()
            )
          }
          hint={t("billing.awaitingPayment", { defaultValue: "issued, awaiting payment" })}
          tone={outstandingCount > 0 ? "warning" : "default"}
        />
      </StatStrip>

      {/* ── Quick pivots ─────────────────────────────────────────────── */}
      <section className="fsh-enter fsh-enter-3 space-y-3">
        <p className="text-[11px] font-semibold uppercase tracking-wider text-[var(--color-muted-foreground)]">
          {t("dashboard.entryPoints", { defaultValue: "Entry points" })}
        </p>
        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
          <PivotCard
            to="/users"
            icon={UsersRound}
            tone="primary"
            title={t("navigation.users", { defaultValue: "Users" })}
            description={t("dashboard.usersDescription", { defaultValue: "Installation users and role management." })}
          />
          <PivotCard
            to="/billing/plans"
            icon={Receipt}
            tone="success"
            title={t("navigation.billing", { defaultValue: "Billing" })}
            description={t("dashboard.billingDescription", { defaultValue: "Plans, subscriptions, invoices and pricing." })}
          />
          <PivotCard
            to="/billing/invoices"
            icon={FileText}
            tone="warning"
            title={t("billing.invoices", { defaultValue: "Invoices" })}
            description={t("dashboard.invoicesDescription", { defaultValue: "Invoice ledger. Issue, mark paid, and void." })}
          />
        </div>
      </section>
    </div>
  );
}

// ─── subcomponents ───────────────────────────────────────────────────

function PivotCard({
  to,
  icon: Icon,
  tone,
  title,
  description,
}: {
  to: string;
  icon: typeof UsersRound;
  tone: ToneIconTileTone;
  title: string;
  description: string;
}) {
  return (
    <Link to={to} className="group block focus:outline-none">
      <div
        className={cn(
          "flex h-full flex-col gap-3 rounded-xl border border-[var(--color-border)] bg-[var(--color-card)] p-4 shadow-xs",
          "transition-colors duration-200 hover:border-[var(--color-border-strong)] hover:bg-[var(--color-accent)]",
        )}
      >
        <div className="flex items-start justify-between">
          <ToneIconTile icon={Icon} tone={tone} size="md" />
          <ArrowRight
            aria-hidden
            className="size-3.5 text-[var(--color-muted-foreground)] opacity-0 transition-all duration-200 group-hover:translate-x-0.5 group-hover:opacity-100"
          />
        </div>
        <div>
          <div className="font-display text-[14px] font-semibold tracking-tight text-[var(--color-foreground)]">
            {title}
          </div>
          <p className="mt-0.5 text-[12px] leading-snug text-[var(--color-muted-foreground)]">
            {description}
          </p>
        </div>
      </div>
    </Link>
  );
}
