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