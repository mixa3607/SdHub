import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';

const appRoutes: Routes = [
  {
    path: 'i',
    loadChildren: () => import('./pages/image/image.module').then(m => m.ImageModule),
  },
  {
    path: 'image',
    loadChildren: () => import('./pages/image/image.module').then(m => m.ImageModule),
  },
  {
    path: 'a',
    loadChildren: () => import('./pages/album/album.module').then(m => m.AlbumModule),
  },
  {
    path: 'album',
    loadChildren: () => import('./pages/album/album.module').then(m => m.AlbumModule),
  },
  {
    path: 'g',
    loadChildren: () => import('./pages/grid/grid.module').then(m => m.GridModule),
  },
  {
    path: 'grid',
    loadChildren: () => import('./pages/grid/grid.module').then(m => m.GridModule),
  },
  {
    path: 'gen',
    loadChildren: () => import('./pages/generated/generated.module').then(m => m.GeneratedModule),
  },
  {
    path: 'about',
    loadChildren: () => import('./pages/about/about.module').then(m => m.AboutModule),
  },
  {
    path: 'auth',
    loadChildren: () => import('./pages/auth/auth.module').then(m => m.AuthModule),
  },
  {
    path: 'user',
    loadChildren: () => import('./pages/user/user.module').then(m => m.UserModule),
  },
  {
    path: 'upload',
    loadChildren: () => import('./pages/upload/upload.module').then(m => m.UploadModule),
  },
  {
    path: '**',
    redirectTo: 'gen',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes)
  ],
  exports: [RouterModule],
  providers: []
})
export class AppRoutingModule {
}
