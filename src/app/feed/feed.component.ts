import { Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
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
//implements OnInit
export class FeedComponent {
    findlings: Findling[] | undefined = [];
    _service : FindlingService;
    _route: ActivatedRoute;
    page: number = 1;
    take: number = 10; 

    constructor(private findlingService: FindlingService, private activatedRoute: ActivatedRoute) 
    {
        this._service = findlingService;
        this._route = activatedRoute;
        let pagenum = this._route.snapshot.paramMap.get('page')?.toString();
        this.NextPage(Number(pagenum));        
    }

    NextPage(npage: number)
    {
        this.page = npage;
        this.findlings = []; 
        this.findlings = this._service.list(this.page, this.take);
        window.scrollTo(0,0);
    }
}
