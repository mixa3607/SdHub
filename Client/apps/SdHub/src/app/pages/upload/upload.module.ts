import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';
import { UploadRoutingModule } from './upload-routing.module';
import { UploadPageComponent } from './upload-page/upload-page.component';
import { NgxFileDropModule } from 'ngx-file-drop';
import { UploadGridComponent } from './upload-grid/upload-grid.component';
import { UploadImageComponent } from './upload-image/upload-image.component';

@NgModule({
  declarations: [
    UploadPageComponent,
    UploadGridComponent,
    UploadImageComponent,
  ],
  imports: [CommonModule, SharedModule, UploadRoutingModule, NgxFileDropModule],
})
export class UploadModule {}
