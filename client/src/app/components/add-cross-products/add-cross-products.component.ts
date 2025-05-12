import { NgFor, NgIf } from '@angular/common';
import { Component, EventEmitter, NgModule, Output } from '@angular/core';
import { ApiService } from '../../shared/api.service';
import { FormsModule, NgModel } from '@angular/forms';
import { IProduct } from '../product/productModel';
import { LoaderComponent } from "../loader/loader.component";

@Component({
  selector: 'app-add-cross-products',
  imports: [NgFor, NgIf, FormsModule, LoaderComponent],
  templateUrl: './add-cross-products.component.html',
  styleUrl: './add-cross-products.component.css'
})
export class AddCrossProductsComponent {
  loading = false;
  constructor(private api: ApiService) {}
  categories: string[] = [];
  ngOnInit() {
      this.loadCategories();
    }

    loadCategories() {
      this.api.getCategoryList().subscribe((res: string[]) => {
        this.categories = res;
      });
    }
  crossProducts = [1, 2, 3];

  @Output() selectedProductsChange = new EventEmitter<Number[]>();

  selectedCategory: string = '';
  products: IProduct[] = [];
  selectedProductIds: Number[] = [];
  maxSelections = 4;

  onCategoryChange(): void {
    if (this.selectedCategory) {
      this.fetchProductsByCategory();
      this.loading = true;
    } else {
      this.products = [];
      this.selectedProductIds = [];
      this.selectedProductsChange.emit(this.selectedProductIds);
    }
  }

  fetchProductsByCategory(): void {
    this.api.getProductCategory(this.selectedCategory).subscribe({
      next: (products: IProduct[]) => {
        this.loading = false;
        this.products = products;
        // this.selectedProductIds = [];
        this.selectedProductsChange.emit(this.selectedProductIds);
      },
      error: (err:any) => {
        console.error('Error fetching products:', err);
        this.loading = false;
        this.products = [];
        this.selectedProductIds = [];
        this.selectedProductsChange.emit(this.selectedProductIds);
      },
    });
  }

  onCheckboxChange(event: Event, productId: Number): void {
    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      if (this.selectedProductIds.length < this.maxSelections) {
        this.selectedProductIds = [...this.selectedProductIds, productId];
      } else {
        checkbox.checked = false;
      }
    } else {
      this.selectedProductIds = this.selectedProductIds.filter((id) => id !== productId);
    }
    this.selectedProductsChange.emit(this.selectedProductIds);
  }

  removeProduct(productId: Number): void {
    this.selectedProductIds = this.selectedProductIds.filter((id) => id !== productId);
    this.selectedProductsChange.emit(this.selectedProductIds);
  }

  isCheckboxDisabled(productId: Number): boolean {
    return (
      this.selectedProductIds.length >= this.maxSelections &&
      !this.selectedProductIds.includes(productId)
    );
  }

  reset(): void {
    this.selectedCategory = '';
    this.products = [];
    this.selectedProductIds = [];
    this.selectedProductsChange.emit(this.selectedProductIds);
  }
}
