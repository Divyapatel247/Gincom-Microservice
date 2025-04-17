import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../shared/api.service';
import { IProduct } from './productModel';
import { CommonModule, CurrencyPipe, NgClass, NgFor } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../service/auth.service';

@Component({
  selector: 'app-product',
  imports: [NgFor, CurrencyPipe,NgClass, RouterLink,FormsModule,CommonModule],
  templateUrl: './product.component.html',
  styleUrl: './product.component.css'
})
export class ProductComponent implements OnInit {

  products: IProduct[] = [];
  selectedCategory: string = '';
  categories: string[] = [];
  clicked: boolean = false;
  constructor(private api : ApiService, private route : ActivatedRoute, private auth: AuthService) {

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

    const userId = parseInt(this.auth.getUserId() || '', 10);

  this.api.getProducts().subscribe(products => {
    this.products = products;

    // Loop through all products and check notification status
    this.products.forEach(product => {
      this.api.checkNotification(product.id, userId).subscribe((exists: boolean) => {
        product.IsNotifyDisabled = exists; // ✅ true if already requested
      });
    });
  });

  }

  displayProducts() {
    this.api.getProducts().subscribe((res:IProduct[]) => {
      this.products = res;
      console.log("response of getproducts",res);

    })
  }

  getProductByCategory(category: string) {
    this.api.getProductCategory(category).subscribe((res:any) => {
      this.products = res;
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

}
