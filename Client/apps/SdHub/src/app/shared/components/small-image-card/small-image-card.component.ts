import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { IImageModel } from "apps/SdHub/src/app/models/autogen/misc.models";
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, combineLatest, Observable } from 'rxjs';
import { filter, map, switchMap } from 'rxjs/operators';
import { AuthStateService } from '../../../core/services/auth-state.service';
import { ImageSelectionService } from '../../../core/services/image-selection.service';
import { MyAlbumsService } from '../../../core/services/my-albums.service';
import { IAlbumModel } from '../../../models/autogen/album.models';
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

    @Input() set selectedAlbum(val: IAlbumModel | null) {
      this.selectedAlbum$.next(val);
    }

    @Output() needReloadImages = new EventEmitter();

    private _imageInfo: IImageModel | null = null;

    public selectedAlbum$ = new BehaviorSubject<IAlbumModel | null>(null);

    public name: string = '';
    public uploadAt: string = '';
    public userName: string = '';
    public software: string = '';
    public thumbUrl: string = '';
    public compressedUrl: string = '';
    public dims: string = '';
    public shortToken: string = '';

    public isMenuOpened: boolean = false;

    public myAlbums$ = this.myAlbumsService.myAlbums$;

    public hasAlbums$ = this.myAlbumsService.hasAlbums$;

    public myAlbumsWithoutCurrent$ = combineLatest([this.myAlbums$, this.selectedAlbum$]).pipe(
      map(([myAlbums, selectedAlbum]) =>
        myAlbums.filter((album) => album.shortToken !== selectedAlbum?.shortToken)
      )
    );

    public canDeleteImage$ = this.authStateService.user$.pipe(
      map((user) => user?.login === this.userName)
    );

    public displayMenuButton$ = combineLatest([this.hasAlbums$, this.canDeleteImage$]).pipe(
      map(([hasAlbums, canDelete]) => hasAlbums || canDelete)
    );

    public isSelected$: Observable<boolean> = null!;

    public isSelectionModeActivated$ = this.imageSelectionService.hasSelectedImages$;

    public isOwnAlbumOpened$ = combineLatest([
      this.authStateService.user$,
      this.selectedAlbum$
    ]).pipe(
      map(([user, selectedAlbum]) => {
        if (!user || !selectedAlbum) {
          return false;
        }

        return selectedAlbum.owner.login === user.login;
      })
    );

    constructor(
      private dialog: MatDialog,
      private albumApi: AlbumApi,
      private imageApi: ImageApi,
      private toastr: ToastrService,
      private authStateService: AuthStateService,
      private myAlbumsService: MyAlbumsService,
      private imageSelectionService: ImageSelectionService,
    ) { }

    ngOnInit(): void {
      this.isSelected$ = this.imageSelectionService.isSelected(this.shortToken);
    }

    public addImageToAlbum(albumShortToken: string) {
      this.albumApi
        .addImages({ albumShortToken, images: [this.shortToken] })
        .subscribe(() => this.toastr.success('Image was added to the album'));
    }

    public moveImageToAnotherAlbum(albumShortToken: string) {
      if (!this.selectedAlbum$.value) {
        return;
      }

      const deleteImageFromAlbumRequest$ = this.albumApi
        .deleteImages({
          albumShortToken: this.selectedAlbum$.value.shortToken,
          images: [this.shortToken]
        });

      const addImageToAlbumRequest$ = this.albumApi
        .addImages({
          albumShortToken,
          images: [this.shortToken]
        });

      combineLatest([deleteImageFromAlbumRequest$, addImageToAlbumRequest$])
        .subscribe(() => {
          this.toastr.success('Image was moved to another album');
          this.needReloadImages.emit();
        });
    }

    public deleteImageFromCurrentAlbum() {
      if (!this.selectedAlbum$.value) {
        return;
      }

      this.albumApi
        .deleteImages({
          albumShortToken: this.selectedAlbum$.value.shortToken,
          images: [this.shortToken]
        })
        .subscribe(() => {
          this.toastr.success('Image was deleted from the album');
          this.needReloadImages.emit();
        });
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
        .subscribe(() => {
          this.toastr.success('Image deleted');
          this.needReloadImages.emit();
        });
    }

    public toggleSelection() {
      this.imageSelectionService.toggleSelected(this.shortToken);
    }

    public onMenuOpened() {
      this.isMenuOpened = true;
    }

    public onMenuClosed() {
      this.isMenuOpened = false;
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
