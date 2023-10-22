import { Routes } from '@angular/router';
import { LayoutComponent } from 'apps/sd-hub/src/app/shared/components/layout/layout.component';
import { SearchPageComponent } from "./search-page/search-page.component";

export const binsRoutes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: 'model',
        loadChildren: () => import('apps/sd-hub/src/app/pages/model/model.module').then(m => m.ModelModule),
      },
      { path: 'search', component: SearchPageComponent },
      { path: '', redirectTo: 'search', pathMatch: 'prefix' },
    ]
  }
];

