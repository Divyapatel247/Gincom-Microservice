importScripts('https://www.gstatic.com/firebasejs/10.12.2/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/10.12.2/firebase-messaging-compat.js');

firebase.initializeApp({
  apiKey: "AIzaSyCs9h_GPmoj85_uEElAI32osSlrL8a_nkQ",
  authDomain: "encom-56b00.firebaseapp.com",
  projectId: "encom-56b00",
  storageBucket: "encom-56b00.firebasestorage.app",
  messagingSenderId: "267200120945",
  appId: "1:267200120945:web:1a72a52e655ae58e72b3d0"
});

const messaging = firebase.messaging();

messaging.onBackgroundMessage((payload) => {
  console.log('[firebase-messaging-sw.js] Background message:', payload);
  const notificationTitle = payload.notification?.title || 'Default Title';
  const notificationOptions = {
    body: payload.notification?.body || 'Default Body',
    icon: '/favicon.ico',
    data: { url: payload.notification?.click_action || '/' }
  };
  self.registration.showNotification(notificationTitle, notificationOptions);
});

self.addEventListener('notificationclick', (event) => {
  event.notification.close();
  const url = event.notification.data.url;
  event.waitUntil(clients.openWindow(url));
});
