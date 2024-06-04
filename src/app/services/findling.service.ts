import { Injectable } from '@angular/core';
import { FINDLINGS } from '../data/mock-findlings';
import { Findling } from '../data/findling';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';

@Injectable({
    providedIn: 'root'
  })
export class FindlingService 
{
  constructor(private _http: HttpClient) 
  {
  }

    listByUser(username: string | undefined): Findling[] | undefined {
      let results: Findling[] = [];
      
      if(username != undefined)
      {
        results = FINDLINGS.filter(f => f.Usercontext.startsWith(username));
      }
      
      return results;
    }

    listall(): Observable<Findling[]>
    { 
      return this._http.get<Findling[]>("./data/findlings.json"); 
    }
    
    list(page: number, take: number) : Observable<Findling[]>
    { 
      return this.listall()      
    }
    
    get(guid: string)
    {
      let findling: Findling | undefined = FINDLINGS.find((f) => f.Guid == guid);
      return findling;
    }
}