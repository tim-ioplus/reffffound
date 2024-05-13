import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FeedComponent } from './feed.component';
import { FindlingDetailComponent } from './findling-detail.component';
import { FindlingService } from './findling.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, FeedComponent, FindlingDetailComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'reFFFFound!';
  claim = 'Image Bookmarking';
}
