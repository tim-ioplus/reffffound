import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Findling } from '../data/findling';
import { FindlingService } from '../services/findling.service';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'findling-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: 'findling.html'
})

export class FindlingComponent 
{
    findling: Findling | undefined;
    contextfindlings: Findling[] = []; 
    usercontext: string | undefined = '';
    //contextusers: 

    constructor(private findlingService: FindlingService, private activatedRoute: ActivatedRoute, private _http: HttpClient
    ) 
    {
      let uriguid = activatedRoute.snapshot.paramMap.get('guid')?.toString();
      console.log(uriguid);
      if(uriguid != undefined)
      {
        this._http.get<Findling[]>('assets/findlings.json')
        .subscribe((data) => {
            this.findling = data.find(f => f.Guid==uriguid);
          });
        this.usercontext = this.findling?.Usercontext;
      }

      console.log(this.findling);
    }
}