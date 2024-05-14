import { Routes } from '@angular/router';
import { FeedComponent } from './feed/feed.component';
import { FindlingComponent } from './findling/findling.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';

export const routes: Routes = [
    {path: 'feed', component: FeedComponent},
    {path: '', redirectTo: '/feed', pathMatch: 'full'},
    {path: 'image/:guid', component: FindlingComponent}    
];
