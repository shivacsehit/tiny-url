import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { UrlService, TinyUrl } from './url.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent implements OnInit {
  longUrl = '';
  isPrivate = false;
  urls: TinyUrl[] = [];
  search = '';
  generatedUrl = '';
  copied = false;
  error = '';
  loading = false;
  urlError = '';

  constructor(private svc: UrlService) { }

  ngOnInit() { this.load(); }

  validateUrl(url: string): string {
    if (!url || url.trim() === '')
      return 'URL is required';

    let testUrl = url.trim();

    if (!testUrl.startsWith('http://') &&
      !testUrl.startsWith('https://'))
      testUrl = 'https://' + testUrl;

    try {
      const parsed = new URL(testUrl);

      if (!parsed.hostname || parsed.hostname.length < 3)
        return 'Invalid URL — missing domain';

      if (!parsed.hostname.includes('.'))
        return 'Invalid URL — must include domain (e.g. google.com)';

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
    // Validate before submitting
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
      error: () => this.error = 'Failed to generate URL'
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

  getShortUrl(code: string): string {
    return `http://localhost:5000/${code}`;
  }
}
