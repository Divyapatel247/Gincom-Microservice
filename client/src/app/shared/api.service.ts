import { HttpClient, HttpHeaders } from '@angular/common/http';
import { async, firstValueFrom, Observable, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { IProduct } from '../components/product/productModel';
import { Basket, BasketResponse } from '../models/cart.interface';
import { Order, OrderResponse } from '../models/order.interface';
import { AuthService } from '../service/auth.service';
import { Product } from '../models/product.interface';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private apiUrl = 'http://localhost:5100';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getProducts(): Observable<IProduct[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.get<IProduct[]>(`${this.apiUrl}/api/products`, {
      headers
    });
  }

  getProductById(productId: string): Observable<IProduct> {
    return this.http.get<IProduct>(`${this.apiUrl}/api/products/${productId}`);
  }

  deleteProduct(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/products/${productId}`);
  }

  addProduct(product: IProduct): Observable<IProduct> {
    return this.http.post<IProduct>(`${this.apiUrl}/api/products/add`, product);
  }

  updateProduct(productId: number, product: IProduct): Observable<IProduct> {
    return this.http.put<IProduct>(`${this.apiUrl}/api/products/${productId}`, {
      title: product.title,
      description: product.description,
      price: product.price,
      stock: product.stock,
      categoryName: product.categoryName,
    });
  }

  login(loginObj: any) {

    const headers = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded'
    });

    return this.http.post<any>(`${this.apiUrl}/connect/token`, loginObj,{headers});
  }

  register(registerObj: any) {
    return this.http.post<any>(`${this.apiUrl}/auth/register`, registerObj);
  }

  refreshToken(refreshToken: string) {
    return this.http.post<any>('https://dummyjson.com/auth/refresh', {
      refreshToken,
    });
  }

  getProductCategory(categoryName: string): Observable<IProduct[]> {
    return this.http.get<IProduct[]>(
      `${this.apiUrl}/products/category/${categoryName}`
    );
  }
  getCategoryList(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/categoryList`);
  }



  //------------------------Cart--------------------------------------------

  getCart(userId: string): Observable<BasketResponse> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.get<BasketResponse>(`${this.apiUrl}/api/orders/${userId}/cart`, {headers} );
  }

  updateCartItem(userId: string, itemId: number, quantity: number): Observable<BasketResponse> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.put<BasketResponse>(`${this.apiUrl}/api/orders/${userId}/cart/items/${itemId}`, { quantity},{ headers });
  }

  removeCartItem(userId: string, itemId: number): Observable<BasketResponse> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.delete<BasketResponse>(`${this.apiUrl}/api/orders/${userId}/cart/items/${itemId}`, {headers});
  }

  deleteCart(userId: string): Observable<any> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.delete<any>(`${this.apiUrl}/api/orders/${userId}/cart`, {headers});
  }

  createOrder(userId: string): Observable<OrderResponse> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.post<OrderResponse>(`${this.apiUrl}/api/orders/${userId}`, {}, {headers});
  }

  getProduct(productId: number): Observable<Product> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.get<Product>(`${this.apiUrl}/api/products/${productId}`, {headers});
  }

}
