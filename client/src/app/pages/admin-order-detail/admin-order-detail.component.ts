import { OrderItem } from './../../models/order.interface';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../shared/api.service';
import { Order } from '../../models/order.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-order-detail',
  imports: [CommonModule],
  templateUrl: './admin-order-detail.component.html',
  styleUrl: './admin-order-detail.component.css'
})
export class AdminOrderDetailComponent implements OnInit {

  orderId!: string;
  order: Order = {
    id: 1001,
    userId: 'user_123',
    status: 'Pending',
    createdAt: new Date(),
    razorpayOrderId: 'order_rp_456',
    items: [
      {
        productId: 1,
        quantity: 1,
        product: {
          id: 1,
          title: 'Apple iCam',
          price: 1300,
          sku: 'APPLE_CAM',
          thumbnail: 'https://res.cloudinary.com/demo/image/upload/sample.jpg',
          description: '',
          discountPercentage: 0,
          stock: 0,
          tags: [],
          categoryId: 0,
          relatedProductIds: []
        }
      },
      {
        productId: 1,
        quantity: 1,
        product: {
          id: 1,
          title: 'Apple iCam',
          price: 1300,
          sku: 'APPLE_CAM',
          thumbnail: 'https://res.cloudinary.com/demo/image/upload/sample.jpg',
          description: '',
          discountPercentage: 0,
          stock: 0,
          tags: [],
          categoryId: 0,
          relatedProductIds: []
        }
      },
      {
        productId: 1,
        quantity: 1,
        product: {
          id: 1,
          title: 'Apple iCam',
          price: 1300,
          sku: 'APPLE_CAM',
          thumbnail: 'https://res.cloudinary.com/demo/image/upload/sample.jpg',
          description: '',
          discountPercentage: 0,
          stock: 0,
          tags: [],
          categoryId: 0,
          relatedProductIds: []
        }
      }
    ]
  };


    constructor(private route: ActivatedRoute,private api:ApiService,private router: Router) {}

    ngOnInit() {
      this.orderId = this.route.snapshot.paramMap.get('id')!;
      console.log("orderId :", this.orderId)
      // this.api.getProductById(this.orderId).subscribe(order =>{
      //   this.orderId = p
      // });
      console.log("product :", this.orderId)
    }




    onEdit(item: any) {
      console.log('Edit:', item);
    }

    onDelete(item: any) {
      console.log('Delete:', item);
    }

}
