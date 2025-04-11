import { Component, EventEmitter, Input, Output } from '@angular/core';
import { BasketItem } from '../../../models/cart.interface';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-cart-item',
  imports: [DatePipe,FormsModule,CommonModule],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.css'
})
export class CartItemComponent {
  @Input() item!: BasketItem;
  @Input() productPrice: number = 9.99; // Dummy price
  @Output() updateQuantity = new EventEmitter<{ id: number; quantity: number }>();
  @Output() removeItem = new EventEmitter<number>();

  onQuantityChange(event: Event) {
    const quantity = +(event.target as HTMLInputElement).value;
    this.updateQuantity.emit({ id: this.item.id, quantity: quantity > 0 ? quantity : 1 });
  }

  onRemove() {
    this.removeItem.emit(this.item.id);
  }
}
