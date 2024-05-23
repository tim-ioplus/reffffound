import { TestBed } from '@angular/core/testing';
import { FindlingService } from './findling.service';

describe('FindlingService', () => {
    beforeEach(async () => {
      await TestBed.configureTestingModule({
        imports: [FindlingService],
      }).compileComponents();
    });
  
    it('should create the service', () => {
      const fixture = TestBed.createComponent(FindlingService);
      const app = fixture.componentInstance;
      expect(app).toBeTruthy();
    })

    it('should get results', () => {
      const fixture = TestBed.createComponent(FindlingService);
      const service = fixture.componentInstance;
      var results = service.listall();
      expect(results.length > 1).toBeTruthy();
    })

    it('should returns all findlings as json string', () => {
      const fixture = TestBed.createComponent(FindlingService);
      const service = fixture.componentInstance;
      expect(service != null).toBeTruthy();

      var result = service.listall();
      expect(result != null && result.length > 20);
      console.log(result.length);

      var findlingsjson = JSON.stringify(result);
      expect(findlingsjson != null && findlingsjson.length > 1000);
      console.log(findlingsjson);      
    })

    it('should get one result', () => {
      const fixture = TestBed.createComponent(FindlingService);
      const service = fixture.componentInstance;
      var result = service.get('a4xwqsd45-df345643-bnghuz78-4587p4lk');
      expect(result != null).toBeTruthy();
      expect(result?.guid == 'a4xwqsd45-df345643-bnghuz78-4587p4lk').toBeTruthy();
      expect(result?.url == 'https://i.imgur.com/rGXMMih').toBeTruthy();
    })
    
    ;}
);