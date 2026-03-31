import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {

  private tokenKey = 'tiny_url_token';
  private base = environment.apiUrl;

  constructor(private http: HttpClient) { }

  autoLogin(): Promise<void> {
    return this.http.post<any>(
      `${this.base}/api/auth/login`,
      { username: 'admin', password: 'Admin@123' }
    ).toPromise()
      .then(res => {
        if (res?.token) {
          localStorage.setItem(this.tokenKey, res.token);
          console.log('✅ Token saved:', res.token.substring(0, 20) + '...');
        }
      })
      .catch(err => {
        console.error('❌ Login failed:', err);
      });
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
}
