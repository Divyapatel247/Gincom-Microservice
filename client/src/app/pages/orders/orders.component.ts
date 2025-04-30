import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../shared/api.service';
import { Order } from '../../models/order.interface';
import { CommonModule, NgClass, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LoaderComponent } from "../../components/loader/loader.component";

@Component({
  selector: 'app-orders',
  imports: [NgFor, NgClass, FormsModule, CommonModule, LoaderComponent],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})
export class OrdersComponent implements OnInit {

  orders : Order[] = []
  loading = true;

  constructor(private api:ApiService, private route:Router){}
  ngOnInit(): void {
    this.api.getOrders().subscribe((res)=>{
      this.orders = res;
      console.log("res :",res)
      console.log(this.orders)
      this.loading = false;
    })
  }

  handleStatusChange(orderId: number, newStatus: string) {
    this.orders = this.orders.map(order =>
      order.id === orderId ? { ...order, status: newStatus } : order
    );
  this.api.changeOrderStatus(orderId,newStatus).subscribe((res)=>{
      console.log("status updated to :", res.status)
  })
  }


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
