import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GeneratedRoutingModule } from 'apps/sd-hub/src/app/pages/generated/generated-routing.module';
import { SearchPageComponent } from 'apps/sd-hub/src/app/pages/generated/search-page/search-page.component';
import { SharedModule } from '../../shared/shared.module';
import { SearchInImagesComponent } from './search-in-images/search-in-images.component';
import { MatChipsModule } from '@angular/material/chips';
import {
  SearchInAlbumsComponent
} from "apps/sd-hub/src/app/pages/generated/search-in-albums/search-in-albums.component";
import { SearchInGridsComponent } from "apps/sd-hub/src/app/pages/generated/search-in-grids/search-in-grids.component";

@NgModule({
  declarations: [
    SearchPageComponent,
    SearchInImagesComponent,
    SearchInAlbumsComponent,
    SearchInGridsComponent,
  ],
  imports: [CommonModule, SharedModule, GeneratedRoutingModule, MatChipsModule],
})
export class GeneratedModule {
}
