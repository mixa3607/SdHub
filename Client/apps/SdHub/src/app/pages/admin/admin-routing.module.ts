import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LayoutComponent } from '../../shared/components/layout/layout.component';
import { AdminUsersComponent } from "apps/SdHub/src/app/pages/admin/admin-users/admin-users.component";
import { AdminFilesComponent } from "apps/SdHub/src/app/pages/admin/admin-files/admin-files.component";
import { AdminModelsComponent } from "apps/SdHub/src/app/pages/admin/admin-models/admin-models.component";

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      {path: 'users', component: AdminUsersComponent},
      {path: 'files', component: AdminFilesComponent},
      {path: 'models', component: AdminModelsComponent},
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
export class AdminRoutingModule {
}
