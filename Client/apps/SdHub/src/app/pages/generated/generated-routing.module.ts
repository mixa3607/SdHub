import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LayoutComponent } from '../../shared/layout/layout.component';
import { SearchPageComponent } from 'apps/SdHub/src/app/pages/generated/search-page/search-page.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', redirectTo: 'search', pathMatch: 'prefix' },
      { path: 'search', component: SearchPageComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GeneratedRoutingModule { }
