import { CommonModule, NgFor, NgIf } from '@angular/common';
import { AfterViewChecked, ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { WebsocketService } from '../../service/websocket.service';
import { Notification } from '../../models/notification.interface';
import { AuthService } from '../../service/auth.service';
import { Observable, Subscription } from 'rxjs';

@Component({
  selector: 'app-customer-navbar',
  imports: [NgIf,NgFor,CommonModule,RouterLink],
  templateUrl: './customer-navbar.component.html',
  styleUrl: './customer-navbar.component.css',
})
export class CustomerNavbarComponent implements OnInit, OnDestroy{
  notifications: Notification[] = [];
  showNotifications = false;
  private subscription: Subscription | undefined;


  constructor(private websocketService: WebsocketService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.subscription = this.websocketService.getNotifications().subscribe(notifs => {
      this.notifications = notifs;
      console.log(this.notifications)
      // Force change detection to update the UI
      this.cdr.detectChanges();
    });
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
    this.websocketService.updateNotifications(updatedNotifications);
  }

  clearAllNotifications(): void {
    event?.stopPropagation();
    this.websocketService.clearNotifications();
  }
}
