import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { applyCulture, normalizeCulture } from "@/i18n";

export function LocalizationRoot({ children }: { children: React.ReactNode }) {
  const { i18n } = useTranslation();

  useEffect(() => {
    const culture = normalizeCulture(i18n.language);
    applyCulture(culture);
  }, [i18n.language]);

  return <>{children}</>;
}
