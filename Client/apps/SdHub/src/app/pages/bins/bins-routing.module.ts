import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LayoutComponent } from 'apps/SdHub/src/app/shared/components/layout/layout.component';
import { SearchPageComponent } from "./search-page/search-page.component";

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: 'model',
        loadChildren: () => import('apps/SdHub/src/app/pages/model/model.module').then(m => m.ModelModule),
      },
      { path: 'search', component: SearchPageComponent },
      { path: '', redirectTo: 'search', pathMatch: 'prefix' },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BinsRoutingModule { }
