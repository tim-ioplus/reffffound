import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { RouterLink } from '@angular/router';
import { RouterLinkActive } from '@angular/router';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
//
//
import { FeedComponent } from './feed/feed.component';
import { FindlingComponent } from './findling/findling.component';
import { UserActivityComponent } from './useractivity/useractivity.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, RouterOutlet, RouterLink, RouterLinkActive, PageNotFoundComponent, 
    FeedComponent, FindlingComponent, UserActivityComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'reFFFFound!';
  claim = 'Image Bookmarking';
}
