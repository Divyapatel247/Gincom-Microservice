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

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    if (this.item) {
      console.log('Fetching product for productId:', this.item.productId);
      this.loadProduct();
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

  onUpdateQuantity(quantity: number) {
    if (this.item) {
      this.updateQuantity.emit({ id: this.item.id, quantity: quantity > 0 ? quantity : 1 });
    }
  }

  onRemove() {
    if (this.item) {
      this.removeItem.emit(this.item.id);
    }
  }
}
