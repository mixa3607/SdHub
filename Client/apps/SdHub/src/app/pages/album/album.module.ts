import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AlbumRoutingModule } from 'apps/SdHub/src/app/pages/album/album-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { AlbumPageComponent } from './album-page/album-page.component';
import {MarkdownModule} from "ngx-markdown";

@NgModule({
  declarations: [AlbumPageComponent],
  imports: [CommonModule, SharedModule, AlbumRoutingModule, MarkdownModule],
})
export class AlbumModule {}
