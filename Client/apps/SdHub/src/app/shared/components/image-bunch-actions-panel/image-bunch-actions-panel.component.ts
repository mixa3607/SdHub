import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { IAlbumModel } from "apps/SdHub/src/app/models/autogen/album.models";
import { keyBy } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, combineLatest, forkJoin, startWith } from 'rxjs';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { IImageModel } from '../../../models/autogen/misc.models';

import { MatDialog } from '@angular/material/dialog';
import { ImageApi } from "apps/SdHub/src/app/shared/services/api/image.api";
import { AuthStateService } from '../../../core/services/auth-state.service';
import { ImageSelectionService } from '../../../core/services/image-selection.service';
import { MyAlbumsService } from '../../../core/services/my-albums.service';
import {
  ConfirmDialogComponent,
  ConfirmDialogModel
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AlbumApi } from '../../services/api/album.api';
import { httpErrorResponseHandler } from "apps/SdHub/src/app/shared/http-error-handling/handlers";

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
    ),
    startWith([])
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
    this.imageSelectionService.selectedImages$
  ]).pipe(
    map(([user, displayedImagesByShortToken, selectedImages]) => {
      if (!user || selectedImages.length === 0) {
        return false;
      }

      return selectedImages
        .every((shortImageToken) => {
          const selectedImageInfo = displayedImagesByShortToken[shortImageToken];
          return selectedImageInfo?.owner?.login === user.login
        })
    })
  );

  public hasSelectedImages$ = this.imageSelectionService.hasSelectedImages$;

  constructor(
    private imageApi: ImageApi,
    private toastr: ToastrService,
    private albumApi: AlbumApi,
    private dialog: MatDialog,
    private myAlbumsService: MyAlbumsService,
    private authStateService: AuthStateService,
    private imageSelectionService: ImageSelectionService,
  ) {
  }

  public ngOnDestroy() {
    this.imageSelectionService.clearSelection();
  }

  public cancelSelection() {
    this.imageSelectionService.clearSelection();
  }

  public addSelectedImagesToAlbum(albumShortToken: string) {
    this.albumApi
      .addImages({
        albumShortToken,
        images: this.imageSelectionService.getSelectedImages()
      })
      .subscribe(() => {
        this.imageSelectionService.clearSelection();
        this.toastr.success('Images were added to the album');
      }, err => httpErrorResponseHandler(err, this.toastr));
  }

  public moveSelectedImagesToAnotherAlbum(albumShortToken: string) {
    if (!this.selectedAlbum$.value) {
      return;
    }

    const deleteImagesFromAlbumRequest$ = this.albumApi
      .deleteImages({
        albumShortToken: this.selectedAlbum$.value.shortToken,
        images: this.imageSelectionService.getSelectedImages()
      });

    const addImagesToAlbumRequest$ = this.albumApi
      .addImages({
        albumShortToken,
        images: this.imageSelectionService.getSelectedImages()
      });

    combineLatest([deleteImagesFromAlbumRequest$, addImagesToAlbumRequest$])
      .subscribe(() => {
        this.toastr.success('Images were moved to another album');
        this.needReloadImages.emit();
      }, err => httpErrorResponseHandler(err, this.toastr));
  }

  public deleteSelectedImagesFromCurrentAlbum() {
    if (!this.selectedAlbum$.value) {
      return;
    }
    this.albumApi
      .deleteImages({
        albumShortToken: this.selectedAlbum$.value.shortToken,
        images: this.imageSelectionService.getSelectedImages()
      })
      .subscribe(() => {
        this.toastr.success('Images were deleted from the album');
        this.needReloadImages.emit();
      }, err => httpErrorResponseHandler(err, this.toastr));
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
        map(() => this.imageSelectionService
          .getSelectedImages()
          .map((shortToken) => this.imageApi.delete({
            shortToken: shortToken,
            manageToken: null
          }))
        ),
        switchMap((requests) => forkJoin(requests)),
      )
      .subscribe(() => {
        this.toastr.success('Images were deleted');
        this.needReloadImages.emit();
      }, err => httpErrorResponseHandler(err, this.toastr));
  }
}
