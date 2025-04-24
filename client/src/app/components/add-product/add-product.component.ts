import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { IProduct } from '../product/productModel';
import { FormGroup, FormControl, Validators, ReactiveFormsModule, FormControlName, FormsModule } from '@angular/forms';
import { ApiService } from '../../shared/api.service';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { AddCrossProductsComponent } from "../add-cross-products/add-cross-products.component";
import { Router, RouterLink } from '@angular/router';
import { routes } from '../../app.routes';

@Component({
  selector: 'app-add-product',
  imports: [ReactiveFormsModule, NgClass, NgIf, NgFor, AddCrossProductsComponent,FormsModule],
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.css'
})
export class AddProductComponent implements OnInit{
  @ViewChild(AddCrossProductsComponent) crossProductsComponent!: AddCrossProductsComponent;

  products : IProduct[] = [];
  categories: string[] = [];

  productForm: FormGroup = new FormGroup({
    title: new FormControl("", [Validators.required, Validators.minLength(3)]),
    description: new FormControl("", [Validators.required, Validators.minLength(10)]),
    price: new FormControl(null, [Validators.required, Validators.min(1)]),
    stock: new FormControl(null, [Validators.required, Validators.min(0)]),
    discountPercentage: new FormControl(null, [Validators.min(0), Validators.max(100)]),
    category: new FormControl("", Validators.required),
    tags: new FormControl([""]),
    relatedProductIds: new FormControl([]),
    thumbnail : new FormControl("",[Validators.required, Validators.minLength(3)])
  });
  errorMessage: string = '';
  tagsArray: string[] = [];


  ngOnInit() {
    this.loadCategories();
  }
  constructor(private api: ApiService,private router:Router) {}

  loadCategories() {
    this.api.getCategoryList().subscribe((res: string[]) => {
      this.categories = res;
    });
  }

  addTag() {
    const control = this.productForm.get('tags');
  const currentValue = control?.value || '';
  const trimmed = currentValue.trim();

  if (trimmed && !this.tagsArray.includes(trimmed)) {
    this.tagsArray.push(trimmed);
    this.updateTagsControl();

  }

  // Clear input field
  control?.setValue('');
  }

  removeTag(tag: string) {
    this.tagsArray = this.tagsArray.filter(t => t !== tag);
  this.updateTagsControl();
  }

  updateTagsControl() {
    this.productForm.get('tags')?.setValue(this.tagsArray);
    console.log("tags :",this.productForm.get('tags')?.value , this.tagsArray);
  }


  onSubmit() {
    if (this.productForm.invalid) {
      return;
    }
    this.addProduct();
  }

  onSelectedProductsChange(productIds: Number[]): void {
    this.productForm.get('relatedProductIds')?.setValue(productIds);
    console.log('Selected Product IDs:',this.productForm.get('relatedProductIds')?.value);
  }

  addProduct() {
    const formValues = this.productForm.value;
    // const tagsFromForm = formValues.tags as string | undefined;

    const productToAdd: Partial<IProduct> = {
      title: formValues.title,
      description: formValues.description,
      price: formValues.price,
      stock: formValues.stock,
      discountPercentage: formValues.discountPercentage,
      categoryName: formValues.category,
      tags: this.tagsArray,
      relatedProductIds : formValues.relatedProductIds,
      thumbnail : formValues.thumbnail
    };

    console.log("productToAdd :",productToAdd)

    this.api.addProduct(productToAdd as IProduct).subscribe({
      next: (res: IProduct) => {
        this.products.push(res);
        this.productForm.reset();
        this.crossProductsComponent.reset();
        this.tagsArray = []
        this.errorMessage = 'Product added successfully!';
        setTimeout(() => (this.errorMessage = ''), 3000);
        // this.router.navigateByUrl('/admin/products')
      },
      error: (err) => {
        this.errorMessage = `Failed to add product: ${err.status} - ${err.statusText}`;
        console.error('Error:', err.message);
      }
    });
  }


  }


