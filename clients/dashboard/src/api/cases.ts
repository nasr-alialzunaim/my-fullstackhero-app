import { apiFetch } from "@/lib/api-client";
import type { PagedResponse } from "@/api/catalog";

export type CaseDto = {
  id: string;
  number: string;
  title: string;
  description?: string | null;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type SearchCasesParams = {
  search?: string;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: "number" | "title" | "createdAtUtc";
  sortDir?: "asc" | "desc";
};

export type CreateCaseInput = {
  number: string;
  title: string;
  description?: string | null;
};

export type UpdateCaseInput = CreateCaseInput & {
  caseId: string;
};

export function searchCases(
  params: SearchCasesParams = {},
): Promise<PagedResponse<CaseDto>> {
  const query = new URLSearchParams();
  if (params.search) query.set("search", params.search);
  query.set("pageNumber", String(params.pageNumber ?? 1));
  query.set("pageSize", String(params.pageSize ?? 20));
  if (params.sortBy) query.set("sortBy", params.sortBy);
  if (params.sortDir) query.set("sortDir", params.sortDir);

  return apiFetch<PagedResponse<CaseDto>>(
    `/api/v1/cases?${query.toString()}`,
  );
}

export function getCaseById(id: string): Promise<CaseDto> {
  return apiFetch<CaseDto>(`/api/v1/cases/${encodeURIComponent(id)}`);
}

export function createCase(input: CreateCaseInput): Promise<string> {
  return apiFetch<string>("/api/v1/cases", {
    method: "POST",
    body: JSON.stringify({
      number: input.number,
      title: input.title,
      description: input.description ?? null,
    }),
  });
}

export function updateCase(input: UpdateCaseInput): Promise<string> {
  return apiFetch<string>(
    `/api/v1/cases/${encodeURIComponent(input.caseId)}`,
    {
      method: "PUT",
      body: JSON.stringify({
        caseId: input.caseId,
        number: input.number,
        title: input.title,
        description: input.description ?? null,
      }),
    },
  );
}
