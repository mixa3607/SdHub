import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GridRoutingModule } from 'apps/SdHub/src/app/pages/grid/grid-routing.module';
import { GridPageComponent } from './grid-page/grid-page.component';
import {LeafletModule} from "@asymmetrik/ngx-leaflet";
import {SharedModule} from "apps/SdHub/src/app/shared/shared.module";

@NgModule({
  declarations: [GridPageComponent],
  imports: [CommonModule, GridRoutingModule, LeafletModule, SharedModule],
})
export class GridModule {}
