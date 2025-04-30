import { Component, NgModule, OnInit } from '@angular/core';
import { FormsModule, NgModel } from '@angular/forms';
import { IProduct, IProductWithRelatedProducts } from '../../components/product/productModel';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../../shared/api.service';
import { NgFor, NgIf } from '@angular/common';
import { LoaderComponent } from "../../components/loader/loader.component";

@Component({
  selector: 'app-show-product-detail',
  imports: [FormsModule, NgFor, RouterLink, NgIf, LoaderComponent],
  templateUrl: './show-product-detail.component.html',
  styleUrl: './show-product-detail.component.css'
})
export class ShowProductDetailComponent implements OnInit {
  loading = true;
  product : IProductWithRelatedProducts = {
    title: '',
    price: 0,
    discountPercentage: 0,
    dimensions: {
      width: 0,
      depth: 0,
      height: 0
    },
    sku: '',
    tags: [],
    id: 0,
    description: '',
    categoryId: 0,
    stock: 0,
    relatedProductIds: [],
    relatedProducts: [{id:0,title:'',price:0,thumbnail:''}],
    thumbnail : ''
  };
  // relatedProducts: object[] = []


  productId!: string;
  categories: string[] = [];


  constructor(private route: ActivatedRoute,private api:ApiService,private router: Router) {}

  ngOnInit() {
    this.productId = this.route.snapshot.paramMap.get('id')!;
    // console.log("productId :", this.productId)
    this.api.getProductById(this.productId).subscribe(p =>{
      this.product = p
      setTimeout(() => {
        this.loading = false;
      }, 3000);
    });
    console.log("product :", this.product)
    this.api.getCategoryList().subscribe((res: string[]) => {
      this.categories = res;
    });

  }


  relatedProducts = [1, 2, 3]; // Replace with real data

  newTag = '';



  addTag() {
    if (this.newTag.trim()) {
      this.product.tags.push(this.newTag.trim());
      this.newTag = '';
    }
    console.log("tags :",this.product.tags)
  }

  removeTag(tag: string) {
    this.product.tags = this.product.tags.filter(t => t !== tag);
    console.log("tags :",this.product.tags)
  }

  onDelete(){
    this.loading = true;
  this.api.deleteProduct(this.productId).subscribe(()=>{
    this.router.navigateByUrl("/admin/products");
  })
  }

  onSave(){
    this.loading = true;
    this.api.updateProduct(Number(this.productId),this.product).subscribe(()=>{
      this.router.navigateByUrl("/admin/products");
    })
  }
}
