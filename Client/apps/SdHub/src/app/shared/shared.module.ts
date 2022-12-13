import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@angular/flex-layout';
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
import { SmallImageCardComponent } from 'apps/SdHub/src/app/shared/components/small-image-card/small-image-card.component';
import { ImageViewerComponent } from './components/image-viewer/image-viewer.component';
import { ImageViewerDialogComponent } from './components/image-viewer-dialog/image-viewer-dialog.component';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import { GridViewerComponent } from 'apps/SdHub/src/app/shared/components/grid-viewer/grid-viewer.component';
import { SmallAlbumCardComponent } from 'apps/SdHub/src/app/shared/components/small-album-card/small-album-card.component';
import { AlbumAutocompleteComponent } from './components/album-autocomplete/album-autocomplete.component';

@NgModule({
  imports: [
    RouterModule,
    CustomMaterialModule,
    FormsModule,
    ReactiveFormsModule,
    FlexLayoutModule,
    OverlayModule,
    A11yModule,
    LeafletModule,
  ],
  declarations: [
    ConfirmDialogComponent,
    SmallImageCardComponent,
    SmallAlbumCardComponent,
    LimitToPipe,
    LocalDatePipe,
    YesNoPipe,
    LayoutComponent,
    PaginatorComponent,
    ImageViewerComponent,
    GridViewerComponent,
    ImageViewerDialogComponent,
    AlbumAutocompleteComponent,
        ImageBunchActionsPanelComponent,
        ImageViewerDialogComponent,
  ],
    exports: [
        FormsModule,
        ReactiveFormsModule,
        FlexLayoutModule,
        CustomMaterialModule,
        LimitToPipe,
        ConfirmDialogComponent,
        SmallImageCardComponent,
        SmallAlbumCardComponent,
        LocalDatePipe,
        YesNoPipe,
        GridViewerComponent,
        ImageBunchActionsPanelComponent,
        PaginatorComponent,
        AlbumAutocompleteComponent,
    ],
})
export class SharedModule {}
