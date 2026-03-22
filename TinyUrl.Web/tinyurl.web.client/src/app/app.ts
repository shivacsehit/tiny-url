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

  constructor(private svc: UrlService) { }

  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.svc.getPublic(this.search).subscribe({
      next: data => {
        this.urls = data;
        this.error = '';      // ← clear error on success
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        // ← only show error if urls is empty
        if (this.urls.length === 0) {
          this.error = 'Failed to load URLs';
        }
      }
    });
  }

  generate() {
    if (!this.longUrl) return;
    this.error = '';
    this.svc.add(this.longUrl, this.isPrivate).subscribe({
      next: res => {
        this.generatedUrl = res.shortUrl;
        this.longUrl = '';
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
}
