import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {SharedModule} from 'apps/sd-hub/src/app/shared/shared.module';
import {MatChipsModule} from '@angular/material/chips';
import {SearchPageComponent} from 'apps/sd-hub/src/app/pages/bins/search-page/search-page.component';
import {SearchInModelsComponent} from './search-in-models/search-in-models.component';
import {OverlayModule} from "@angular/cdk/overlay";
import {RouterModule} from "@angular/router";
import {binsRoutes} from "./bins.routes";

@NgModule({
  declarations: [
    SearchPageComponent,
    SearchInModelsComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild(binsRoutes),
    MatChipsModule,
    OverlayModule
  ],
})
export class BinsModule {
}
