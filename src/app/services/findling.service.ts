import { Injectable } from '@angular/core';
import { FINDLINGS } from '../data/mock-findlings';
import { Findling } from '../data/findling';

@Injectable({
    providedIn: 'root'
  })
export class FindlingService 
{
    list(): Findling[]{ return FINDLINGS} //param page num, take 10
}