import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserRoutingModule } from 'apps/sd-hub/src/app/pages/user/user-routing.module';
import { UserPageComponent } from 'apps/sd-hub/src/app/pages/user/user-page/user-page.component';
import { SharedModule } from '../../shared/shared.module';
import { MarkdownModule } from 'ngx-markdown';
import { UserImagesComponent } from './user-images/user-images.component';
import { UserAlbumsComponent } from 'apps/sd-hub/src/app/pages/user/user-albums/user-albums.component';
import { AddAlbumDialogComponent } from './add-album-dialog/add-album-dialog.component';

@NgModule({
  declarations: [
    UserPageComponent,
    UserImagesComponent,
    UserAlbumsComponent,
    AddAlbumDialogComponent,
  ],
  imports: [CommonModule, SharedModule, UserRoutingModule, MarkdownModule],
})
export class UserModule {}
