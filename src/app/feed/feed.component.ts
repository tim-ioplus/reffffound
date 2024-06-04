import { Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { Findling } from '../data/findling';
import { FindlingService } from '../services/findling.service';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs/internal/Observable';
import { of } from 'rxjs/internal/observable/of';
import { tap } from 'rxjs/internal/operators/tap';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Component({
  selector: 'feed-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterLink],
  templateUrl: './feed.component.html'
  /*styleUrl: './feed.component.css'*/
})
//implements OnInit
export class FeedComponent {
    findlings : Findling[];
    errorMessage : string = "No Findlings found."
    _service : FindlingService;
    _route: ActivatedRoute;
    start: number = 0;
    end: number = 10;
    take: number = 10; 

    constructor(private findlingService: FindlingService, private activatedRoute: ActivatedRoute,
        private _http: HttpClient
    ) 
    {
        this._service = findlingService;
        this._route = activatedRoute;
        this.start = (Number(this._route.snapshot.paramMap.get('page')?.toString()) -1) * 10;
        this.end = this.start + this.take;
        this.findlings = [];

        this.NextPage(1);
    }

    ngOnInit() 
    {

    }

    NextPage(page: number)
    {
        this._http.get<Findling[]>('assets/findlings.json')
        .subscribe((data) => {
            this.start = Number((page-1) * 10);
            this.end = this.start + this.take;
            this.findlings = data.slice(this.start, this.end);
          });
    }
}
