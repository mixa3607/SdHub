import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GeneratedRoutingModule } from 'apps/SdHub/src/app/pages/generated/generated-routing.module';
import { SearchPageComponent } from 'apps/SdHub/src/app/pages/generated/about-page/search-page.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [SearchPageComponent],
  imports: [
    CommonModule,
    SharedModule,
    GeneratedRoutingModule
  ]
})
export class GeneratedModule { }
