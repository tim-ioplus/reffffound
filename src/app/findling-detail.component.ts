import { Component, Input } from '@angular/core';
import { Findling } from './findling';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'findling-detail',
  standalone: true,
  templateUrl: 'findling-detail.html',
  imports: [CommonModule]
})

export class FindlingDetailComponent 
{
    @Input() findling!: Findling;
    contextfindlings: Findling[] = []; 
    //contextusers: 
}