import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { A11yModule } from '@angular/cdk/a11y';
import { OverlayModule } from '@angular/cdk/overlay';
import { LeafletModule } from "@asymmetrik/ngx-leaflet";
import { GridViewerComponent } from "apps/SdHub/src/app/shared/components/grid-viewer/grid-viewer.component";
import {
  SmallAlbumCardComponent
} from "apps/SdHub/src/app/shared/components/small-album-card/small-album-card.component";
import {
  SmallImageCardComponent
} from 'apps/SdHub/src/app/shared/components/small-image-card/small-image-card.component';
import { CustomMaterialModule } from '../custom-material/custom-material.module';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { ImageBunchActionsPanelComponent } from './components/image-bunch-actions-panel/image-bunch-actions-panel.component';
import { ImageViewerDialogComponent } from './components/image-viewer-dialog/image-viewer-dialog.component';
import { ImageViewerComponent } from './components/image-viewer/image-viewer.component';
import { LayoutComponent } from './components/layout/layout.component';
import { PaginatorComponent } from './components/paginator/paginator.component';
import { LimitToPipe } from './pipes/limit-to.pipe';
import { LocalDatePipe } from './pipes/local-date.pipe';
import { YesNoPipe } from './pipes/yes-no.pipe';

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
    ],
})
export class SharedModule {
}
