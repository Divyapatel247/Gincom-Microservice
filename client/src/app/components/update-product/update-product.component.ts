import { Component, Input, Output,EventEmitter } from '@angular/core';
import { IProduct } from '../product/productModel';
import { FormsModule, NgForm, NgModel } from '@angular/forms';
import { ApiService } from '../../shared/api.service';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-update-product',
  imports: [FormsModule, NgFor],
  templateUrl: './update-product.component.html',
  styleUrl: './update-product.component.css'
})
export class UpdateProductComponent {
  @Input() product: any = {
    id: 0,
    title: '',
    description: '',
    price: 0,
    stock: 0,
    category: '',
    discountPercentage: 0,
    rating: 0,
    images: []
  };
  @Output() update = new EventEmitter<any>();
  @Output() close = new EventEmitter<void>();
  categories: string[] = [];

 constructor(private api: ApiService) {
  this.loadCategories();
 }

 loadCategories() {
  this.api.getCategoryList().subscribe({
    next: (res: string[]) => {
      this.categories = res;
    },
    error: (err) => {
      console.error('Error loading categories:', err);
    }
  });
}

updateProduct() {
  if (this.product && this.product.id) {
    this.api.updateProduct(this.product.id, this.product).subscribe({
      next: (res: any) => {
        this.update.emit(res);
        this.close.emit();
      },
      error: (err) => {
        console.error('Error updating product:', err);
      }
    });
  } else {
    console.error('Invalid product data:', this.product);
  }
}

cancel() {
  this.close.emit();
}
}

