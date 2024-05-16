import { Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { Findling } from '../data/findling';
import { FindlingService } from '../services/findling.service';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'userfeed-root',
    standalone: true,
    imports: [RouterOutlet, CommonModule, RouterLink],
    templateUrl: './userfeed.component.html'
    /*styleUrl: './feed.component.css'*/
  })
  //implements OnInit
  export class UserFeedComponent {
      findlings: Findling[] | undefined = [];
      _service : FindlingService;
      _route: ActivatedRoute;
      _username: string | undefined= '' ;
  
      constructor(private findlingService: FindlingService, private activatedRoute: ActivatedRoute) 
      {
        this._username = '';
          this._service = findlingService;
          this._route = activatedRoute;
          this._username = this._route.snapshot.paramMap.get('name')?.toString();
          
          this.findlings = this._service.listByUser(this._username);
          window.scrollTo(0,0);
      }
  }