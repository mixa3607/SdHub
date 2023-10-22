import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImagePageComponent } from './image-page/image-page.component';
import { SharedModule } from '../../shared/shared.module';
import { ImageRoutingModule } from './image-routing.module';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { ManageTokenInputModalComponent } from './manage-token-input-modal/manage-token-input-modal.component';

@NgModule({
  declarations: [ImagePageComponent, ManageTokenInputModalComponent],
  imports: [CommonModule, SharedModule, ImageRoutingModule, ClipboardModule],
})
export class ImageModule {}
