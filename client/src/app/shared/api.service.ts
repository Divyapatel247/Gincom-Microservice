import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { IProduct } from '../components/product/productModel';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private apiUrl = 'http://10.13.106.23:5145/api';

  constructor(private http: HttpClient) { }

  getProducts(): Observable<IProduct[]> {
    return this.http.get<IProduct[]>(`${this.apiUrl}/products`);
  }

  getProductById(productId: string): Observable<IProduct> {
    return this.http.get<IProduct>(`${this.apiUrl}/products/${productId}`);
  }

  deleteProduct(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/products/${productId}`);
  }

  addProduct(product: IProduct): Observable<IProduct> {
    return this.http.post<IProduct>(`${this.apiUrl}/products/add`, product);
  }

  updateProduct(productId: number, product: IProduct): Observable<IProduct> {
    return this.http.put<IProduct>(`${this.apiUrl}/products/${productId}`, {
      title: product.title,
      description: product.description,
      price: product.price,
      stock: product.stock,
      categoryName: product.categoryName
    });
  }


  login(loginObj: any) {
    return this.http.post<any>(`${this.apiUrl}/login`, loginObj);
  }

  refreshToken(refreshToken: string) {
    return this.http.post<any>("https://dummyjson.com/auth/refresh", { refreshToken });
  }

  getProductCategory(categoryName: string): Observable<IProduct[]> {
    return this.http.get<IProduct[]>(`${this.apiUrl}/products/category/${categoryName}`);
  }
  getCategoryList() : Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/categoryList`);
  }



}
