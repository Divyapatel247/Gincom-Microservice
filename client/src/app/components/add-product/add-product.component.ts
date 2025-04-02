import { Component, OnInit } from '@angular/core';
import { IProduct } from '../product/productModel';
import { FormGroup, FormControl, Validators, ReactiveFormsModule, FormControlName } from '@angular/forms';
import { ApiService } from '../../shared/api.service';
import { NgClass, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-add-product',
  imports: [ReactiveFormsModule,NgClass,NgIf,NgFor],
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.css'
})
export class AddProductComponent implements OnInit{

  products : IProduct[] = [];
  categories: string[] = [];
  productForm: FormGroup = new FormGroup({
    title: new FormControl("", [Validators.required, Validators.minLength(3)]),
    description: new FormControl("", [Validators.required, Validators.minLength(10)]),
    price: new FormControl(null, [Validators.required, Validators.min(1)]),
    stock: new FormControl(null, [Validators.required, Validators.min(0)]),
    discountPercentage: new FormControl(0, [Validators.min(0), Validators.max(100)]),
    category: new FormControl("", Validators.required),
    tags: new FormControl(""),
  });
  errorMessage: string = '';

  ngOnInit() {
    this.loadCategories();
  }
  constructor(private api: ApiService) {}

  loadCategories() {
    this.api.getCategoryList().subscribe((res: string[]) => {
      this.categories = res;
    });
  }


  onSubmit() {
    if (this.productForm.invalid) {
      return;
    }
    this.addProduct();
  }

  addProduct() {
    const formValues = this.productForm.value;
    const tagsFromForm = formValues.tags as string | undefined;
    const selectedCategory = formValues.category as string;

    if (!selectedCategory || !this.categories.includes(selectedCategory)) {
      this.errorMessage = 'Invalid category selected. Please choose a valid category.';
      return;
    }

    const productToAdd: Partial<IProduct> = {
      title: formValues.title,
      description: formValues.description,
      price: formValues.price,
      stock: formValues.stock,
      discountPercentage: formValues.discountPercentage,
      categoryName: selectedCategory,
      tags: tagsFromForm ? tagsFromForm.split(',').map(tag => tag.trim()) : [],
    };

    this.api.addProduct(productToAdd as IProduct).subscribe({
      next: (res: IProduct) => {
        this.products.push(res);
        this.productForm.reset();
        if (this.categories.length > 0) {
          this.productForm.get('category')?.setValue(this.categories[0]);
        }
        this.errorMessage = 'Product added successfully!';
        setTimeout(() => (this.errorMessage = ''), 3000);
      },
      error: (err) => {
        this.errorMessage = `Failed to add product: ${err.status} - ${err.statusText}`;
        console.error('Error:', err.message); 
      }
    });
  }
  }


