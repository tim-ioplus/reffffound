import { Injectable } from '@angular/core';
import { FINDLINGS } from '../data/mock-findlings';
import { Findling } from '../data/findling';

@Injectable({
    providedIn: 'root'
  })
export class FindlingService 
{
    listByUser(username: string | undefined): Findling[] | undefined {
      let results: Findling[] = [];
      
      if(username != undefined)
      {
        results = FINDLINGS.filter(f => f.usercontext.startsWith(username));
      }
      
      return results;
    }

    listall(): Findling[]{ return FINDLINGS }
    
    list(page: number, take: number): Findling[] | undefined
    {
      let results: Findling[] = [];
      let startIndex: number = (page - 1) * take;
      let endIndex:number = startIndex + take;

      for (let index = startIndex; index < endIndex; index++)
      {
        let element: Findling = FINDLINGS[index];
        if(element != null)
        {
          results.push(element);
        }
      }
      
      return results;
    }
    


    get(guid: string)
    {
      let findling: Findling | undefined = FINDLINGS.find((f) => f.guid == guid);

      return findling;
    }
}