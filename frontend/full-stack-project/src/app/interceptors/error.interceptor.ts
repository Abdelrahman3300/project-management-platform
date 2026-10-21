import {
  HttpErrorResponse,
  HttpInterceptorFn
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notifications = inject(NotificationService);

  return next(req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse) {
        let message: string;

        switch (error.status) {
          case 0:
            message = 'Cannot connect to the server. Check your connection and ensure the API is running.';
            break;

          case 400:
            message = getValidationMessage(error) ??
              'The request was invalid. Check the information and try again.';
            break;

          case 401:
            // Do not replace the login page's invalid-credentials message.
            if (!req.url.includes('/api/auth/login') &&
                !req.url.includes('/api/auth/register')) {
              message = 'Your session may have expired. Please sign in again.';
            } else {
              message = '';
            }
            break;

          case 403:
            message = 'You do not have permission to perform this action.';
            break;

          case 404:
            message = 'The requested resource could not be found.';
            break;

          case 409:
            message = 'This operation conflicts with the current data.';
            break;

          case 422:
            message = getValidationMessage(error) ??
              'Some of the submitted information could not be processed.';
            break;

          case 429:
            message = 'Too many requests. Please wait and try again.';
            break;

          default:
            message = error.status >= 500
              ? 'A server error occurred. Please try again later.'
              : 'An unexpected request error occurred.';
        }

        if (message) {
          notifications.error(message);
        }
      }

      return throwError(() => error);
    })
  );
};

function getValidationMessage(
  error: HttpErrorResponse
): string | null {
  const body = error.error;

  if (!body || typeof body !== 'object') {
    return null;
  }

  if (typeof body.message === 'string' && body.message.trim()) {
    return body.message;
  }

  if (typeof body.title === 'string' && body.title.trim()) {
    return body.title;
  }

  if (body.errors && typeof body.errors === 'object') {
    const messages = Object.values(body.errors as Record<string, unknown>)
      .flatMap(value => Array.isArray(value) ? value : [value])
      .filter((value): value is string =>
        typeof value === 'string' && value.trim().length > 0
      );

    if (messages.length > 0) {
      return messages.slice(0, 3).join(' ');
    }
  }

  return null;
}
