import { Component, OnInit } from '@angular/core';
import { IProduct, IReview } from '../product/productModel';
import { ApiService } from '../../shared/api.service';
import { ActivatedRoute, Router } from '@angular/router';
import {
  NgFor,
  NgIf,
  Location,
  CommonModule,
} from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../service/auth.service';
import { BasketItem, BasketResponse } from '../../models/cart.interface';



@Component({
  selector: 'app-product-detail',
  imports: [ NgIf, NgFor, CommonModule, FormsModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.css',
})
export class ProductDetailComponent implements OnInit {
  isToggle: any;
  productDetail: IProduct = {
    id: 0,
    description: '',
    stock: 0,
    categoryId: 0,
    images: [],
    title: '',
    price: 0,
    discountPercentage: 0,
    rating: 0,
    dimensions: { width: 0, height: 0, depth: 0 },
    weight: 0,
    brand: '',
    sku: '',
    tags: [],
    availabilityStatus: '',
    shippingInformation: '',
    warrantyInformation: '',
    returnPolicy: '',
    reviews: [],
  };
  quantity: number = 1;
  remainingStock: number = 0;
  loading: boolean = false;
  errorMessage: string = '';


  constructor(
    private api: ApiService,
    private activadedRoute: ActivatedRoute,
    private router: Router,
    private location: Location,
    private authService: AuthService
  ) {}



  ngOnInit(): void {
    this.currentUserId = this.authService.getUserId() ? parseInt(this.authService.getUserId()!, 10) : null;
    console.log('currentUserId:', this.currentUserId,'type:', typeof this.currentUserId);
    console.log('isLoggedIn:', this.authService.isLoggedIn());
    if (!this.currentUserId || !this.authService.isLoggedIn()) {
      console.warn('No user ID or not logged in. Redirecting to login.');
      this.router.navigate(['/login']);
      return;
    }
    let productid = this.activadedRoute.snapshot.paramMap.get('productid');
    // console.log(productid);
    productid &&
      this.api.getProductById(productid).subscribe((res: IProduct) => {
        this.productDetail = res;
        this.remainingStock = res.stock;
        console.log(res);
        this.adjustRemainingStock(this.currentUserId!.toString(), this.productDetail.id);
      });
    if (productid) {
      this.api
        .getReviewsofProduct(productid)
        .subscribe((reviews: IReview[]) => {
          this.productDetail.reviews = reviews;
          reviews.forEach(r => console.log('review.userId:', r.userId, 'type:', typeof r.userId));
          console.log('Reviews:', reviews);
        });
    }
  }

  showAllReviews: boolean = false;

  get visibleReviews() {
    if (!this.productDetail?.reviews) return [];
    return this.showAllReviews
      ? this.productDetail.reviews
      : this.productDetail.reviews.slice(0, 2);

  }

  newReview = {
    rating: 0,
    description: '',
  };

  ratingWarning: boolean = false;

  checkRating() {
    this.ratingWarning = this.newReview.rating < 1 || this.newReview.rating > 5;
  }

  addReview() {
    const productId = this.productDetail?.id;
    if (!productId) return;

    const review: IReview = {
      productId: productId,
      rating: this.newReview.rating,
      description: this.newReview.description,
      id: 0,
      userId: this.currentUserId || 0,
      createdAt: '',
    };

    this.api.addReview(this.productDetail.id, review).subscribe((res) => {
      this.productDetail.reviews?.push(res);
      this.newReview = { rating: 0, description: '' }; // clear form after submit
    });
  }

  goBack() {
    this.location.back();
  }

  currentUserId: number | null = null;
  deleteReview(reviewId: number, userId:number){
    if(!this.productDetail?.id) return;

    this.api.deleteReview(reviewId, userId).subscribe(()=>{
      this.productDetail.reviews = this.productDetail.reviews?.filter(r=>r.id !== reviewId);
    })
  }


  clampQuantity(value: number, min: number, max: number): number {
    return Math.max(min, Math.min(max, value));
  }

  get isAddDisabled(): boolean {
    return this.loading || !this.productDetail.id || this.remainingStock === 0 || this.quantity <= 0;
  }

  private adjustRemainingStock(userId: string, productId: number): void {
    this.api.getCart(userId).subscribe({
      next: (cart: BasketResponse) => {
        const existingItem = cart.items.find(item => item.productId === productId);
        const usedStock = existingItem ? existingItem.quantity : 0;
        this.remainingStock = Math.max(0, this.productDetail.stock - usedStock);
        console.log('Adjusted remainingStock:', this.remainingStock, 'usedStock:', usedStock);
      },
      error: (err) => {
        console.error('Failed to fetch cart:', err);
        this.remainingStock = this.productDetail.stock;
      }
    });
  }

  // Add to cart using bulk endpoint with specified quantity
  addToCart() {
    const userId = this.authService?.getUserId();
    console.log('addToCart - AuthService:', !!this.authService, 'userId:', userId, 'remainingStock:', this.remainingStock);
    if (this.isAddDisabled) {
      this.errorMessage = 'No stock available or invalid quantity.';
      console.warn('Cannot add to cart: button disabled.');
      return;
    }

    // Clamp quantity to remaining stock and update immediately
    const clampedQuantity = this.clampQuantity(this.quantity, 1, this.remainingStock);
    this.remainingStock -= clampedQuantity;
    this.loading = true;
    this.errorMessage = '';

    const item: BasketItem = {
      id: 0,
      productId: this.productDetail.id,
      quantity: clampedQuantity
    };

    if (!userId) {
      console.error('User ID is null. Cannot add to cart.');
      return;
    }
    this.api.addToCartBulk(userId, [item]).subscribe({
      next: (response: BasketResponse) => {
        console.log('Added to cart successfully:', response);
        this.adjustRemainingStock(userId, this.productDetail.id);
        alert(`Added ${clampedQuantity} ${this.productDetail.title}(s) to cart!`);
        this.quantity = 1;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to add to cart:', err);
        this.remainingStock += clampedQuantity;
        this.errorMessage = 'Failed to add to cart. Please try again.';
        this.loading = false;
      }
    });
  }
}
