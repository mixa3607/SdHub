import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LimitToPipe } from './pipes/limit-to.pipe';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { LocalDatePipe } from './pipes/local-date.pipe';
import { YesNoPipe } from './pipes/yes-no.pipe';
import { LayoutComponent } from './components/layout/layout.component';
import { CustomMaterialModule } from '../custom-material/custom-material.module';
import { OverlayModule } from '@angular/cdk/overlay';
import { A11yModule } from '@angular/cdk/a11y';
import { PaginatorComponent } from './components/paginator/paginator.component';
import {
  SmallImageCardComponent
} from 'apps/sd-hub/src/app/shared/components/small-image-card/small-image-card.component';
import { ImageViewerComponent } from './components/image-viewer/image-viewer.component';
import { ImageViewerDialogComponent } from './components/image-viewer-dialog/image-viewer-dialog.component';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import {
  SmallAlbumCardComponent
} from 'apps/sd-hub/src/app/shared/components/small-album-card/small-album-card.component';
import { AlbumAutocompleteComponent } from './components/album-autocomplete/album-autocomplete.component';
import {
  ImageBunchActionsPanelComponent
} from "apps/sd-hub/src/app/shared/components/image-bunch-actions-panel/image-bunch-actions-panel.component";
import { SmallGridCardComponent } from "apps/sd-hub/src/app/shared/components/small-grid-card/small-grid-card.component";
import {
  MapImagePopupComponent
} from "apps/sd-hub/src/app/shared/components/image-viewer/map-image-popup/map-image-popup.component";

@NgModule({
  imports: [
    RouterModule,
    CustomMaterialModule,
    FormsModule,
    ReactiveFormsModule,
    OverlayModule,
    A11yModule,
    LeafletModule,
  ],
  declarations: [
    ConfirmDialogComponent,
    SmallImageCardComponent,
    SmallAlbumCardComponent,
    SmallGridCardComponent,
    LimitToPipe,
    LocalDatePipe,
    YesNoPipe,
    LayoutComponent,
    PaginatorComponent,
    ImageViewerComponent,
    ImageViewerDialogComponent,
    AlbumAutocompleteComponent,
    ImageBunchActionsPanelComponent,
    ImageViewerDialogComponent,
    MapImagePopupComponent,
  ],
  exports: [
    FormsModule,
    ReactiveFormsModule,
    CustomMaterialModule,
    LimitToPipe,
    ConfirmDialogComponent,
    SmallImageCardComponent,
    SmallAlbumCardComponent,
    SmallGridCardComponent,
    LocalDatePipe,
    YesNoPipe,
    ImageBunchActionsPanelComponent,
    PaginatorComponent,
    AlbumAutocompleteComponent,
  ],
})
export class SharedModule {
}
