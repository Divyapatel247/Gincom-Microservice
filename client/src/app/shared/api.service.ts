import { HttpClient, HttpHeaders } from '@angular/common/http';
import { async, firstValueFrom, Observable, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { IProduct } from '../components/product/productModel';
import { Basket, BasketResponse } from '../models/cart.interface';
import { Order, OrderResponse } from '../models/order.interface';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private apiUrl = 'http://localhost:5100';

  constructor(private http: HttpClient) { }

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

  // Get cart for user
  getCart(userId: string): Observable<BasketResponse> {
    // return this.http.get<BasketResponse>(`${this.baseUrl}/orders/${userId}/cart`);
    return new Observable<BasketResponse>(observer => {
      const dummyResponse: BasketResponse = {
        id: 1,
        userId: 'user123',
        items: [
          { id: 1, productId: 1, quantity: 2 },
          { id: 2, productId: 2, quantity: 1 },
        ],
        createdAt: new Date(),
        totalAmount: 29.97, // Matches backend totalAmount
      };
      observer.next(dummyResponse);
      observer.complete();
    });
  }

  updateCartItem(userId: string, itemId: number, quantity: number): Observable<BasketResponse> {
    // return this.http.put<BasketResponse>(`${this.baseUrl}/orders/${userId}/cart/items/${itemId}`, { quantity });
    return new Observable<BasketResponse>(observer => {
      const dummyResponse: BasketResponse = {
        id: 1,
        userId: 'user123',
        items: [
          { id: itemId, productId: 1, quantity: quantity },
          { id: 2, productId: 2, quantity: 1 },
        ],
        createdAt: new Date(),
        totalAmount: quantity * 9.99 + 9.99, // Matches backend price logic
      };
      observer.next(dummyResponse);
      observer.complete();
    });
  }

  removeCartItem(userId: string, itemId: number): Observable<BasketResponse> {
    // return this.http.delete<BasketResponse>(`${this.baseUrl}/orders/${userId}/cart/items/${itemId}`);
    return new Observable<BasketResponse>(observer => {
      const dummyResponse: BasketResponse = {
        id: 1,
        userId: 'user123',
        items: [{ id: 2, productId: 2, quantity: 1 }],
        createdAt: new Date(),
        totalAmount: 9.99,
      };
      observer.next(dummyResponse);
      observer.complete();
    });
  }

  createOrder(userId: string): Observable<OrderResponse> {
    // return this.http.post<OrderResponse>(`${this.baseUrl}/orders/${userId}`, {});
    return new Observable<OrderResponse>(observer => {
      const dummyResponse: OrderResponse = {
        id: 1,
        userId: 'user123',
        status: 'Pending',
        items: [
          { productId: 1, quantity: 2 },
          { productId: 2, quantity: 1 },
        ],
        createdAt: new Date(),
        razorpayOrderId: 'rzp_order_123',
      };
      observer.next(dummyResponse);
      observer.complete();
    });
  }
}
