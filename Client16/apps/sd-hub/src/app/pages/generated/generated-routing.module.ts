import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LayoutComponent } from '../../shared/components/layout/layout.component';
import { SearchPageComponent } from 'apps/sd-hub/src/app/pages/generated/search-page/search-page.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: 'search', component: SearchPageComponent },
      { path: '', redirectTo: 'search', pathMatch: 'prefix' },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GeneratedRoutingModule { }
