import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {LayoutComponent} from '../../shared/components/layout/layout.component';
import {AlbumPageComponent} from "apps/SdHub/src/app/pages/album/album-page/album-page.component";

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      {path: ':shortCode', component: AlbumPageComponent},
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
export class AlbumRoutingModule {
}
