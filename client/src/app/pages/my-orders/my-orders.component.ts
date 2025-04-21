import { CommonModule } from '@angular/common';
import { Component, OnInit, Pipe } from '@angular/core';
import { Order, OrderItem, OrderResponse } from '../../models/order.interface';
import { ApiService } from '../../shared/api.service';
import { AuthService } from '../../service/auth.service';
import { lastValueFrom } from 'rxjs';
import { IProduct } from '../../components/product/productModel';

@Component({
  selector: 'app-my-orders',
  imports: [CommonModule],
  templateUrl: './my-orders.component.html',
  styleUrl: './my-orders.component.css'
})
export class MyOrdersComponent implements OnInit {
  orders: Order[] = [];
  userId: string | null = null;
  error: string | null = null;

  constructor(private apiService: ApiService, private authService: AuthService) {}

  ngOnInit() {
    this.userId = this.authService.getUserId();
    if (this.userId) {
      this.fetchOrders();

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
}







