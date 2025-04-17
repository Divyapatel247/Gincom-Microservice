import { Component, NgModule, OnInit } from '@angular/core';
import { FormsModule, NgModel } from '@angular/forms';
import { IProduct } from '../../components/product/productModel';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../shared/api.service';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-show-product-detail',
  imports: [FormsModule,NgFor],
  templateUrl: './show-product-detail.component.html',
  styleUrl: './show-product-detail.component.css'
})
export class ShowProductDetailComponent implements OnInit {
  product : IProduct = {
    title: '',
    price: 0,
    discountPercentage: 0,
    dimensions: {
      width: 0,
      depth: 0,
      height: 0
    },
    sku: '',
    tags: ['Beauty', 'Eyeshadow'],
    id: 0,
    description: '',
    categoryId: 0,
    stock: 0
  };


  productId!: string;


  constructor(private route: ActivatedRoute,private api:ApiService,private router: Router) {}

  ngOnInit() {
    this.productId = this.route.snapshot.paramMap.get('id')!;
    console.log("productId :", this.productId)
    this.api.getProductById(this.productId).subscribe(p => this.product = p);
    console.log("product :", this.product)
  }


  relatedProducts = [1, 2, 3]; // Replace with real data
  crossProducts = [1, 2, 3]; // Replace with real data

  newTag = '';

  // save(int productId) {
  //   // send update to backend
  //   this.api.updateProduct(productId,this.product); // update cache
  //   this.router.navigate(['/products']); // go back
  // }

  addTag() {
    if (this.newTag.trim()) {
      this.product.tags.push(this.newTag.trim());
      this.newTag = '';
    }
  }

  removeTag(tag: string) {
    this.product.tags = this.product.tags.filter(t => t !== tag);
  }
}
