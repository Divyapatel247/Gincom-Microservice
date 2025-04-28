import { Order } from './../../models/order.interface';
import { Component, Input, OnInit } from '@angular/core';
import { CommonStatisticsComponent } from '../../components/common-statistics/common-statistics.component';
import { OrderTotalComponent } from '../../components/order-total/order-total.component';
import { LatestOrdersComponent } from '../../components/latest-orders/latest-orders.component';
import { ApiService } from '../../shared/api.service';
import { WebsocketService } from '../../service/websocket.service';

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonStatisticsComponent,
    OrderTotalComponent,
    LatestOrdersComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent implements OnInit {
  constructor(private api: ApiService, private ws: WebsocketService) {}

  orders: Order[] = [];
  latestOrders: Order[] = [];
  pendingOrders:number = 0;
  count:number = 0;
  lowStokCount: number = 0;

  ngOnInit(): void {
    this.api.getOrders().subscribe((res) => {
      console.log(res);
      this.orders = res;
      this.latestOrders = this.getLatestFiveOrders(this.orders);
      this.getOrderWithPendingStatus(this.orders);
    });

    this.api.getCustomerCount().subscribe((res)=>{
    this.count = res
    })

    this.api.getLowStockProducts().subscribe((res)=>{
      this.lowStokCount = res;
    })

    this.ws.totalOrder$.subscribe((data) => {
      this.handleRealtimeOrder(data);
    });

    this.ws.registerCustomer$.subscribe((data)=>{
      console.log("data :",data.count)
      this.count = data.count
    })

    this.ws.lowStock$.subscribe((data)=>{
      console.log("lowStock :", data.lowStock)
      this.lowStokCount = data.lowStock
    })

  }

  getLatestFiveOrders(orders: Order[]): Order[] {
    return orders
      .filter((order) => order.createdAt)
      .sort(
        (a, b) =>
          new Date(b.createdAt!).getTime() - new Date(a.createdAt!).getTime()
      )
      .slice(0, 5);
  }

  handleRealtimeOrder(data: any): void {
    if (!data.orderId) return;

    const newOrder: Order = {
      id: data.orderId,
      userId: data.userId,
      status: data.status,
      items: [], // Add items if available
      createdAt:
        data.createdAt && data.createdAt !== '0001-01-01 00:00:00'
          ? new Date(data.createdAt)
          : new Date(),
    };

    this.orders = [newOrder, ...this.orders];
    this.latestOrders = this.getLatestFiveOrders(this.orders);
  }

  getOrderWithPendingStatus(orders: Order[]) {
       this.pendingOrders =  orders.filter(order => order.status === "Pending").length;
       console.log("pending orders :", this.pendingOrders)
  }
}
