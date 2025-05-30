import { HttpClient, HttpHeaders } from '@angular/common/http';

import { BehaviorSubject, tap } from 'rxjs';

import { async, firstValueFrom, Observable, of } from 'rxjs';

import { Injectable } from '@angular/core';
import {
  IProduct,
  IProductWithRelatedProducts,
  IReview,
} from '../components/product/productModel';
import { Basket, BasketItem, BasketResponse } from '../models/cart.interface';

import { Order, OrderResponse } from '../models/order.interface';
import { AuthService } from '../service/auth.service';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private apiUrl = 'http://localhost:5100';

  constructor(private http: HttpClient, private authService: AuthService) {}

  private products: IProduct[] = [];
  private productsSubject = new BehaviorSubject<IProduct[]>([]);

  products$ = this.productsSubject.asObservable();


  getProducts(): Observable<IProduct[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http
      .get<IProduct[]>(`${this.apiUrl}/api/products`, {
        headers,
      })
      .pipe(
        tap((products) => {
          this.products = products;
          this.productsSubject.next(products);
        })
      );
  }

  getProductById(productId: string): Observable<IProductWithRelatedProducts> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<IProductWithRelatedProducts>(
      `${this.apiUrl}/api/products/${productId}`,
      { headers }
    );
  }

  deleteProduct(productId: string): Observable<void> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.delete<void>(`${this.apiUrl}/api/products/${productId}`, {
      headers,
    });
  }

  addProduct(product: IProduct): Observable<IProduct> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post<IProduct>(
      `${this.apiUrl}/api/products/add`,
      product,
      { headers }
    );
  }

  updateProduct(productId: number, product: IProduct): Observable<IProduct> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    const index = this.products.findIndex((p) => p.id === productId);
    if (index !== -1) {
      this.products[index] = product;
      this.productsSubject.next([...this.products]);
    }


    return this.http.put<IProduct>(`${this.apiUrl}/api/products/${productId}`, product,{headers});

  }

  login(loginObj: any) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded',
    });

    return this.http.post<any>(`${this.apiUrl}/connect/token`, loginObj, {
      headers,
    });
  }

  register(registerObj: any) {
    return this.http.post<any>(`${this.apiUrl}/auth/register`, registerObj);
  }

  getCustomerCount(): Observable<number> {

    return this.http.get<number>(`${this.apiUrl}/auth/count-users`);

  }

  refreshToken(refreshToken: string) {
    return this.http.post<any>('https://dummyjson.com/auth/refresh', {
      refreshToken,
    });
  }

  getProductCategory(categoryName: string): Observable<IProduct[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<IProduct[]>(
      `${this.apiUrl}/api/products/category/${categoryName}`,
      { headers }
    );
  }
  getCategoryList(): Observable<string[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<string[]>(`${this.apiUrl}/api/category/categoryList`, {
      headers,
    });
  }

  getReviewsofProduct(productId: any): Observable<IReview[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<IReview[]>(
      `${this.apiUrl}/api/products/${productId}/reviews`,
      { headers }
    );
  }

  addReview(
    productId: number,
    review: { rating: number; description: string }
  ): Observable<IReview> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post<IReview>(
      `${this.apiUrl}/api/products/${productId}/reviews`,
      review,
      { headers }
    );
  }
  deleteReview(reviewId: number, userId: number) {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.delete(`${this.apiUrl}/api/products/reviews/${reviewId}`, {
      headers,
    });
  }
  // deleteReview(reviewId: number): Observable<any> {
  //   const token = localStorage.getItem('access_token');
  //   const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  //   const url = `${this.apiUrl}/reviews/${reviewId}`;
  //   console.log('DELETE Request:', { url, token });
  //   return this.http.delete(url, { headers });
  // }

  //-----------searchBar
  searchProducts(query: string): Observable<IProduct[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<IProduct[]>(
      `${this.apiUrl}/api/products/search?query=${query}`,
      { headers }
    );
  }

  //------------------------Cart--------------------------------------------

  getCart(userId: string): Observable<BasketResponse> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<BasketResponse>(
      `${this.apiUrl}/api/orders/${userId}/cart`,
      { headers }
    );
  }

  updateCartItem(
    userId: string,
    itemId: number,
    quantity: number
  ): Observable<BasketResponse> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.put<BasketResponse>(
      `${this.apiUrl}/api/orders/${userId}/cart/items/${itemId}`,
      { quantity },
      { headers }
    );
  }

  removeCartItem(userId: string, itemId: number): Observable<BasketResponse> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.delete<BasketResponse>(
      `${this.apiUrl}/api/orders/${userId}/cart/items/${itemId}`,
      { headers }
    );
  }

  deleteCart(userId: string): Observable<any> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.delete<any>(`${this.apiUrl}/api/orders/${userId}/cart`, {
      headers,
    });
  }

  createOrder(userId: string,userEmail: string): Observable<OrderResponse> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({

       Authorization: `Bearer ${token}`,
       'Content-Type': 'application/json'
     });
    const body = {userEmail};
    return this.http.post<OrderResponse>(`${this.apiUrl}/api/orders/${userId}`, body, {headers});

  }

  getProduct(productId: number): Observable<IProduct> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<IProduct>(`${this.apiUrl}/api/products/${productId}`, {
      headers,
    });
  }

  getOrdersByUserId(userId: string): Observable<OrderResponse[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<OrderResponse[]>(
      `${this.apiUrl}/api/orders/user/${userId}`,
      { headers }
    );
  }
  getOrders(): Observable<OrderResponse[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<OrderResponse[]>(`${this.apiUrl}/api/orders`, {
      headers,
    });
  }

  changeOrderStatus(orderId: number, Status: string): Observable<Order> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.put<Order>(
      `${this.apiUrl}/api/orders/${orderId}/status`,
      { Status },
      { headers }
    );
  }

  //-------------------store the product id and user id in table
  registerNotification(productId: number, userId: number) {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post(
      `${this.apiUrl}/api/products/notifyMe`,
      { productId, userId },
      { headers }
    );
  }

  checkNotification(productId: number, userId: number): Observable<boolean> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<boolean>(`${this.apiUrl}/api/products/check`, {
      headers,
    });
  }

  getNotifiedProductIds(userId: number): Observable<number[]> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<number[]>(
      `${this.apiUrl}/api/products/user-notified-products?userId=${userId}`,
      { headers }
    );
  }

  addToCartBulk(
    userId: string,
    items: BasketItem[]
  ): Observable<BasketResponse> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    const request = { Items: items };
    return this.http.post<BasketResponse>(
      `${this.apiUrl}/api/orders/${userId}/cart/items/bulk`,
      request,
      { headers }
    );
  }

  // getOrdersByProductId(productId:number): Observable<> {
  //   const token = localStorage.getItem('access_token');
  //   const headers = new HttpHeaders({
  //      Authorization: `Bearer ${token}`
  //    });
  //   return this.http.get<>(`${this.apiUrl}/api/orders/product/${productId}`, {headers});
  // }

  getOrderByOrderId(orderId: number): Observable<Order> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<Order>(`${this.apiUrl}/api/orders/${orderId}`, {
      headers,
    });
 
  }
  getLowStockProducts(): Observable<number> {
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders({
       Authorization: `Bearer ${token}`
     });
    return this.http.get<number>(`${this.apiUrl}/api/products/lowStok`,{headers});
  }


}
