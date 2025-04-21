import { Component, OnInit } from '@angular/core';
import { CommonStatisticsComponent } from "../../components/common-statistics/common-statistics.component";
import { OrderTotalComponent } from "../../components/order-total/order-total.component";
import { LatestOrdersComponent } from "../../components/latest-orders/latest-orders.component";
import { Order } from '../../models/order.interface';
import { ApiService } from '../../shared/api.service';


@Component({
  selector: 'app-dashboard',
  imports: [CommonStatisticsComponent, OrderTotalComponent, LatestOrdersComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  constructor(private api:ApiService){}

  orders: Order[]=[]
  latestOrders : Order[] = []

   ngOnInit(): void {
     this.api.getOrders().subscribe((res)=>{
      console.log(res)
      this.orders = res
      this.latestOrders =  this.getLatestFiveOrders(this.orders);
     })
   }

    getLatestFiveOrders(orders: Order[]): Order[] {
    return orders
      .filter(order => order.createdAt)
      .sort((a, b) => new Date(b.createdAt!).getTime() - new Date(a.createdAt!).getTime())
      .slice(0, 5);
  }


}
