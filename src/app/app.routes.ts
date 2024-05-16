import { Routes } from '@angular/router';
import { FeedComponent } from './feed/feed.component';
import { FindlingComponent } from './findling/findling.component';
import { UserFeedComponent} from './user/userfeed.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';

export const routes: Routes = [
    {path: 'feed/:page', component: FeedComponent},
    {path: 'feed', redirectTo: '/feed/1', pathMatch: 'full'},    
    {path: '', redirectTo: '/feed/1', pathMatch: 'full'},
    {path: 'image/:guid', component: FindlingComponent},
    {path: 'user/:name', component: UserFeedComponent},    
];
