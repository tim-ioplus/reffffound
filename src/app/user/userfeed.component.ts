import { Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { Findling } from '../data/findling';
import { FindlingService } from '../services/findling.service';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'userfeed-root',
    standalone: true,
    imports: [RouterOutlet, CommonModule, RouterLink],
    templateUrl: './userfeed.component.html'
    /*styleUrl: './feed.component.css'*/
  })
  //implements OnInit
  export class UserFeedComponent {
      findlings : Findling[];
      _service : FindlingService;
      _route: ActivatedRoute;
      _username: string = '' ;
  
      constructor(private findlingService: FindlingService, private activatedRoute: ActivatedRoute,
        private _http: HttpClient
      ) 
      {
        this.findlings = [];
        this._username = '';
          this._service = findlingService;
          this._route = activatedRoute;
          this._username = this._route.snapshot.paramMap.get('name')!;
          
         this._http.get<Findling[]>('assets/findlings.json')
          .subscribe((data) => {
              this.findlings = data.filter(f => f.Usercontext.startsWith(this._username));
            });
      }
  }