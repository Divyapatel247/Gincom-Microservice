import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { AuthService } from './auth.service';
import  { HubConnection, HubConnectionBuilder} from '@microsoft/signalr';
import * as signalR from '@microsoft/signalr';

import { Notification } from '../models/notification.interface';

@Injectable({
  providedIn: 'root'
})
export class WebsocketService implements OnDestroy {
  private hubConnection: HubConnection | null = null;
  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  private notifications: Notification[] = [];
  private readonly STORAGE_KEY = 'user_notifications';
  private readonly MAX_NOTIFICATIONS = 5;


  constructor(private authService: AuthService) {
    this.loadNotificationsFromStorage();

    this.initializeSignalR();
  }

  private loadNotificationsFromStorage(): void {
    const userId = this.authService.getUserId();
    if (userId) {
      const savedNotifications = localStorage.getItem(`${this.STORAGE_KEY}_${userId}`);
      if (savedNotifications) {
        try {
          this.notifications = JSON.parse(savedNotifications);
          this.notificationsSubject.next([...this.notifications]);
        } catch (e) {
          console.error('Error parsing saved notifications:', e);
          // Reset if corrupted
          localStorage.removeItem(`${this.STORAGE_KEY}_${userId}`);
        }
      }
    }
  }

  private saveNotificationsToStorage(): void {
    const userId = this.authService.getUserId();
    if (userId) {
      // Keep only the most recent 5 notifications
      const recentNotifications = this.notifications.slice(0, this.MAX_NOTIFICATIONS);
      localStorage.setItem(`${this.STORAGE_KEY}_${userId}`, JSON.stringify(recentNotifications));
    }
  }


  private initializeSignalR() {
    const userId = this.authService.getUserId();
    const token = localStorage.getItem('access_token');
    if (userId && token) {
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(`http://localhost:5100/notificationHub?userId=${userId}`, {
          accessTokenFactory: () => token,
        })
        .configureLogging(signalR.LogLevel.None)
        .build();

      this.hubConnection
        .start()
        .then(() => {
          console.log('SignalR connection started');
          this.joinUserGroup(userId);
        })
        .catch((err) => console.error('SignalR connection error:', err));

      this.hubConnection.onclose((error) => {
        console.log('SignalR connection closed:', error);
      });

      this.hubConnection.on('ReceiveNotification', (data: any) => {
        console.log('Received notification data:', data);
        if (data.messageType === 'UserNotification') {
          const userNotification: Notification = {
            message: data.details,
            timestamp: new Date().toISOString(),
          };
          console.log('User notification received:', userNotification);

          // Add new notification to the beginning of the array
          this.notifications.unshift(userNotification);

          // Keep only the most recent notifications
          if (this.notifications.length > this.MAX_NOTIFICATIONS) {
            this.notifications = this.notifications.slice(0, this.MAX_NOTIFICATIONS);
          }

          // Update the subject and save to local storage
          this.notificationsSubject.next([...this.notifications]);
          this.saveNotificationsToStorage();
        }
      });
    } else {
      console.error('UserId or access_token not found');
    }
  }


  private joinUserGroup(userId: string) {
    if (this.hubConnection) {
      this.hubConnection.invoke('JoinUserGroup', userId)
        .then(() => console.log(`Joined group User_${userId}`))
        .catch((err) => console.error('Error joining group:', err));
    }
  }
  private reconnect() {
    const userId = this.authService.getUserId();
    const token = localStorage.getItem('access_token');
    if (userId && token && this.hubConnection?.state !== 'Connected') {
      setTimeout(() => this.initializeSignalR(), 2000);
    }
  }

  getNotifications(): Observable<Notification[]> {
    return this.notificationsSubject.asObservable();
  }

  ngOnDestroy() {
    if (this.hubConnection) {
      this.hubConnection.stop().catch((err) => console.error('Error stopping connection:', err));
    }
  }

  clearNotifications(): void {
    this.notifications = [];
    this.notificationsSubject.next([]);
    this.saveNotificationsToStorage();
  }

  updateNotifications(notifications: Notification[]): void {
    this.notifications = notifications;
    this.notificationsSubject.next([...this.notifications]);
    this.saveNotificationsToStorage();
  }
}
