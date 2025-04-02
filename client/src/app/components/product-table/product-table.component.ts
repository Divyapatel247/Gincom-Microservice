import { Component, OnInit } from '@angular/core';
import { IProduct } from '../product/productModel';
import { ApiService } from '../../shared/api.service';
import { CurrencyPipe, NgClass, NgFor, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UpdateProductComponent } from '../update-product/update-product.component';

@Component({
  selector: 'app-product-table',
  imports: [NgFor, CurrencyPipe,NgClass,RouterLink,UpdateProductComponent,NgIf],
  templateUrl: './product-table.component.html',
  styleUrl: './product-table.component.css'
})
export class ProductTableComponent implements OnInit {
  products : IProduct[] = [];
  selectedProduct: any | null = null;
  errorMessage: string = '';

  constructor(private api : ApiService) {}

  ngOnInit() {
    this.getProducts();
  }

  getProducts() {
    this.api.getProducts().subscribe({
      next: (res: IProduct[]) => {
        this.products = res;
        console.log('Products loaded:', this.products);
      },
      error: (err) => {
        this.errorMessage = 'Failed to load products. Please try again later.';
        console.error('Error loading products:', err);
      }
    });
  }

  deleteProduct(productId: number) {
    const productToDelete = this.products.find(product => product.id === productId);
    if (productToDelete) {
      const confirmDelete = window.confirm(`Are you sure you want to delete "${productToDelete.title}"?`);
      if (confirmDelete) {
        console.log('Deleting product:', productToDelete);
        this.api.deleteProduct(productId).subscribe({
          next: (response) => {
            console.log('Delete response:', response);
            this.getProducts();
            console.log(`Product with ID ${productId} deleted successfully.`);
            alert(`"${productToDelete.title}" has been deleted.`);
          },
          error: (err) => {
            this.errorMessage = `Failed to delete product with ID ${productId}. Error: ${err.status} - ${err.statusText}`;
            console.error('Error deleting product:', err);
          }
        });
      } else {
        console.log('Deletion canceled.');
      }
    } else {
      this.errorMessage = `Product with ID ${productId} not found.`;
      console.log(`Product with ID ${productId} not found.`);
    }
  }

  openUpdatePanel(product: any) {
    this.selectedProduct = { ...product };
  }

  closeUpdatePanel() {
    this.selectedProduct = null;
  }


  updateProduct(updatedProduct: any) {
    console.log('Updated Product:', updatedProduct);
    if (!updatedProduct || !updatedProduct.id) {
      console.error('Invalid product received:', updatedProduct);
      return;
    }
    const index = this.products.findIndex(p => p.id === updatedProduct.id);

    if (index !== -1) {
      this.products[index] = { ...updatedProduct };
      this.products = [...this.products];
    }
    this.closeUpdatePanel();
  }




}
