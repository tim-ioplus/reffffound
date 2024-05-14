import { Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterLink } from '@angular/router';
import { Findling } from '../data/findling';
import { FindlingService } from '../services/findling.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'feed-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterLink],
  templateUrl: './feed.component.html'
  /*styleUrl: './feed.component.css'*/
})
export class FeedComponent implements OnInit{
    findlings: Findling[] = [];
    _service : FindlingService;

    constructor(private findlingService: FindlingService) 
    {
        this._service = findlingService;
    }

    ngOnInit(): void 
    {
        this.findlings = this._service.list();
    }
}
