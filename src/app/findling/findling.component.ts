import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
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
    findling: Findling;
    contextfindlings: Findling[] = []; 
    //contextusers: 

    constructor(private findlingService: FindlingService)
    {
      var idx = Math.round(Math.random());
      console.log(idx);
      this.findling = findlingService.list()[idx];
      console.log(this.findling);
    }
}