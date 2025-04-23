import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, Pipe } from '@angular/core';
import { Order, OrderItem, OrderResponse } from '../../models/order.interface';
import { ApiService } from '../../shared/api.service';
import { AuthService } from '../../service/auth.service';
import { lastValueFrom, Subscription } from 'rxjs';
import { IProduct } from '../../components/product/productModel';
import { WebsocketService } from '../../service/websocket.service';
import { Notification as AppNotification } from '../../models/notification.interface';

@Component({
  selector: 'app-my-orders',
  imports: [CommonModule],
  templateUrl: './my-orders.component.html',
  styleUrl: './my-orders.component.css'
})
export class MyOrdersComponent implements OnInit, OnDestroy {
  orders: Order[] = [];
  userId: string | null = null;
  notifications: AppNotification[] = [];
  private subscriptions: Subscription[] = [];
  error: string | null = null;

  constructor(private apiService: ApiService, private authService: AuthService, private ws : WebsocketService) {}

  ngOnInit() {
    this.userId = this.authService.getUserId();
    if (this.userId) {
      this.fetchOrders();
      this.subscribeToNotifications();
    } else {
      this.error = 'No user logged in';
    }
  }


  fetchOrders() {
    this.apiService.getOrdersByUserId(this.userId!).subscribe({
      next: (data: OrderResponse[]) => {
        console.log('Orders fetched:', data);
        const sortedData = [...data].sort((a, b) => b.id - a.id);
        this.orders = sortedData.map(order => ({
          id: order.id,
          userId: order.userId,
          status: order.status,
          items: order.items,
          createdAt: order.createdAt || new Date(),
          razorpayOrderId: order.razorpayOrderId || ''
        }));
        this.loadProductDetails();
      },
      error: (err) => {
        console.error('Error fetching orders:', err);
        this.error = 'Failed to load orders';
      }
    });
  }
  loadProductDetails() {
    this.orders.forEach(order => {
      order.items.forEach((item: OrderItem) => {
        this.apiService.getProduct(item.productId).subscribe({
          next: (product: IProduct) => {
            item.product = product;
          },
          error: (err) => {
            console.error(`Error fetching product ${item.productId}:`, err);
          }
        });
      });
    });
  }
getOrderTotal(order: any): number {
  let total = 0;
  for (const item of order.items) {
    total += (item.product?.price || 0) * item.quantity;
  }
  return total;
}

private subscribeToNotifications() {
  const notificationsSub = this.ws.getNotifications().subscribe((notifications: AppNotification[]) => {
    this.notifications = notifications;
  });
  this.subscriptions.push(notificationsSub);
  const statusUpdateSub = this.ws.notification$.subscribe((data: any) => {
    console.log('Raw notification received in MyOrdersComponent:', data);
    if (data.messageType === 'UserNotification' && data.orderId) {
      const order = this.orders.find(o => o.id === data.orderId);
      if (order) {
        order.status = data.newStatus;
        console.log(`Updated status of Order ${order.id} to ${order.status}`);
      }
    }
  });
  this.subscriptions.push(statusUpdateSub);
}

ngOnDestroy() {
  this.subscriptions.forEach(sub => sub.unsubscribe());
}
}






