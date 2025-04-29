import { OrderItem } from './../../models/order.interface';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../shared/api.service';
import { Order } from '../../models/order.interface';
import { CommonModule } from '@angular/common';
import { LoaderComponent } from "../../components/loader/loader.component";

@Component({
  selector: 'app-admin-order-detail',
  imports: [CommonModule, LoaderComponent],
  templateUrl: './admin-order-detail.component.html',
  styleUrl: './admin-order-detail.component.css',
})
export class AdminOrderDetailComponent implements OnInit {
  loading = true
  orderId!: string;
  order: Order = {
    id: 0,
    userId: '',
    status: '',
    items: [],
  };
  orderTotle: number = 0;

  constructor(
    private route: ActivatedRoute,
    private api: ApiService,
    private router: Router
  ) {}

  ngOnInit() {
    this.orderId = this.route.snapshot.paramMap.get('id')!;
    console.log('orderId :', this.orderId);
    this.api.getOrderByOrderId(parseInt(this.orderId)).subscribe((order) => {
      this.order = order;
      this.calOrderTotle();
      this.loading = false;
    });
    console.log('product :', this.orderId);
    console.log('order:', this.order);
  }
  calOrderTotle(){
    this.order.items.forEach((item) => {
      const price = item.product?.price ?? 0;
      this.orderTotle += price * item.quantity;
    });
  }

  onEdit(item: any) {
    console.log('Edit:', item);
  }

  onDelete(item: any) {
    console.log('Delete:', item);
  }
}

// id: 1001,
//     userId: 'user_123',
//     status: 'Pending',
//     createdAt: new Date(),
//     razorpayOrderId: 'order_rp_456',
//     items: [
//       {
//         productId: 1,
//         quantity: 1,
//         product: {
//           id: 1,
//           title: 'Apple iCam',
//           price: 1300,
//           sku: 'APPLE_CAM',
//           thumbnail: 'https://res.cloudinary.com/demo/image/upload/sample.jpg',
//           description: '',
//           discountPercentage: 0,
//           stock: 0,
//           tags: [],
//           categoryId: 0,
//           relatedProductIds: []
//         }
//       },
//       {
//         productId: 1,
//         quantity: 1,
//         product: {
//           id: 1,
//           title: 'Apple iCam',
//           price: 1300,
//           sku: 'APPLE_CAM',
//           thumbnail: 'https://res.cloudinary.com/demo/image/upload/sample.jpg',
//           description: '',
//           discountPercentage: 0,
//           stock: 0,
//           tags: [],
//           categoryId: 0,
//           relatedProductIds: []
//         }
//       },
//       {
//         productId: 1,
//         quantity: 1,
//         product: {
//           id: 1,
//           title: 'Apple iCam',
//           price: 1300,
//           sku: 'APPLE_CAM',
//           thumbnail: 'https://res.cloudinary.com/demo/image/upload/sample.jpg',
//           description: '',
//           discountPercentage: 0,
//           stock: 0,
//           tags: [],
//           categoryId: 0,
//           relatedProductIds: []
//         }
//       }
//     ]
