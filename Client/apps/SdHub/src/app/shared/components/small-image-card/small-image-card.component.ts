import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { IImageModel } from "apps/SdHub/src/app/models/autogen/misc.models";
import { combineLatest } from 'rxjs';
import { filter, map, switchMap } from 'rxjs/operators';
import { AuthStateService } from '../../../core/services/auth-state.service';
import { MyAlbumsService } from '../../../core/services/my-albums.service';
import { AlbumApi } from '../../services/api/album.api';
import { ImageApi } from '../../services/api/image.api';
import { ConfirmDialogComponent, ConfirmDialogModel } from '../confirm-dialog/confirm-dialog.component';

@Component({
    selector: 'small-image-card',
    templateUrl: './small-image-card.component.html',
    styleUrls: ['./small-image-card.component.scss'],
})
export class SmallImageCardComponent implements OnInit {
    get imageInfo(): IImageModel | null {
        return this._imageInfo;
    }

    @Input() set imageInfo(value: IImageModel | null) {
        this._imageInfo = value;
        this.loadImage(value);
    }

    private _imageInfo: IImageModel | null = null;

    public name: string = '';
    public uploadAt: string = '';
    public userName: string = '';
    public software: string = '';
    public thumbUrl: string = '';
    public compressedUrl: string = '';
    public dims: string = '';
    public shortToken: string = '';

    public myAlbums$ = this.myAlbumsService.myAlbums$;

    public hasAlbums$ = this.myAlbumsService.hasAlbums$;

    public canDelete$ = this.authStateService.user$.pipe(
      map((user) => user?.login === this.userName)
    );

    public displayMenuButton$ = combineLatest([this.hasAlbums$, this.canDelete$]).pipe(
      map(([hasAlbums, canDelete]) => hasAlbums || canDelete)
    );

    constructor(
      private dialog: MatDialog,
      private albumApi: AlbumApi,
      private imageApi: ImageApi,
      private authStateService: AuthStateService,
      private myAlbumsService: MyAlbumsService,
    ) { }

    ngOnInit(): void {
    }

    public addImageToAlbum(albumShortToken: string) {
      this.albumApi
        .addImages({ albumShortToken, images: [this.shortToken] })
        .subscribe();
    }

    public deleteImage() {
      this.dialog
        .open<ConfirmDialogComponent, ConfirmDialogModel, boolean>(
          ConfirmDialogComponent, {
            data: {
              title: 'Delete image',
              message: 'Are you sure you want to delete this image?'
            }
          }
        )
        .afterClosed()
        .pipe(
          filter((res) => !!res),
          switchMap(() =>
            this.imageApi.delete({ shortToken: this.shortToken, manageToken: null })
          )
        )
        .subscribe();
    }

    private loadImage(value: IImageModel | null): void {
        this.name = value?.name ?? '';
        this.uploadAt = value?.createdAt ?? '';
        this.userName = value?.owner?.login ?? '';
        this.software = value?.parsedMetadata?.tags?.[0]?.software ?? 'unknown';
        this.thumbUrl = value?.thumbImage?.directUrl ?? '';
        this.compressedUrl = value?.compressedImage?.directUrl ?? '';
        this.dims = value?.parsedMetadata?.width != null && value?.parsedMetadata?.height != null
            ? `${value?.parsedMetadata?.width}x${value?.parsedMetadata?.height}`
            : '';
        this.shortToken = value?.shortToken ?? '';
    }
}