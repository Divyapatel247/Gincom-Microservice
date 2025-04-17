import { HttpClient, HttpHeaders } from '@angular/common/http';

import { BehaviorSubject, tap } from 'rxjs';

import { async, firstValueFrom, Observable, of } from 'rxjs';

import { Injectable } from '@angular/core';
import { IProduct, IReview } from '../components/product/productModel';
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

  private products: IProduct[] = [];
  private productsSubject = new BehaviorSubject<IProduct[]>([]);

  products$ = this.productsSubject.asObservable();

  getProducts(): Observable<IProduct[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.get<IProduct[]>(`${this.apiUrl}/api/products`, {
      headers
    }).pipe(
      tap((products) => {
        this.products = products;
        this.productsSubject.next(products);
      })
    );
  }

  getProductById(productId: string): Observable<IProduct> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.get<IProduct>(`${this.apiUrl}/api/products/${productId}` , {headers});

  }

  deleteProduct(productId: string): Observable<void> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.delete<void>(`${this.apiUrl}/api/products/${productId}`,{headers});
  }

  addProduct(product: IProduct): Observable<IProduct> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.post<IProduct>(`${this.apiUrl}/api/products/add`, product,{headers});
  }

  updateProduct(productId: number, product: IProduct): Observable<IProduct> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    const index = this.products.findIndex(p => p.id === productId);
    if (index !== -1) {
      this.products[index] = product;
      this.productsSubject.next([...this.products]);
    }
    return this.http.put<IProduct>(`${this.apiUrl}/api/products/${productId}`, {
      title: product.title,
      description: product.description,
      price: product.price,
      stock: product.stock,
      categoryName: product.categoryName,
    },{headers});
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
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.get<string[]>(`${this.apiUrl}/api/category/categoryList`,{headers});
  }

  getReviewsofProduct(productId: any): Observable<IReview[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.get<IReview[]>(`${this.apiUrl}/api/products/${productId}/reviews`, {headers});
  }

  addReview(productId: number, review: { rating: number; description: string }): Observable<IReview> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
    return this.http.post<IReview>(`${this.apiUrl}/api/products/${productId}/reviews`, review, { headers });
  }
  deleteReview( reviewId: number, userId: number){
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
    return this.http.delete(`${this.apiUrl}/api/products/reviews/${reviewId}`,{ headers })
  }
  // deleteReview(reviewId: number): Observable<any> {
  //   const token = localStorage.getItem('access_token');
  //   const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  //   const url = `${this.apiUrl}/reviews/${reviewId}`;
  //   console.log('DELETE Request:', { url, token });
  //   return this.http.delete(url, { headers });
  // }




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

  getOrdersByUserId(userId: string): Observable<OrderResponse[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.get<OrderResponse[]>(`${this.apiUrl}/api/orders/${userId}`, {headers});
  }

  //-------------------store the product id and user id in table
  registerNotification(productId : number, userId: number){
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
     return this.http.post(`${this.apiUrl}/api/products/notifyMe`,{productId, userId}, {headers})
  }

  checkNotification(productId: number, userId: number): Observable<boolean> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.get<boolean>(`${this.apiUrl}/api/products/check`, {headers});
  }
  

}
