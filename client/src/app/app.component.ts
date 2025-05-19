import { OAuthService } from 'angular-oauth2-oidc';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import 'flowbite';
import { HttpClient } from '@angular/common/http';
// import { PushNotificationService } from './service/push-notification.service';
import { AuthService } from './service/auth.service';


@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  showPermissionDeniedMessage = false

  // constructor(private pushService : PushNotificationService, private auth: AuthService) {
  //   console.log('AppComponent initialized.');

  // }
  title = 'Encom';


  userId: string | undefined;


  ngOnInit(): void {
    // this.userId = this.auth.getUserId() ?? undefined;
    // console.log('User ID:', this.userId);

    // console.log('Registering service worker...');
    // if ('serviceWorker' in navigator && 'PushManager' in window) {
    //   navigator.serviceWorker.register('/firebase-messaging-sw.js')
    //     .then(registration => console.log('Service Worker registered:', registration))
    //     .catch(err => console.error('Service Worker registration failed:', err));
    // }
    // this.pushService.listenForMessages();
    // console.log('Automatically requesting notification permission on page load...');
    // this.enableNotifications();
  }

  // async enableNotifications(): Promise<void> {
  //   console.log('Enabling notifications...');
  //   const token = await this.pushService.requestPermission();
  //   if (token) {
  //     const userId = this.userId ?? 'defaultUserId';
  //     console.log('User ID:', userId);
  //     this.pushService.registerToken(userId, token);
  //     this.showPermissionDeniedMessage = false;
  //   } else {
  //     if (Notification.permission === 'denied') {
  //       this.showPermissionDeniedMessage = true;
  //     }
  //     console.log('No token received');

  //   }
  // }
}
