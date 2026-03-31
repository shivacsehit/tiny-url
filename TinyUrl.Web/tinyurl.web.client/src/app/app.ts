import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UrlService, TinyUrl } from './url.service';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {

  longUrl = '';
  isPrivate = false;
  urls: TinyUrl[] = [];
  search = '';
  generatedUrl = '';
  copied = false;
  error = '';
  loading = false;
  urlError = '';

  constructor(
    private svc: UrlService,
    private auth: AuthService
  ) {
    this.auth.autoLogin().then(() => {
      this.load();
    });
  }

  validateUrl(url: string): string {
    if (!url || url.trim() === '') return '';
    let testUrl = url.trim();
    if (!testUrl.startsWith('http://') &&
      !testUrl.startsWith('https://'))
      testUrl = 'https://' + testUrl;
    try {
      const parsed = new URL(testUrl);
      if (!parsed.hostname.includes('.'))
        return 'Invalid URL — must include domain';
      if (url.includes(' '))
        return 'URL cannot contain spaces';
      return '';
    } catch {
      return 'Invalid URL — please enter a valid web address';
    }
  }

  onUrlInput() {
    this.urlError = this.validateUrl(this.longUrl);
  }

  load() {
    this.loading = true;
    this.svc.getPublic(this.search).subscribe({
      next: data => {
        this.urls = data;
        this.error = '';
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        if (this.urls.length === 0)
          this.error = 'Failed to load URLs';
      }
    });
  }

  generate() {
    if (!this.longUrl || this.longUrl.trim() === '') {
      this.urlError = 'URL is required';
      return;
    }
    this.urlError = this.validateUrl(this.longUrl);
    if (this.urlError) return;

    this.error = '';
    this.svc.add(this.longUrl, this.isPrivate).subscribe({
      next: res => {
        this.generatedUrl = res.shortUrl;
        this.longUrl = '';
        this.urlError = '';
        this.load();
      },
      error: err => {
        this.error = `Failed to generate URL: ${err.status}`;
      }
    });
  }

  delete(code: string) {
    this.svc.delete(code).subscribe({
      next: () => this.load(),
      error: () => this.error = 'Failed to delete URL'
    });
  }

  copy(url: string) {
    navigator.clipboard.writeText(url);
    this.copied = true;
    setTimeout(() => this.copied = false, 2000);
  }
}
