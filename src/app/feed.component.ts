import { Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Findling } from './findling';
import { FindlingService } from './findling.service';

@Component({
  selector: 'feed-root',
  standalone: true,
  imports: [RouterOutlet],
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
