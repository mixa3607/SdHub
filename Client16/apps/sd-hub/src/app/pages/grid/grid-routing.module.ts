import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LayoutComponent } from '../../shared/components/layout/layout.component';
import { GridPageComponent } from "apps/sd-hub/src/app/pages/grid/grid-page/grid-page.component";

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [{path: ':shortCode', component: GridPageComponent},
      {
        path: '**',
        redirectTo: '/',
        pathMatch: 'full'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GridRoutingModule {
}
