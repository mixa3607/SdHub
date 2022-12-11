import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { IAlbumModel } from "apps/SdHub/src/app/models/autogen/album.models";
import { keyBy } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, combineLatest, forkJoin } from 'rxjs';
import { filter, map, switchMap } from 'rxjs/operators';
import { IImageModel } from '../../../models/autogen/misc.models';

import { MatDialog } from '@angular/material/dialog';
import { ImageApi } from "apps/SdHub/src/app/shared/services/api/image.api";
import { AuthStateService } from '../../../core/services/auth-state.service';
import { ImageSelectionService } from '../../../core/services/image-selection.service';
import { MyAlbumsService } from '../../../core/services/my-albums.service';
import { ConfirmDialogComponent, ConfirmDialogModel } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AlbumApi } from '../../services/api/album.api';

@Component({
  selector: 'image-bunch-actions-panel',
  templateUrl: './image-bunch-actions-panel.component.html',
  styleUrls: ['./image-bunch-actions-panel.component.scss'],
})
export class ImageBunchActionsPanelComponent implements OnDestroy {
  @Input() set displayedImages(val: IImageModel[]) {
    this.displayedImages$.next(val);
  }

  @Input() set selectedAlbum(val: IAlbumModel | null) {
    this.selectedAlbum$.next(val);
  }

  @Output() needReloadImages = new EventEmitter();

  private displayedImages$ = new BehaviorSubject<IImageModel[]>([]);

  private selectedAlbum$ = new BehaviorSubject<IAlbumModel | null>(null);

  public myAlbums$ = this.myAlbumsService.myAlbums$;

  public hasAlbums$ = this.myAlbumsService.hasAlbums$;

  public myAlbumsWithoutCurrent$ = combineLatest([this.myAlbums$, this.selectedAlbum$]).pipe(
    map(([myAlbums, selectedAlbum]) =>
      myAlbums.filter((album) => album.shortToken !== selectedAlbum?.shortToken)
    )
  );

  private displayedImagesByShortToken$ = this.displayedImages$.pipe(
    map((displayedImages) =>
      keyBy(displayedImages, (img) => img.shortToken)
    )
  );

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

  public areActionsUnavailable$ = this.authStateService.user$.pipe(
    map((user) => !user)
  );

  public canDeleteImages$ = combineLatest([
    this.authStateService.user$,
    this.displayedImagesByShortToken$,
    this.imagesSelectionService.selectedImages$
  ]).pipe(
    map(([user, displayedImagesByShortToken, selectedImages]) => {
      if (!user) {
        return false;
      }

      return selectedImages
        .every((shortImageToken) => {
          const selectedImageInfo = displayedImagesByShortToken[shortImageToken];
          return selectedImageInfo?.owner?.login === user.login
        })
    })
  );

  public hasSelectedImages$ = this.imagesSelectionService.hasSelectedImages$;

  constructor(
    private imageApi: ImageApi,
    private toastr: ToastrService,
    private albumApi: AlbumApi,
    private dialog: MatDialog,
    private myAlbumsService: MyAlbumsService,
    private authStateService: AuthStateService,
    private imagesSelectionService: ImageSelectionService,
  ) {}

  public ngOnDestroy() {
    this.imagesSelectionService.clearSelection();
  }

  public cancelSelection() {
    this.imagesSelectionService.clearSelection();
  }

  public addSelectedImagesToAlbum(albumShortToken: string) {
    this.albumApi
      .addImages({
        albumShortToken,
        images: this.imagesSelectionService.getSelectedImages()
      })
      .subscribe(() => {
        this.imagesSelectionService.clearSelection();
        this.toastr.success('Images were added to the album');
      });
  }

  public moveSelectedImagesToAnotherAlbum(albumShortToken: string) {
    if (!this.selectedAlbum$.value) {
      return;
    }

    const deleteImagesFromAlbumRequest$ = this.albumApi
      .deleteImages({
        albumShortToken: this.selectedAlbum$.value.shortToken,
        images: this.imagesSelectionService.getSelectedImages()
      });

    const addImagesToAlbumRequest$ = this.albumApi
      .addImages({
        albumShortToken,
        images: this.imagesSelectionService.getSelectedImages()
      });

    combineLatest([deleteImagesFromAlbumRequest$, addImagesToAlbumRequest$])
      .subscribe(() => {
        this.imagesSelectionService.clearSelection();
        this.toastr.success('Images were moved to another album');

        this.needReloadImages.emit();
      });
  }

  public deleteSelectedImagesFromCurrentAlbum() {
    if (!this.selectedAlbum$.value) {
      return;
    }
    this.albumApi
      .deleteImages({
        albumShortToken: this.selectedAlbum$.value.shortToken,
        images: this.imagesSelectionService.getSelectedImages()
      })
      .subscribe(() => {
        this.imagesSelectionService.clearSelection();
        this.toastr.success('Images were deleted from the album');

        this.needReloadImages.emit();
      });
  }

  public deleteSelectedImages() {
    this.dialog
      .open<ConfirmDialogComponent, ConfirmDialogModel, boolean>(
        ConfirmDialogComponent, {
        data: {
          title: 'Delete images',
          message: 'Are you sure you want to delete these images?'
        }
      }
      )
      .afterClosed()
      .pipe(
        filter((res) => !!res),
        map(() => this.imagesSelectionService
          .getSelectedImages()
          .map((shortToken) => this.imageApi.delete({
            shortToken: shortToken,
            manageToken: null
          }))
        ),
        switchMap((requests) => forkJoin(requests)),
      )
      .subscribe(() => {
        this.imagesSelectionService.clearSelection();
        this.toastr.success('Images were deleted');

        this.needReloadImages.emit();
      });
  }
}
