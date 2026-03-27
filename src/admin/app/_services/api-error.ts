import { AxiosError } from "axios";
import { toast } from "sonner";

/**
 * Shape of the API's error envelope.
 * Matches the ApiResponse from the backend.
 */
interface ApiErrorResponse {
  success: false;
  message?: string;
  errors?: string[];
}

/**
 * Extract a user-friendly message from an Axios error.
 *
 * Priority:
 *  1. `errors[]` array from the ApiResponse envelope → joined with newlines
 *  2. `message` string from the ApiResponse envelope
 *  3. Status-based fallback (404, 403, 409, 500, etc.)
 *  4. Network / unknown error fallback
 */
export function extractApiError(error: unknown): string {
  if (!isAxiosError(error)) {
    return "An unexpected error occurred.";
  }

  const data = error.response?.data as ApiErrorResponse | undefined;

  // Backend returned the ApiResponse envelope with specific errors
  if (data?.errors?.length) {
    return data.errors.join("\n");
  }

  if (data?.message) {
    return data.message;
  }

  // Status-based fallbacks
  const status = error.response?.status;
  switch (status) {
    case 400:
      return "The request was invalid. Please check your input.";
    case 401:
      return "Your session has expired. Please sign in again.";
    case 403:
      return "You don't have permission to perform this action.";
    case 404:
      return "The requested resource was not found.";
    case 409:
      return "A conflict occurred. The resource may already exist.";
    case 429:
      return "Too many requests. Please wait and try again.";
    case 500:
    case 502:
    case 503:
      return "Something went wrong on our end. Please try again later.";
    default:
      break;
  }

  // Network error (no response at all)
  if (error.request && !error.response) {
    return "Unable to reach the server. Please check your connection.";
  }

  return "An unexpected error occurred.";
}

/**
 * Show a toast for an API error.
 * Call this in any catch block to surface a friendly message.
 * Safe to call on the server — it no-ops when `window` is unavailable.
 */
export function showApiError(error: unknown) {
  if (typeof window === "undefined") return;

  const message = extractApiError(error);
  toast.error(message);
}

function isAxiosError(error: unknown): error is AxiosError {
  return (
    typeof error === "object" &&
    error !== null &&
    "isAxiosError" in error &&
    (error as AxiosError).isAxiosError === true
  );
}
