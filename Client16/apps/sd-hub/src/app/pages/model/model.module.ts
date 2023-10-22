import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import { SharedModule } from 'apps/sd-hub/src/app/shared/shared.module';
import { ModelPageComponent } from './model-page/model-page.component';
import { EditModelDialogComponent } from './edit-model-dialog/edit-model-dialog.component';
import { MarkdownModule } from "ngx-markdown";
import { ModelRoutingModule } from "apps/sd-hub/src/app/pages/model/model-routing.module";

@NgModule({
  declarations: [ModelPageComponent, EditModelDialogComponent],
  imports: [CommonModule, ModelRoutingModule, LeafletModule, SharedModule, MarkdownModule],
})
export class ModelModule {}
