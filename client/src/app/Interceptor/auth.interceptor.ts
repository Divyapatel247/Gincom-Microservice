import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse,
  HttpClient
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, throwError, of } from 'rxjs';
import { catchError, switchMap, filter, take } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private refreshInProgress = false;
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(private http: HttpClient, private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const accessToken = this.getAccessToken();

    if (this.isTokenExpired(accessToken)) {
      return this.handleRefreshToken(req, next);
    }

    const cloned = this.addTokenHeader(req, accessToken);
    return next.handle(cloned).pipe(
      catchError(err => {
        if (err instanceof HttpErrorResponse && err.status === 401) {
          return this.handleRefreshToken(req, next);
        }
        return throwError(() => err);
      })
    );
  }

  private handleRefreshToken(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.refreshInProgress) {
      this.refreshInProgress = true;
      this.refreshTokenSubject.next(null);

      return this.refreshToken().pipe(
        switchMap((tokens: any) => {
          this.refreshInProgress = false;
          this.setTokens(tokens.access_token, tokens.refresh_token);
          this.refreshTokenSubject.next(tokens.access_token);

          const cloned = this.addTokenHeader(req, tokens.access_token);
          return next.handle(cloned);
        }),
        catchError(err => {
          this.refreshInProgress = false;
          this.logout();
          return throwError(() => err);
        })
      );
    } else {
      return this.refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap(token => {
          const cloned = this.addTokenHeader(req, token!);
          return next.handle(cloned);
        })
      );
    }
  }

  private getAccessToken(): string | null {
    return localStorage.getItem('access_token');
  }

  private getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  private isTokenExpired(token: string | null): boolean {
    if (!token) return true;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp;
      return (Math.floor(new Date().getTime() / 1000)) >= expiry;
    } catch {
      return true;
    }
  }

  private refreshToken(): Observable<any> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) return throwError(() => new Error('No refresh token found'));

    const body = new URLSearchParams();
    body.set('grant_type', 'refresh_token');
    body.set('refresh_token', refreshToken);
    body.set('client_id', 'client'); // Replace with actual
    body.set('client_secret', 'secret'); // Optional, depends on your setup



    return this.http.post('/connect/token', body.toString(), {
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
    });
  }

  private addTokenHeader(req: HttpRequest<any>, token: string | null): HttpRequest<any> {
    if (!token) return req;
    return req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  private setTokens(accessToken: string, refreshToken: string) {
    localStorage.setItem('access_token', accessToken);
    localStorage.setItem('refresh_token', refreshToken);
  }

  private logout() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    this.router.navigate(['/login']);
  }


}
