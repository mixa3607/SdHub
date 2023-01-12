import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BinsRoutingModule } from 'apps/SdHub/src/app/pages/bins/bins-routing.module';
import { SharedModule } from 'apps/SdHub/src/app/shared/shared.module';
import { MatChipsModule } from '@angular/material/chips';
import { SearchPageComponent } from 'apps/SdHub/src/app/pages/bins/search-page/search-page.component';
import { SearchInModelsComponent } from './search-in-models/search-in-models.component';
import { OverlayModule } from "@angular/cdk/overlay";

@NgModule({
  declarations: [SearchPageComponent, SearchInModelsComponent],
  imports: [CommonModule, SharedModule, BinsRoutingModule, MatChipsModule, OverlayModule],
})
export class BinsModule {}
