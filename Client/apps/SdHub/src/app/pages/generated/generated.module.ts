import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GeneratedRoutingModule } from 'apps/SdHub/src/app/pages/generated/generated-routing.module';
import { SearchPageComponent } from 'apps/SdHub/src/app/pages/generated/search-page/search-page.component';
import { SharedModule } from '../../shared/shared.module';
import { SearchInImagesComponent } from './search-in-images/search-in-images.component';
import {MatChipsModule} from "@angular/material/chips";

@NgModule({
  declarations: [SearchPageComponent, SearchInImagesComponent],
  imports: [CommonModule, SharedModule, GeneratedRoutingModule, MatChipsModule],
})
export class GeneratedModule {}
