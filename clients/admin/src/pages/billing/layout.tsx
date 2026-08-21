import { NavLink, Outlet } from "react-router-dom";
import { CreditCard } from "lucide-react";
import { cn } from "@/lib/cn";
import { useTranslation } from "react-i18next";
import { EntityPageHeader } from "@/components/list";

const TAB_KEYS = [
  { to: "/billing/plans", key: "plans" },
  { to: "/billing/invoices", key: "invoices" },
] as const;

/**
 * BillingLayout — page hero + horizontal tabbed sub-nav. Child routes render
 * inside `<Outlet />`.
 */
export function BillingLayout() {
  const { t } = useTranslation();
  return (
    <div className="space-y-6">
      <EntityPageHeader
        icon={CreditCard}
        tone="saffron"
        title={t("billing.title", { defaultValue: "Billing" })}
        description={t("billing.description", { defaultValue: "Manage plans, subscriptions, and invoices across every tenant on this instance." })}
      />

      <nav
        className="flex items-center gap-1 border-b border-[var(--color-border)]"
        aria-label={t("billing.sections", { defaultValue: "Billing sections" })}
      >
        {TAB_KEYS.map((tab) => (
          <NavLink
            key={tab.to}
            to={tab.to}
            className={({ isActive }) =>
              cn(
                "relative -mb-px border-b-2 px-4 py-2.5 text-sm font-medium transition-colors",
                isActive
                  ? "border-[var(--color-foreground)] text-[var(--color-foreground)]"
                  : "border-transparent text-[var(--color-muted-foreground)] hover:text-[var(--color-foreground)]",
              )
            }
          >
            {t(`billing.${tab.key}`, { defaultValue: tab.key === "plans" ? "Plans" : "Invoices" })}
          </NavLink>
        ))}
      </nav>

      <div className="pt-1">
        <Outlet />
      </div>
    </div>
  );
}
