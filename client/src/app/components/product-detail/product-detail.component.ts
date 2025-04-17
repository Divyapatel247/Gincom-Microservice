import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { IProduct, IReview } from '../product/productModel';
import { ApiService } from '../../shared/api.service';
import { ActivatedRoute, Router } from '@angular/router';
import {
  DatePipe,
  NgClass,
  NgFor,
  NgIf,
  Location,
  CommonModule,
} from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../service/auth.service';
import { Subscription } from 'rxjs';
import { WebsocketService } from '../../service/websocket.service';
import { Notification } from '../../models/notification.interface';


@Component({
  selector: 'app-product-detail',
  imports: [NgClass, NgIf, DatePipe, NgFor, CommonModule, FormsModule],
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

  
 

  constructor(
    private api: ApiService,
    private activadedRoute: ActivatedRoute,
    private router: Router,
    private location: Location,
    private authservice: AuthService,
    
  ) {}



  ngOnInit(): void {
    this.currentUserId = this.authservice.getUserId() ? parseInt(this.authservice.getUserId()!, 10) : null;
    console.log('currentUserId:', this.currentUserId,'type:', typeof this.currentUserId);
    console.log('isLoggedIn:', this.authservice.isLoggedIn());
    if (!this.currentUserId || !this.authservice.isLoggedIn()) {
      console.warn('No user ID or not logged in. Redirecting to login.');
      this.router.navigate(['/login']);
      return;
    }
    const productid = this.activadedRoute.snapshot.paramMap.get('productid');
    // console.log(productid);
    productid &&
      this.api.getProductById(productid).subscribe((res: IProduct) => {
        console.log(res);
        this.productDetail = res;
        
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

  // ngOnDestroy(): void {
  //   if (this.Subscription) {
  //     this.Subscription.unsubscribe();
  //   }
  // }

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

  
  
}


