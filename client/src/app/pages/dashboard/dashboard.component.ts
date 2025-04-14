import { Component } from '@angular/core';
import { CommonStatisticsComponent } from "../../components/common-statistics/common-statistics.component";
import { OrderTotalComponent } from "../../components/order-total/order-total.component";
import { LatestOrdersComponent } from "../../components/latest-orders/latest-orders.component";

@Component({
  selector: 'app-dashboard',
  imports: [CommonStatisticsComponent, OrderTotalComponent, LatestOrdersComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {

}
