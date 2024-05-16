import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Findling } from '../data/findling';
import { FindlingService } from '../services/findling.service';

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

    constructor(private findlingService: FindlingService, private activatedRoute: ActivatedRoute)
    {
      let uriguid = activatedRoute.snapshot.paramMap.get('guid')?.toString();
      console.log(uriguid);
      if(uriguid != undefined)
      {
        this.findling = findlingService.get(uriguid);
        this.usercontext = this.findling?.usercontext;
      }

      console.log(this.findling);
    }
}