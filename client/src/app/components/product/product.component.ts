import { Component, NgModule, OnInit } from '@angular/core';
import { ApiService } from '../../shared/api.service';
import { IProduct } from './productModel';
import { CommonModule, CurrencyPipe, NgClass, NgFor } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule, NgModel } from '@angular/forms';
import { AuthService } from '../../service/auth.service';
import { WebsocketService } from '../../service/websocket.service';
import { LoaderComponent } from "../loader/loader.component";


@Component({
  selector: 'app-product',
  imports: [NgFor, CurrencyPipe, NgClass, RouterLink, FormsModule, CommonModule, LoaderComponent],
  templateUrl: './product.component.html',
  styleUrl: './product.component.css'
})
export class ProductComponent implements OnInit {
  loading = true;
  products: IProduct[] = [];
  selectedCategory: string = '';
  categories: string[] = [];
  clicked: boolean = false;
  searchTerm : string= "";
  searchResult: IProduct[] = [];
  constructor(private api : ApiService, private route : ActivatedRoute, private auth: AuthService, private websocketService: WebsocketService) {

  }
  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const category = params['category'] || '';
      this.selectedCategory = category;
      if (category) {
        this.getProductByCategory(category);
      } else {
        this.displayProducts();
      }


    });


    this.getCategoryList();

    const userId = parseInt(this.auth.getUserId() || '');
    console.log("userId", userId, typeof userId);
    this.api.getNotifiedProductIds(userId).subscribe((notifiedIds: number[])=>{
          this.products.forEach(p => {
            p.IsNotifyDisabled = notifiedIds.includes(p.id);
          });
    });

    this.websocketService.stockUpdate.subscribe(({ productId }) => {
      console.log('SignalR update received for productId:', productId);
      this.api.getProductById(productId).subscribe(updated => {
        console.log('Fetched updated product:', updated);
        const index = this.products.findIndex(p => p.id === parseInt(productId));
        if (index !== -1) {
          this.products[index] = updated;
          this.products = [...this.products]; // reassign to trigger change detection
          console.log('Product index found in list:', index);
        }
      });
    });

  }

  displayProducts() {
    this.api.getProducts().subscribe((res:IProduct[]) => {
      this.products = res;
      this.searchResult = res;
      this.loading = false;
      console.log("response of getproducts",res);

    })
  }

  getProductByCategory(category: string) {
    this.api.getProductCategory(category).subscribe((res:any) => {
      this.products = res;
      this.loading = false;
    })
  }

  getCategoryList() {
    this.api.getCategoryList().subscribe((res: string[]) => {
      this.categories = res;
      console.log('Categories in Layout:', res);
    });
  }

  getStars(rating: number | undefined): number[] {
    const validRating = rating ?? 0;
    return Array.from({ length: 5 }, (_, i) => i + 1);
  }

  notifyMe(productId: number): void{

    // const userId = this.auth.getUserId()

    // const product = this.products.find(p => p.id === productId);
    const userId = this.auth.getUserId() ? parseInt(this.auth.getUserId()!, 10) : null;
    console.log("notify me function")
    if(userId == null) return
    var exists = this.api.checkNotification(productId, userId);
    if(!exists) return
    // this.api.registerNotification(productId, userId).subscribe((res: Boolean)=>{
    //   console.log("notification registered");
    // });
    this.api.checkNotification(productId, userId).subscribe((exists: boolean) => {
      if (exists) return; // already registered

      this.api.registerNotification(productId, userId).subscribe(() => {
        console.log("notification registered");

        // ✅ Disable button for this product
        const product = this.products.find(p => p.id == productId);
        if (product) {
          product.IsNotifyDisabled = true;
        }
      });
    });
  }

  onSearch(): void{
    const trimmedQuery = this.searchTerm.trim();
  if (trimmedQuery === '') {
    this.searchResult = [...this.products];
    return;
  }
  else {
    this.searchResult = this.products.filter(product =>
      product.title.toLowerCase().includes(trimmedQuery)
    );
  }
    // this.api.searchProducts(this.searchTerm).subscribe({
    //   next: (IProduct)=>{
    //     this.searchResult = IProduct;
    //   },
    //   error: (err: IProduct) => {
    //     console.error('Search error:', err);
    //   }

    // })
  }

  }
