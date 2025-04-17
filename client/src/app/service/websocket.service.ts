import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { AuthService } from './auth.service';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Notification } from '../models/notification.interface';

@Injectable({
  providedIn: 'root'
})
export class WebsocketService implements OnDestroy {
  private hubConnection: HubConnection | null = null;
  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  private notifications: Notification[] = [];


  constructor(private authService: AuthService) {
    this.initializeSignalR();
  }

  private initializeSignalR() {
    const userId = this.authService.getUserId();
    const token = localStorage.getItem('access_token');
    if (userId && token) {
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(`http://localhost:5100/notificationHub?userId=${userId}`, {
          accessTokenFactory: () => token,
        })
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
          this.notifications.push(userNotification);
          this.notificationsSubject.next([...this.notifications]);
        }
      });
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

  sendNotification(message: string): void {
    const notification: Notification = {
      message: message,
      timestamp: new Date().toISOString(),
    };
    this.notifications.push(notification);
    this.notificationsSubject.next([...this.notifications]);
  }

  ngOnDestroy() {
    if (this.hubConnection) {
      this.hubConnection.stop().catch((err) => console.error('Error stopping connection:', err));
    }
  }

  clearNotifications(): void {
    this.notifications = [];
    this.notificationsSubject.next([]);
  }

  updateNotifications(notifications: Notification[]): void {
    this.notifications = notifications;
    this.notificationsSubject.next([...this.notifications]);
  }
}
