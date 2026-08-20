import { Languages } from "lucide-react";
import { useTranslation } from "react-i18next";
import { applyCulture, type SupportedCulture } from "@/i18n";

const cultures: Array<{ value: SupportedCulture; labelKey: string }> = [
  { value: "en-US", labelKey: "language.english" },
  { value: "ar-SA", labelKey: "language.arabic" },
];

export function LanguageSwitcher() {
  const { i18n, t } = useTranslation();
  const current = i18n.language.toLowerCase().startsWith("ar") ? "ar-SA" : "en-US";

  return (
    <label className="inline-flex items-center gap-2 text-xs text-[var(--color-muted-foreground)]">
      <Languages size={15} aria-hidden="true" />
      <span className="sr-only">{t("language.label")}</span>
      <select
        aria-label={t("language.label")}
        value={current}
        onChange={(event) => applyCulture(event.target.value as SupportedCulture)}
        className="rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-2 py-1 text-xs text-[var(--color-foreground)]"
      >
        {cultures.map((culture) => (
          <option key={culture.value} value={culture.value}>
            {t(culture.labelKey)}
          </option>
        ))}
      </select>
    </label>
  );
}
