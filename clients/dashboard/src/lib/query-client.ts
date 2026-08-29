import { MutationCache, QueryCache, QueryClient } from "@tanstack/react-query";
import {
  ApiRequestError,
  isImpersonationRevokedError,
} from "@/lib/api-client";
import { router } from "@/routes";

const IMPERSONATION_ENDED_PATH = "/impersonation-ended";

// Global terminal-state handler for revoked/expired impersonation sessions.
function handleGlobalError(error: unknown) {
  if (isImpersonationRevokedError(error)) {
    if (router.state.location.pathname === IMPERSONATION_ENDED_PATH) return;
    // Surface the dev-only rejection reason on the page when present; prod
    // blanks it, so the page copy must stand on its own without it.
    const reason =
      error instanceof ApiRequestError && typeof error.problem?.reason === "string"
        ? error.problem.reason
        : undefined;
    void router.navigate(IMPERSONATION_ENDED_PATH, { replace: true, state: { reason } });
  }
}

export const queryClient = new QueryClient({
  queryCache: new QueryCache({ onError: handleGlobalError }),
  mutationCache: new MutationCache({ onError: handleGlobalError }),
  defaultOptions: {
    queries: {
      retry: (failureCount, error) => {
        if (error instanceof ApiRequestError && (error.status === 401 || error.status === 403)) {
          return false;
        }
        return failureCount < 2;
      },
      staleTime: 30_000,
      refetchOnWindowFocus: false,
    },
  },
});
