import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  isLoggedIn(): boolean {
    return !!localStorage.getItem('access_token');
  }

  getDecodedToken(): any | null {
    const token = localStorage.getItem('access_token');
    if (!token) return null;

    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch (e) {
      console.error('Failed to decode token', e);
      return null;
    }
  }

  getRole(): string | null {
    const decoded = this.getDecodedToken();
    return decoded?.role ?? null;
  }

  getUserId(): string | null {
    const decoded = this.getDecodedToken();
    return decoded?.sub ?? null;
  }

  getEmail(): string | null {
    const decoded = this.getDecodedToken();
    return decoded?.email ?? null;
  }
}
