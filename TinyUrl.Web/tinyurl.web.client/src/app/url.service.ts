import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

export interface TinyUrl {
  id: number;
  shortCode: string;
  originalUrl: string;
  isPrivate: boolean;
  clicks: number;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class UrlService {
  private base = environment.apiUrl;
  constructor(private http: HttpClient) { }

  add(url: string, isPrivate: boolean) {
    return this.http.post<{ shortUrl: string; code: string }>(
      `${this.base}/api/add`, { url, isPrivate });
  }

  getPublic(search = '') {
    return this.http.get<TinyUrl[]>(
      `${this.base}/api/public?search=${search}`);
  }

  delete(code: string) {
    return this.http.delete(`${this.base}/api/delete/${code}`);
  }
}
