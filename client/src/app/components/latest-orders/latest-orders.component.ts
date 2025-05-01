import { Component, Input } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Order } from '../../models/order.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-latest-orders',
  imports: [RouterLink,CommonModule],
  templateUrl: './latest-orders.component.html',
  styleUrl: './latest-orders.component.css'
})
export class LatestOrdersComponent {
  @Input() orders: Order[] = [];
  constructor(private route:Router){}

  getStatusStyle(status: string): string {
    switch (status) {
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'Processing':
        return 'bg-blue-100 text-blue-800';
      case 'Completed':
        return 'bg-green-100 text-green-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  onView(orderId:number){
    this.route.navigateByUrl(`/admin/orders/${orderId}`)
   }
}
