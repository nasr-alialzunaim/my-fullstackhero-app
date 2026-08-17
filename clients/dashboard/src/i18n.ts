import i18n from "i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import { initReactI18next } from "react-i18next";
import enCommon from "@/locales/en-US/common.json";
import arCommon from "@/locales/ar-SA/common.json";

export const supportedCultures = ["en-US", "ar-SA"] as const;
export type SupportedCulture = (typeof supportedCultures)[number];

export function normalizeCulture(value: string | null | undefined): SupportedCulture {
  return value?.toLowerCase().startsWith("ar") ? "ar-SA" : "en-US";
}

void i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    fallbackLng: "en-US",
    supportedLngs: supportedCultures,
    load: "currentOnly",
    ns: ["common"],
    defaultNS: "common",
    resources: {
      "en-US": { common: enCommon },
      "ar-SA": { common: arCommon },
    },
    detection: {
      order: ["localStorage", "navigator"],
      lookupLocalStorage: "fsh-culture",
      caches: ["localStorage"],
    },
    interpolation: { escapeValue: false },
  });

export function applyCulture(culture: SupportedCulture): void {
  document.documentElement.lang = culture;
  document.documentElement.dir = culture === "ar-SA" ? "rtl" : "ltr";
  localStorage.setItem("fsh-culture", culture);
  void i18n.changeLanguage(culture);
}

export default i18n;
