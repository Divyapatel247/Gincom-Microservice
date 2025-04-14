import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BasketItem } from '../../../models/cart.interface';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product } from '../../../models/product.interface';
import { ApiService } from '../../../shared/api.service';

@Component({
  selector: 'app-cart-item',
  imports: [DatePipe,FormsModule,CommonModule],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.css'
})
export class CartItemComponent implements OnInit {
  @Input() item!: BasketItem;
  @Output() updateQuantity = new EventEmitter<{ id: number; quantity: number }>();
  @Output() removeItem = new EventEmitter<number>();
  product: Product | null = null;
  stock: number = 0;
  error: string | null = null;


  constructor(private apiService: ApiService) {}

  ngOnInit() {
    if (this.item) {
      console.log('Fetching product for productId:', this.item.productId);
      this.loadProduct();
      this.checkStock();
    } else {
      console.warn('Item input is undefined in CartItemComponent');
    }
  }

  loadProduct() {
    this.apiService.getProduct(this.item.productId).subscribe({
      next: (data) => {
        console.log('Product fetched:', data);
        this.product = data;
      },
      error: (err) => console.error('Error fetching product:', err),
    });
  }

  checkStock() {
    this.apiService.getProduct(this.item.productId).subscribe({
      next: (product) => {
        this.stock = product.stock || 0;
        if (this.item.quantity <= this.stock) {
          this.error = null;
        }
      },
      error: (err) => console.error('Error checking stock:', err),
    });
  }

  onUpdateQuantity(quantity: number) {
    if (this.item) {
      if (quantity > this.stock) {
        this.error = `Quantity exceeds available stock (${this.stock}). Please reduce it.`;
        quantity = this.stock;
      } else if (quantity < 1) {
        quantity = 1;
      } else {
        this.error = null;
      }
      this.updateQuantity.emit({ id: this.item.id, quantity });

    }
  }

  onRemove() {
    if (this.item) {
      this.removeItem.emit(this.item.id);
    }
  }
}
