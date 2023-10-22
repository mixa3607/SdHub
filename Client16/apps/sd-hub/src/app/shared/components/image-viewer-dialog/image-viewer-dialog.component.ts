import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogConfig, MatDialogRef } from "@angular/material/dialog";
import {
  IGridOptions,
  ImageViewerComponent
} from "apps/sd-hub/src/app/shared/components/image-viewer/image-viewer.component";
import { IImageModel } from "apps/sd-hub/src/app/models/autogen/misc.models";
import { IGridModel } from "apps/sd-hub/src/app/models/autogen/grid.models";
import { filter } from "rxjs";
import { NavigationStart, Router, RouterEvent } from "@angular/router";

export interface IImageViewerDialogData {
  imageInfo?: IImageModel;
  gridInfo?: IGridModel;
}

export interface IImageViewerDialogResult {
}

@Component({
  selector: 'image-viewer-dialog',
  templateUrl: './image-viewer-dialog.component.html',
  styleUrls: ['./image-viewer-dialog.component.scss'],
})
export class ImageViewerDialogComponent implements OnInit {
  @ViewChild('imageViewer', {static: true}) imageViewer?: ImageViewerComponent;

  constructor(public dialogRef: MatDialogRef<ImageViewerDialogComponent, IImageViewerDialogResult>,
              public router: Router,
              @Inject(MAT_DIALOG_DATA) public data: IImageViewerDialogData) {
  }

  ngOnInit(): void {
    this.router.events
      .subscribe(() => {
        this.dialogRef.close();
      });

    if (this.imageViewer == null)
      return;
    if (this.data.imageInfo != null) {
      const opts: IGridOptions = {
        xTiles: 1,
        yTiles: 1,
        tileWidth: this.data.imageInfo.parsedMetadata.width,
        tileHeight: this.data.imageInfo.parsedMetadata.height,
        maxNativeZoom: 18,
        minNativeZoom: 18,
        maxZoom: 19,
        minZoom: 18,

        tilesUrlTemplate: this.data.imageInfo.originalImage.directUrl,
        yLegend: [],
        xLegend: [],
        showLegend: false,
      }
      this.imageViewer.initOptions(opts);
    } else if (this.data.gridInfo != null) {
      const grid = this.data.gridInfo;
      const opts: IGridOptions = {
        xTiles: grid.xTiles,
        yTiles: grid.yTiles,
        tileWidth: grid.gridImages[0].image.parsedMetadata.width,
        tileHeight: grid.gridImages[0].image.parsedMetadata.height,
        maxNativeZoom: grid.maxLayer,
        minNativeZoom: grid.minLayer,
        maxZoom: grid.maxLayer + 1,
        minZoom: grid.minLayer,

        tilesUrlTemplate: grid.layersDirectory.directUrl + '/layers/{z}/{x}_{y}.webp',
        xLegend: grid.xValues.map((x, i) => ({
          name: x,
          background: i % 2 == 0 ? 'rgba(238,238,238,0.9)' : 'rgba(218,218,218,0.9)'
        })),
        yLegend: grid.yValues.map((x, i) => ({
          name: x,
          background: i % 2 == 0 ? 'rgba(238,238,238,0.9)' : 'rgba(218,218,218,0.9)'
        })),
        showLegend: true,
      }
      this.fillGeoJson(grid, opts);
      this.imageViewer.initOptions(opts);
    }
  }

  private fillGeoJson(grid: IGridModel, options: IGridOptions): void {
    options.tiles = grid.gridImages.map(x => x.image).map(x => ({
      name: x.name ?? '',
      description: x.description ?? '',
      shortUrl: x.shortUrl,
      shortCode: x.shortToken,
      props: x.parsedMetadata.tags.map(x => ({name: x.name, value: x.value})),
    }));
  }

  public static open(data: IImageViewerDialogData, dialog: MatDialog) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = true;
    dialogConfig.data = data;
    dialogConfig.maxWidth = '100vw';
    dialogConfig.maxHeight = '100vh';
    dialogConfig.height = 'calc(100% - 2rem)';
    dialogConfig.width = 'calc(100% - 2rem)';
    dialogConfig.panelClass = 'md-fill-space';
    dialogConfig.closeOnNavigation = true;

    return dialog.open<ImageViewerDialogComponent, IImageViewerDialogData, IImageViewerDialogResult>(ImageViewerDialogComponent, dialogConfig);
  }
}
