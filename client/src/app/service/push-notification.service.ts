// import { HttpClient } from '@angular/common/http';
// import { inject, Injectable } from '@angular/core';
// import { initializeApp } from '@angular/fire/app';
// import { getMessaging, getToken, Messaging, onMessage } from '@angular/fire/messaging';
// import { firebaseConfig } from '../../environments/firebase-config';

// @Injectable({
//   providedIn: 'root'
// })
// export class PushNotificationService {
//   private messaging;
//   private http = inject(HttpClient);

//   constructor() {
//     console.log('Initializing Firebase in PushNotificationService...');
//     const app = initializeApp(firebaseConfig);
//     this.messaging = getMessaging(app);
//     console.log('Firebase initialized successfully.');
//   }

//   async requestPermission(): Promise<string | null> {
//     try {
//       console.log('Requesting notification permission...');
//       const permission = await Notification.requestPermission();
//       if (permission === 'granted') {
//         console.log('Notification permission granted.');

//         console.log('Retrieving FCM token...');
//         const token = await getToken(this.messaging, {
//           vapidKey: 'BO2B3gwLI7GmJWJ96e01gJL0ogq7893uW-T0GvUIyZ_ohLx2r6jFUHqMyRddXQsS5B6RSvzYW6cLV3_7ZGUwWdY' // Replace with your VAPID key
//         });
//         console.log('FCM Token:', token);
//         return token;
//       } else {
//         console.log('Notification permission denied.');
//         return null;
//       }
//     } catch (error) {
//       console.error('Error requesting permission:', error);
//       return null;
//     }
//   }

//   listenForMessages(): void {
//     console.log('Setting up foreground message listener...');
//     onMessage(this.messaging, (payload) => {
//       console.log('Foreground message received:', payload);
//       const notificationTitle = payload.notification?.title || 'Default Title';
//       const notificationOptions = {
//         body: payload.notification?.body || 'Default Body',
//         icon: '/favicon.ico'
//       };
//       new Notification(notificationTitle, notificationOptions);
//     });
//   }

//   registerToken(userId: string, token: string): void {
//     console.log('Registering token for UserId:', userId, 'with token:', token);
//     console.log('Sending token to:', `http://3.111.30.105:5100/api/notifications/register`);
//     this.http.post('http://ec2-3-111-30-105.ap-south-1.compute.amazonaws.com:5100/api/notifications/register', { userId, token }).subscribe({
//       next: () => console.log('Token registered successfully'),
//       error: (err) => console.error('Error registering token:', err)
//     });
//   }
// }

