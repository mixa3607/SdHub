import {Component, ElementRef, Inject, OnInit, ViewChild} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogConfig, MatDialogRef} from "@angular/material/dialog";
import {
    IGridOptions,
    ImageViewerComponent
} from "apps/SdHub/src/app/shared/components/image-viewer/image-viewer.component";
import {IImageModel} from "apps/SdHub/src/app/models/autogen/misc.models";

export interface IImageViewerDialogData {
    imageInfo: IImageModel;
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
                @Inject(MAT_DIALOG_DATA) public data: IImageViewerDialogData) {
    }

    ngOnInit(): void {
        if (this.imageViewer == null)
            return;
        /*const opts: IGridOptions = {
            xTiles: 11,
            yTiles: 11,
            tileWidth: 512,
            tileHeight: 896,
            maxNativeZoom: 18,
            minNativeZoom: 11,
            tilesUrlTemplate: '/layers/{z}/{x}_{y}.webp',
            yLegend: [],
            xLegend: [],
            showLegend: true,
        }
        const xPrompts = [
            'sitting',
            'standing',
            'squatting',
            'rabbit pose',
            'bent over',
            'leaning forward',
            'paw pose',
            'claw pose',
            'crossed legs',
            'ojou-sama pose',
            'fighting stance',
            'legs apart',
            'arms up',
            'arms behind back',
            'arms behind head',
            'hands on hips',
            'hands up',
            'all fours',
            'hands between legs',
            'covering mouth',
            'outstretched arms',
            'top-down bottom-up',
            'on back',
            'on stomach',
            'lying',
            'on side',
            'standing on one leg',
        ]
        const yPrompts = Array.from({length: 151 - 70}, (v, k) => k + 70).map(x => x.toString());
        opts.xTiles = xPrompts.length;
        opts.yTiles = yPrompts.length;

        for (let i = 0; i < opts.xTiles; i++) {
            const background = i % 2 == 0 ? 'rgba(238,238,238,0.9)' : 'rgba(218,218,218,0.9)';
            opts.xLegend.push({name: xPrompts[i], background});
        }
        for (let i = 0; i < opts.yTiles; i++) {
            const background = i % 2 == 0 ? 'rgba(238,238,238,0.9)' : 'rgba(218,218,218,0.9)';
            opts.yLegend.push({name: yPrompts[i], background});
        }*/
        const opts: IGridOptions = {
            xTiles: 1,
            yTiles: 1,
            tileWidth: this.data.imageInfo.parsedMetadata.width,
            tileHeight: this.data.imageInfo.parsedMetadata.height,
            maxNativeZoom: 18,
            minNativeZoom: 18,
            tilesUrlTemplate: this.data.imageInfo.originalImage.directUrl,
            yLegend: [],
            xLegend: [],
            showLegend: false,
        }
        this.imageViewer.initOptions(opts);
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

        return dialog.open<ImageViewerDialogComponent, IImageViewerDialogData, IImageViewerDialogResult>(ImageViewerDialogComponent, dialogConfig);
    }
}
