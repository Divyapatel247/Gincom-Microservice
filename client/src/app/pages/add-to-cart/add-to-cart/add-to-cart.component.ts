import { Component, OnInit } from '@angular/core';
import { Basket, BasketResponse } from '../../../models/cart.interface';
import { ApiService } from '../../../shared/api.service';
import { CartItemComponent } from "../../../components/cart-item/cart-item/cart-item.component";
import { CommonModule, NgForOf, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-to-cart',
  imports: [CartItemComponent,NgIf,NgForOf,CommonModule,FormsModule],
  templateUrl: './add-to-cart.component.html',
  styleUrl: './add-to-cart.component.css'
})
export class AddToCartComponent implements OnInit {
  basket: BasketResponse | null = null;
  totalQuantity: number = 0;
  totalAmount: number = 0;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    // this.apiService.getCart('user123').subscribe(data => {
    //   this.basket = data;
    //   this.calculateTotals();
    // });
    const dummyBasket: BasketResponse = {
      id: 1,
      userId: 'user123',
      items: [
        { id: 1, productId: 1, quantity: 2 },
        { id: 2, productId: 2, quantity: 1 },
      ],
      createdAt: new Date(),
      totalAmount: 29.97,
    };
    this.basket = dummyBasket;
    this.calculateTotals();
  }

  calculateTotals() {
    if (this.basket) {
      this.totalQuantity = this.basket.items.reduce((sum, item) => sum + item.quantity, 0);
      this.totalAmount = this.basket.totalAmount || this.basket.items.reduce((sum, item) => sum + (9.99 * item.quantity), 0);
    }
  }

  updateQuantity(event: { id: number; quantity: number }) {
    if (this.basket) {
      const item = this.basket.items.find(i => i.id === event.id);
      if (item) {
        item.quantity = event.quantity > 0 ? event.quantity : 1;
        // this.apiService.updateCartItem('user123', event.id, item.quantity).subscribe(data => {
        //   this.basket = data;
        //   this.calculateTotals();
        // });
        const dummyResponse: BasketResponse = {
          id: 1,
          userId: 'user123',
          items: this.basket.items.map(i => i.id === event.id ? { ...i, quantity: item.quantity } : i),
          createdAt: new Date(),
          totalAmount: item.quantity * 9.99 + (this.basket.items.find(i => i.id !== event.id)?.quantity || 0) * 9.99,
        };
        this.basket = dummyResponse;
        this.calculateTotals();
      }
    }
  }

  removeItem(itemId: number) {
    if (this.basket) {
      this.basket.items = this.basket.items.filter(i => i.id !== itemId);
      // this.apiService.removeCartItem('user123', itemId).subscribe(data => {
      //   this.basket = data;
      //   this.calculateTotals();
      // });
      const dummyResponse: BasketResponse = {
        id: 1,
        userId: 'user123',
        items: this.basket.items.filter(i => i.id !== itemId),
        createdAt: new Date(),
        totalAmount: this.basket.items.reduce((sum, i) => sum + (9.99 * i.quantity), 0),
      };
      this.basket = dummyResponse;
      this.calculateTotals();
    }
  }

  makeOrder() {
    // this.apiService.createOrder('user123').subscribe(response => {
    //   console.log('Order created:', response);
    //   this.basket = null;
    // });
    this.apiService.createOrder('user123').subscribe(response => {
      console.log('Order created:', response);
      this.basket = null;
    });
    console.log('Order created for user123 with total amount:', this.totalAmount);
    this.basket = null;
  }
}
