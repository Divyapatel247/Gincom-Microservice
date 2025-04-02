import { Component, OnInit } from '@angular/core';
import { IProduct } from '../product/productModel';
import { ApiService } from '../../shared/api.service';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe, NgClass, NgFor, NgIf, Location } from '@angular/common';

@Component({
  selector: 'app-product-detail',
  imports: [NgClass, NgIf, DatePipe, NgFor],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.css'
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
      reviews: []
    };


    constructor(private api: ApiService, private activadedRoute: ActivatedRoute, private router: Router, private location: Location) {

    }

    ngOnInit(): void {
      let productid = this.activadedRoute.snapshot.paramMap.get('productid');
      // console.log(productid);
      productid && this.api.getProductById(productid).subscribe((res: IProduct) => {
        this.productDetail = res;
        console.log(res);
      })
    }

    goBack() {
      this.location.back();
    }


}
