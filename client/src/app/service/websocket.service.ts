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
  private userNotifications: Notification[] = [];
  private adminNotificationsSubject = new BehaviorSubject<Notification[]>([]);
  private adminNotifications: Notification[] = [];
  private readonly USER_STORAGE_KEY = 'user_notifications';
  private readonly ADMIN_STORAGE_KEY = 'admin_notifications';
  private readonly MAX_NOTIFICATIONS = 5;

  constructor(private authService: AuthService) {
    this.loadNotificationsFromStorage();
    this.initializeSignalR();
    setInterval(() => this.clearOldNotifications(), 1800000);
  }

  private loadNotificationsFromStorage(): void {
    const userId = this.authService.getUserId();
    const role = this.authService.getRole();
    if (userId && role !== 'Admin') {
      const savedUserNotifications = localStorage.getItem(`${this.USER_STORAGE_KEY}_${userId}`);
      if (savedUserNotifications) {
        try {
          this.userNotifications = JSON.parse(savedUserNotifications);
          this.notificationsSubject.next([...this.userNotifications]);
        } catch (e) {
          console.error('Error parsing user notifications:', e);
          localStorage.removeItem(`${this.USER_STORAGE_KEY}_${userId}`);
        }
      }
    }
    if (role === 'Admin') {
      const savedAdminNotifications = localStorage.getItem(this.ADMIN_STORAGE_KEY);
      if (savedAdminNotifications) {
        try {
          this.adminNotifications = JSON.parse(savedAdminNotifications);
          this.adminNotificationsSubject.next([...this.adminNotifications]);
        } catch (e) {
          console.error('Error parsing admin notifications:', e);
          localStorage.removeItem(this.ADMIN_STORAGE_KEY);
        }
      }
    }
  }

  private saveNotificationsToStorage(): void {
    const userId = this.authService.getUserId();
    const role = this.authService.getRole();
    if (userId && role !== 'Admin') {
      const recentUserNotifications = this.userNotifications.slice(0, this.MAX_NOTIFICATIONS);
      localStorage.setItem(`${this.USER_STORAGE_KEY}_${userId}`, JSON.stringify(recentUserNotifications));
    }
    if (role === 'Admin') {
      const recentAdminNotifications = this.adminNotifications.slice(0, this.MAX_NOTIFICATIONS);
      localStorage.setItem(this.ADMIN_STORAGE_KEY, JSON.stringify(recentAdminNotifications));
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
          this.joinGroups(userId);
        })
        .catch((err) => {
          console.error('SignalR connection error:', err);
          this.reconnect();
        });

      this.hubConnection.onclose((error) => {
        console.log('SignalR connection closed:', error);
        this.reconnect();
      });

      this.hubConnection.on('ReceiveNotification', (data: any) => {
        console.log('Received notification data:', data);
        const notification: Notification = {
          message: data.details || 'No details',
          timestamp: new Date().toISOString(),
          status: data.status || 'Unknown',
          createdAt: data.createdAt || new Date().toISOString(), 
        };
        const role = this.authService.getRole();

        if (data.messageType === 'UserNotification' && role !== 'Admin') {
          this.userNotifications.unshift(notification);
          if (this.userNotifications.length > this.MAX_NOTIFICATIONS) {
            this.userNotifications = this.userNotifications.slice(0, this.MAX_NOTIFICATIONS);
          }
          this.notificationsSubject.next([...this.userNotifications]);
        } else if (data.messageType === 'AdminNotification' && role === 'Admin') {
          this.adminNotifications.unshift(notification);
          if (this.adminNotifications.length > this.MAX_NOTIFICATIONS) {
            this.adminNotifications = this.adminNotifications.slice(0, this.MAX_NOTIFICATIONS);
          }
          this.adminNotificationsSubject.next([...this.adminNotifications]);
        }

        this.saveNotificationsToStorage();
      });
    } else {
      console.error('UserId or access_token not found');
    }
  }

  private joinGroups(userId: string) {
    if (this.hubConnection) {
      const role = this.authService.getRole();
      if (role !== 'Admin') {
        this.hubConnection.invoke('JoinUserGroup', userId)
          .then(() => console.log(`Joined group User_${userId}`))
          .catch((err) => console.error('Error joining user group:', err));
      } else {
        this.hubConnection.invoke('JoinAdminGroup')
          .then(() => console.log('Joined group Admin'))
          .catch((err) => {
            console.error('Error joining admin group:', err.message);
            if (err.source && err.source.error) {
              console.error('Server error details:', err.source.error);
            }
          });
      }
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

  getAdminNotifications(): Observable<Notification[]> {
    return this.adminNotificationsSubject.asObservable();
  }

  sendNotification(message: string): void {
    const notification: Notification = {
      message: message,
      timestamp: new Date().toISOString(),
    };
    this.userNotifications.push(notification);
    this.notificationsSubject.next([...this.userNotifications]);
    this.saveNotificationsToStorage();
  }

  ngOnDestroy() {
    if (this.hubConnection) {
      this.hubConnection.stop().catch((err) => console.error('Error stopping connection:', err));
    }
  }

  clearNotifications(): void {
    this.userNotifications = [];
    this.adminNotifications = [];
    this.notificationsSubject.next([]);
    this.adminNotificationsSubject.next([]);
    this.saveNotificationsToStorage();
  }

  updateNotifications(notifications: Notification[]): void {
    this.userNotifications = notifications;
    this.notificationsSubject.next([...this.userNotifications]);
    this.saveNotificationsToStorage();
  }

  updateAdminNotifications(notifications: Notification[]): void {
    this.adminNotifications = notifications;
    this.adminNotificationsSubject.next([...this.adminNotifications]);
    this.saveNotificationsToStorage();
  }

  clearAdminNotifications(): void {
    this.adminNotifications = [];
    this.adminNotificationsSubject.next([]);
    this.saveNotificationsToStorage();
  }

  private clearOldNotifications(): void {
    const userId = this.authService.getUserId();
    if (userId && this.authService.getRole() !== 'Admin') {
      localStorage.removeItem(`${this.USER_STORAGE_KEY}_${userId}`);
      this.userNotifications = [];
    }
    if (this.authService.getRole() === 'Admin') {
      localStorage.removeItem(this.ADMIN_STORAGE_KEY);
      this.adminNotifications = [];
    }
    this.notificationsSubject.next([]);
    this.adminNotificationsSubject.next([]);
    console.log('Notifications cleared from localStorage after 30 minutes');
  }
}
