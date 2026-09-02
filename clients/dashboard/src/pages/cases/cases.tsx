import {
  useEffect,
  useMemo,
  useState,
  type FormEvent,
} from "react";
import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import {
  ChevronRight,
  FolderOpen,
  Pencil,
  Plus,
  Search,
} from "lucide-react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";

import {
  createCase,
  searchCases,
  updateCase,
  type CaseDto,
  type CreateCaseInput,
  type UpdateCaseInput,
} from "@/api/cases";
import { useAuth } from "@/auth/use-auth";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogBody,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import {
  EntityEmpty,
  EntityInitialsAvatar,
  EntityListCard,
  EntityListHeader,
  EntityListLoading,
  EntityListRow,
  EntityMobileCard,
  EntityPageHeader,
  EntityPager,
  EntitySearch,
  Field,
} from "@/components/list";
import { cn } from "@/lib/cn";
import {
  describe,
  formatDate,
  formatRelative,
} from "@/lib/list-helpers";

const PAGE_SIZE = 20;
const CREATE_PERMISSION = "Permissions.Cases.Create";
const UPDATE_PERMISSION = "Permissions.Cases.Update";

type EditorState =
  | { mode: "closed" }
  | { mode: "create" }
  | { mode: "edit"; item: CaseDto };

export function CasesPage() {
  const { t } = useTranslation();
  const { user } = useAuth();
  const permissions = user?.permissions ?? [];
  const canCreate = permissions.includes(CREATE_PERMISSION);
  const canUpdate = permissions.includes(UPDATE_PERMISSION);

  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [editor, setEditor] = useState<EditorState>({ mode: "closed" });

  useEffect(() => {
    const timer = window.setTimeout(() => {
      setDebouncedSearch(search.trim());
      setPageNumber(1);
    }, 250);

    return () => window.clearTimeout(timer);
  }, [search]);

  const query = useQuery({
    queryKey: [
      "cases",
      { search: debouncedSearch, pageNumber, pageSize: PAGE_SIZE },
    ],
    queryFn: () =>
      searchCases({
        search: debouncedSearch || undefined,
        pageNumber,
        pageSize: PAGE_SIZE,
        sortBy: "createdAtUtc",
        sortDir: "desc",
      }),
    placeholderData: keepPreviousData,
  });

  const data = query.data;
  const items = data?.items ?? [];
  const searchActive = debouncedSearch.length > 0;

  return (
    <div className="space-y-4 sm:space-y-6">
      <EntityPageHeader
        icon={FolderOpen}
        title={t("cases.title", { defaultValue: "Cases" })}
        total={data?.totalCount ?? null}
        unit={t("cases.unit", { defaultValue: "case" })}
        description={t("cases.description", {
          defaultValue:
            "Investigative and identification contexts that anchor the forensic workflow.",
        })}
      >
        {canCreate && (
          <Button
            onClick={() => setEditor({ mode: "create" })}
            className="h-9 flex-1 gap-1.5 rounded-lg px-4 text-[13px] font-semibold sm:flex-none"
          >
            <Plus className="size-4" />
            {t("cases.newCase", { defaultValue: "New case" })}
          </Button>
        )}
      </EntityPageHeader>

      <EntitySearch
        value={search}
        onChange={setSearch}
        placeholder={t("cases.searchPlaceholder", {
          defaultValue: "Search by case number, title, or description…",
        })}
      />

      {query.isLoading && items.length === 0 ? (
        <EntityListLoading desktopColumns="grid-cols-[180px_1fr_170px_36px]" />
      ) : items.length === 0 ? (
        <EntityEmpty
          icon={searchActive ? Search : FolderOpen}
          title={t(searchActive ? "cases.noResults" : "cases.empty", {
            defaultValue: searchActive ? "No cases found" : "No cases yet",
          })}
          body={
            searchActive
              ? t("cases.noResultsDescription", {
                  term: debouncedSearch,
                  defaultValue: `Nothing matches “${debouncedSearch}”.`,
                })
              : t("cases.emptyDescription", {
                  defaultValue: "Create the first case to start the forensic workflow.",
                })
          }
          action={
            searchActive ? (
              <Button
                variant="outline"
                onClick={() => setSearch("")}
                className="h-9 rounded-lg px-4 text-[13px]"
              >
                {t("cases.clearSearch", { defaultValue: "Clear search" })}
              </Button>
            ) : canCreate ? (
              <Button
                onClick={() => setEditor({ mode: "create" })}
                className="h-9 rounded-lg px-4 text-[13px]"
              >
                <Plus className="me-1.5 size-4" />
                {t("cases.newCase", { defaultValue: "New case" })}
              </Button>
            ) : undefined
          }
        />
      ) : (
        <div>
          <div className="mb-3 flex items-center justify-between">
            <p className="text-[12px] font-medium text-[var(--color-muted-foreground)]">
              {t("cases.found", {
                count: data?.totalCount ?? 0,
                defaultValue: `${data?.totalCount ?? 0} case(s) found`,
              })}
            </p>
          </div>

          <div className="space-y-2 md:hidden">
            {items.map((item) => (
              <CaseMobileCard
                key={item.id}
                item={item}
                canUpdate={canUpdate}
                onEdit={() => setEditor({ mode: "edit", item })}
              />
            ))}
          </div>

          <EntityListCard className="hidden md:block">
            <EntityListHeader className="grid-cols-[180px_1fr_170px_36px]">
              <span>{t("cases.number", { defaultValue: "Case number" })}</span>
              <span>{t("cases.case", { defaultValue: "Case" })}</span>
              <span>{t("cases.created", { defaultValue: "Created" })}</span>
              <span />
            </EntityListHeader>

            {items.map((item, index) => (
              <CaseDesktopRow
                key={item.id}
                item={item}
                isLast={index === items.length - 1}
                canUpdate={canUpdate}
                onEdit={() => setEditor({ mode: "edit", item })}
              />
            ))}
          </EntityListCard>

          <EntityPager
            page={data?.pageNumber ?? 1}
            totalPages={data?.totalPages ?? 1}
            hasPrev={!!data?.hasPrevious}
            hasNext={!!data?.hasNext}
            onPrev={() => setPageNumber((page) => Math.max(1, page - 1))}
            onNext={() => setPageNumber((page) => page + 1)}
          />
        </div>
      )}

      {query.isError && (
        <div
          role="alert"
          className="rounded-lg border border-[oklch(from_var(--color-destructive)_l_c_h_/_0.30)] bg-[oklch(from_var(--color-destructive)_l_c_h_/_0.06)] px-3 py-2 text-sm text-[var(--color-destructive)]"
        >
          {t("cases.loadFailed", { defaultValue: "Could not load cases" })}:{" "}
          {describe(query.error)}
        </div>
      )}

      <CaseEditorDialog
        state={editor}
        onClose={() => setEditor({ mode: "closed" })}
      />
    </div>
  );
}

function CaseMobileCard({
  item,
  canUpdate,
  onEdit,
}: {
  item: CaseDto;
  canUpdate: boolean;
  onEdit: () => void;
}) {
  return (
    <EntityMobileCard
      href="#"
      onClick={(event) => {
        event.preventDefault();
        if (canUpdate) onEdit();
      }}
      aria-label={item.title}
    >
      <div className="flex items-center justify-between gap-3">
        <div className="flex min-w-0 items-center gap-3">
          <EntityInitialsAvatar name={item.title} size={40} />
          <div className="min-w-0">
            <p className="truncate text-[14px] font-medium text-[var(--color-foreground)]">
              {item.title}
            </p>
            <code
              className="mt-0.5 block truncate font-mono text-[11px] text-[var(--color-muted-foreground)]"
              dir="ltr"
            >
              {item.number}
            </code>
          </div>
        </div>
        {canUpdate && (
          <ChevronRight className="size-4 shrink-0 text-[var(--color-border)] rtl:rotate-180" />
        )}
      </div>

      {item.description && (
        <p className="mt-2 line-clamp-2 ps-[52px] text-[12px] text-[var(--color-muted-foreground)]">
          {item.description}
        </p>
      )}
    </EntityMobileCard>
  );
}

function CaseDesktopRow({
  item,
  isLast,
  canUpdate,
  onEdit,
}: {
  item: CaseDto;
  isLast: boolean;
  canUpdate: boolean;
  onEdit: () => void;
}) {
  return (
    <EntityListRow
      className="grid-cols-[180px_1fr_170px_36px]"
      isLast={isLast}
    >
      <code
        className="truncate font-mono text-[12px] font-medium text-[var(--color-foreground)]"
        dir="ltr"
        title={item.number}
      >
        {item.number}
      </code>

      <div className="flex min-w-0 items-center gap-3">
        <EntityInitialsAvatar name={item.title} size={36} />
        <div className="min-w-0">
          <div className="truncate text-[14px] font-medium text-[var(--color-foreground)]">
            {item.title}
          </div>
          {item.description && (
            <div
              className="mt-0.5 truncate text-[12px] text-[var(--color-muted-foreground)]"
              title={item.description}
            >
              {item.description}
            </div>
          )}
        </div>
      </div>

      <div className="min-w-0 text-[12px] text-[var(--color-muted-foreground)] tabular-nums">
        <div className="truncate">{formatDate(item.createdAtUtc)}</div>
        <div className="truncate text-[11px] opacity-70">
          {formatRelative(item.createdAtUtc)}
        </div>
      </div>

      <div className="flex items-center justify-end">
        {canUpdate ? (
          <button
            type="button"
            onClick={onEdit}
            aria-label={item.title}
            className={cn(
              "grid size-7 cursor-pointer place-items-center rounded-md",
              "text-[var(--color-muted-foreground)] opacity-0 transition-all",
              "hover:bg-[var(--color-muted)] hover:text-[var(--color-foreground)] group-hover:opacity-100",
              "focus-visible:opacity-100 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[var(--color-ring)]",
            )}
          >
            <Pencil className="size-3.5" />
          </button>
        ) : (
          <span className="size-7" aria-hidden />
        )}
      </div>
    </EntityListRow>
  );
}

function CaseEditorDialog({
  state,
  onClose,
}: {
  state: EditorState;
  onClose: () => void;
}) {
  const { t } = useTranslation();
  const queryClient = useQueryClient();
  const isOpen = state.mode === "create" || state.mode === "edit";
  const item = state.mode === "edit" ? state.item : undefined;

  const initial = useMemo(
    () => ({
      number: item?.number ?? "",
      title: item?.title ?? "",
      description: item?.description ?? "",
    }),
    [item?.number, item?.title, item?.description],
  );

  const [number, setNumber] = useState(initial.number);
  const [title, setTitle] = useState(initial.title);
  const [description, setDescription] = useState(initial.description);

  useEffect(() => {
    if (!isOpen) return;
    setNumber(initial.number);
    setTitle(initial.title);
    setDescription(initial.description);
  }, [isOpen, initial.number, initial.title, initial.description]);

  const createMutation = useMutation({
    mutationFn: (input: CreateCaseInput) => createCase(input),
    onSuccess: () => {
      toast.success(t("cases.createdToast", { defaultValue: "Case created" }));
      queryClient.invalidateQueries({ queryKey: ["cases"] });
      onClose();
    },
    onError: (error) =>
      toast.error(
        t("cases.createFailed", { defaultValue: "Could not create case" }),
        { description: describe(error) },
      ),
  });

  const updateMutation = useMutation({
    mutationFn: (input: UpdateCaseInput) => updateCase(input),
    onSuccess: () => {
      toast.success(t("cases.updatedToast", { defaultValue: "Case updated" }));
      queryClient.invalidateQueries({ queryKey: ["cases"] });
      onClose();
    },
    onError: (error) =>
      toast.error(
        t("cases.updateFailed", { defaultValue: "Could not update case" }),
        { description: describe(error) },
      ),
  });

  const isPending = createMutation.isPending || updateMutation.isPending;
  const trimmedNumber = number.trim();
  const trimmedTitle = title.trim();

  const onSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!trimmedNumber || !trimmedTitle) return;

    const payload: CreateCaseInput = {
      number: trimmedNumber,
      title: trimmedTitle,
      description: description.trim() || null,
    };

    if (state.mode === "edit" && item) {
      updateMutation.mutate({ caseId: item.id, ...payload });
      return;
    }

    createMutation.mutate(payload);
  };

  return (
    <Dialog open={isOpen} onOpenChange={(open) => (!open ? onClose() : undefined)}>
      <DialogContent className="!max-w-lg">
        <form onSubmit={onSubmit}>
          <DialogHeader>
            <DialogTitle>
              {item
                ? t("cases.edit", { defaultValue: "Edit case" })
                : t("cases.add", { defaultValue: "Add a case" })}
            </DialogTitle>
            <DialogDescription>
              {item
                ? t("cases.editDescription", {
                    defaultValue: "Update the case reference and descriptive details.",
                  })
                : t("cases.addDescription", {
                    defaultValue:
                      "Create the top-level case record. Evidence and samples follow in later workflow slices.",
                  })}
            </DialogDescription>
          </DialogHeader>

          <DialogBody className="space-y-5">
            <Field id="case-number" label={t("cases.number")} required>
              <Input
                id="case-number"
                value={number}
                onChange={(event) => setNumber(event.target.value)}
                placeholder="CASE-2026-000001"
                autoFocus
                required
                maxLength={64}
                dir="ltr"
              />
            </Field>

            <Field id="case-title" label={t("cases.titleField")} required>
              <Input
                id="case-title"
                value={title}
                onChange={(event) => setTitle(event.target.value)}
                required
                maxLength={200}
              />
            </Field>

            <Field
              id="case-description"
              label={t("cases.descriptionField")}
              hint={t("cases.descriptionHint")}
            >
              <textarea
                id="case-description"
                value={description}
                onChange={(event) => setDescription(event.target.value)}
                rows={4}
                maxLength={4096}
                className={cn(
                  "flex w-full rounded-lg border border-[var(--color-input)] bg-transparent px-3 py-2 text-sm shadow-xs",
                  "placeholder:text-[oklch(from_var(--color-muted-foreground)_l_c_h_/_0.6)]",
                  "focus-visible:border-[var(--color-ring)] focus-visible:outline-none focus-visible:ring-[3px] focus-visible:ring-[oklch(from_var(--color-ring)_l_c_h_/_0.5)]",
                )}
              />
            </Field>
          </DialogBody>

          <DialogFooter>
            <DialogClose asChild>
              <Button type="button" variant="outline" disabled={isPending}>
                {t("cases.cancel", { defaultValue: "Cancel" })}
              </Button>
            </DialogClose>

            <Button
              type="submit"
              disabled={isPending || !trimmedNumber || !trimmedTitle}
            >
              {isPending
                ? t("cases.saving", { defaultValue: "Saving…" })
                : item
                  ? t("cases.save", { defaultValue: "Save changes" })
                  : t("cases.create", { defaultValue: "Create case" })}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
