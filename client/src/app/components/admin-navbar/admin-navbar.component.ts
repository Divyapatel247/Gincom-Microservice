import { Router } from '@angular/router';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { Subscription } from 'rxjs';
import { WebsocketService } from '../../service/websocket.service';
import { AuthService } from '../../service/auth.service';
import { Notification } from '../../models/notification.interface';

@Component({
  selector: 'app-admin-navbar',
  imports: [NgIf, NgFor,CommonModule],
  templateUrl: './admin-navbar.component.html',
  styleUrl: './admin-navbar.component.css'
})
export class AdminNavbarComponent implements OnInit, OnDestroy {
  notifications: Notification[] = [];
  showNotifications = false;
  private subscription: Subscription | undefined;
  isAdmin: boolean = false;

  constructor(
    private websocketService: WebsocketService,
    private cdr: ChangeDetectorRef,
    private authService: AuthService
  ) {}
  ngOnInit(): void {
    this.isAdmin = this.authService.getRole() === 'Admin';
    if (this.isAdmin) {
      this.subscription = this.websocketService.getAdminNotifications().subscribe(notifs => {
        this.notifications = notifs;
        console.log('Admin notifications:', this.notifications);
        this.cdr.detectChanges();
      });
    }
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  toggleNotifications(): void {
    this.showNotifications = !this.showNotifications;
  }

  deleteNotification(index: number): void {
    event?.stopPropagation();
    const updatedNotifications = [...this.notifications];
    updatedNotifications.splice(index, 1);
    this.websocketService.updateAdminNotifications(updatedNotifications);
  }

  clearAllNotifications(): void {
    event?.stopPropagation();
    this.websocketService.clearAdminNotifications();
  }
}


