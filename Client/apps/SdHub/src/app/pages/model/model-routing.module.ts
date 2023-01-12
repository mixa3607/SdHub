import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LayoutComponent } from '../../shared/components/layout/layout.component';
import { ModelPageComponent } from "apps/SdHub/src/app/pages/model/model-page/model-page.component";

const routes: Routes = [
  {path: ':shortCode', component: ModelPageComponent},
  {
    path: '**',
    redirectTo: '/',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ModelRoutingModule {
}
