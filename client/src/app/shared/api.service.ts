import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Injectable } from '@angular/core';
import { IProduct } from '../components/product/productModel';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private apiUrl = 'http://localhost:5100';

  constructor(private http: HttpClient) {}

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
    return this.http.get<IProduct>(`${this.apiUrl}/api/products/${productId}`,{headers});
  }

  deleteProduct(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/products/${productId}`);
  }

  addProduct(product: IProduct): Observable<IProduct> {
    return this.http.post<IProduct>(`${this.apiUrl}/api/products/add`, product);
  }

  updateProduct(productId: number, product: IProduct): Observable<IProduct> {
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
}
