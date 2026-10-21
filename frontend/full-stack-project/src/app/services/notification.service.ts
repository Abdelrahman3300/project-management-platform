import { Injectable, signal } from '@angular/core';

import {
  AppNotification,
  NotificationType
} from '../models/notification';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private nextId = 0;

  readonly notifications = signal<AppNotification[]>([]);

  success(
    message: string,
    title = 'Success'
  ): void {
    this.show('success', title, message);
  }

  error(
    message: string,
    title = 'Error'
  ): void {
    this.show('error', title, message);
  }

  warning(
    message: string,
    title = 'Warning'
  ): void {
    this.show('warning', title, message);
  }

  info(
    message: string,
    title = 'Information'
  ): void {
    this.show('info', title, message);
  }

  dismiss(id: number): void {
    this.notifications.update(items =>
      items.filter(item => item.id !== id)
    );
  }

  clear(): void {
    this.notifications.set([]);
  }

  private show(
    type: NotificationType,
    title: string,
    message: string
  ): void {
    const id = ++this.nextId;

    this.notifications.update(items => [
      ...items,
      { id, type, title, message }
    ]);

    setTimeout(() => {
      this.dismiss(id);
    }, 5000);
  }
}
