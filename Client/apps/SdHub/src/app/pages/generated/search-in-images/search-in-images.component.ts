import { HttpErrorResponse } from "@angular/common/http";
import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import {
  ISearchImageResponse,
  SearchImageInFieldType,
  SearchImageOrderByFieldType,
  SearchImageOrderByType,
  SoftwareGeneratedTypes
} from "apps/SdHub/src/app/models/autogen/image.models";
import { PerformType } from "apps/SdHub/src/app/pages/generated/search-page/search-page.component";
import { httpErrorResponseHandler } from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import { ImageApi } from "apps/SdHub/src/app/shared/services/api/image.api";
import { Dictionary, keyBy } from 'lodash';
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, combineLatest, filter, forkJoin, map, Observable } from "rxjs";
import { switchMap } from "rxjs/operators";
import { AuthStateService } from '../../../core/services/auth-state.service';
import { ImageSelectionService } from '../../../core/services/image-selection.service';
import { MyAlbumsService } from '../../../core/services/my-albums.service';
import { IImageModel } from '../../../models/autogen/misc.models';
import { ConfirmDialogComponent, ConfirmDialogModel } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AlbumApi } from '../../../shared/services/api/album.api';

@UntilDestroy()
@Component({
    selector: 'search-in-images',
    templateUrl: './search-in-images.component.html',
    styleUrls: ['./search-in-images.component.scss'],
})
export class SearchInImagesComponent implements OnInit, OnDestroy {
    public readonly orderBy: { name: string, field: SearchImageOrderByFieldType, order: SearchImageOrderByType | null, icon: string | null }[] = [
        {
            name: 'Upload date',
            field: SearchImageOrderByFieldType.UploadDate,
            order: SearchImageOrderByType.Asc,
            icon: 'arrow_downward',
        },
        {
            name: 'User name',
            field: SearchImageOrderByFieldType.UserName,
            order: null,
            icon: null,
        },
    ];
    public readonly softwareCheckboxes: { id: string, value: string, checked: boolean }[] = [
        {id: SoftwareGeneratedTypes.AutomaticWebUi, value: 'AutomaticWebUi', checked: true},
        {id: SoftwareGeneratedTypes.DreamStudio, value: 'DreamStudio', checked: true},
        {id: SoftwareGeneratedTypes.NovelAi, value: 'NovelAi', checked: true},
        {id: SoftwareGeneratedTypes.Unknown, value: '-----', checked: false},
    ];
    public readonly searchFieldsCheckboxes: { id: SearchImageInFieldType, value: string, checked: boolean }[] = [
        {id: SearchImageInFieldType.Prompt, value: 'Prompt', checked: true},
        {id: SearchImageInFieldType.Name, value: 'Name', checked: true},
        {id: SearchImageInFieldType.Description, value: 'Description', checked: true},
        {id: SearchImageInFieldType.User, value: 'User name', checked: false},
    ];

    // region search text
    private _searchText = '';
    get searchText(): string {
        return this._searchText;
    }

    @Input() set searchText(value: string) {
        this._searchText = value;
    }

    // endregion

    // region search btn
    @Input() set searchButtonClick(value: Observable<unknown> | null) {
        value?.pipe(untilDestroyed(this)).subscribe(x => this.runSearch(PerformType.Search));
    }

    // endregion

    @Output() searchTextChange = new EventEmitter<string>();
    @ViewChild('scrollTo', {read: ElementRef}) scrollTo?: ElementRef;
    public alsoFromGrids = false;
    public onlyFromRegisteredUsers = true;
    public searchAsRegexp = false;
    public loading = false;
    public searchResult$ = new BehaviorSubject<ISearchImageResponse | null>(null);
    public pageSize = 50;
    public page = 0;
    public totalPages = 1;

    public myAlbums$ = this.myAlbumsService.myAlbums$;

    public hasAlbums$ = this.myAlbumsService.hasAlbums$;

    private displayedImagesByShortToken$ = this.searchResult$.pipe(
      map((searchResult) => {
        if(!searchResult?.images) {
          return {} as Dictionary<IImageModel>;
        }

        return keyBy(searchResult.images, (img) => img.shortToken);
      })
    )

    public areActionsUnavailable$ = this.authStateService.user$.pipe(
      map((user) => !user)
    );

    public canDelete$ = combineLatest([
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
    ) { }

    ngOnInit(): void {
    }

    ngOnDestroy(): void {
      this.imagesSelectionService.clearSelection();
    }

    private runSearch(type: PerformType): void {
        let take = this.pageSize;
        let skip = 0;
        if (type === PerformType.Search) {
            skip = 0;
        } else if (type === PerformType.Pagination) {
            skip = this.page * this.pageSize;
        }
        this.imageApi.search({
            skip,
            take,
            onlyFromRegisteredUsers: this.onlyFromRegisteredUsers,
            alsoFromGrids: this.alsoFromGrids,
            searchAsRegexp: this.searchAsRegexp,
            orderBy: this.orderBy.find(x => x.order != null)!.order!,
            orderByField: this.orderBy.find(x => x.order != null)!.field!,
            fields: this.searchFieldsCheckboxes.filter(x => x.checked).map(x => x.id),
            softwares: this.softwareCheckboxes.filter(x => x.checked).map(x => x.id),
            searchText: this.searchText,
        }).subscribe({
            next: resp => {
                this.loading = false;
                this.searchResult$.next(resp);
                this.totalPages = Math.floor(resp.total / this.pageSize) + ((resp.total % this.pageSize) === 0 ? 0 : 1);
                if (type === PerformType.Search) {
                    this.page = 0;
                }
                if (type === PerformType.Pagination) {
                    this.scrollTo?.nativeElement?.scrollIntoView({behavior: "smooth"});
                }
            },
            error: (err: HttpErrorResponse) => {
                this.loading = false;
                this.searchResult$.next(null);
                httpErrorResponseHandler(err, this.toastr);
            }
        });
    }

    public onImageDeleted(): void {
        this.runSearch(PerformType.Search);
    }

    public onPageChange(): void {
        this.runSearch(PerformType.Pagination);
    }

    public onOrderBtnClick(field: SearchImageOrderByFieldType): void {
        for (const entry of this.orderBy) {
            if (entry.field === field) {
                entry.order = entry.order === SearchImageOrderByType.Asc
                    ? SearchImageOrderByType.Desc
                    : SearchImageOrderByType.Asc;
                if (entry.order === SearchImageOrderByType.Desc)
                    entry.icon = 'arrow_upward';
                else if (entry.order === SearchImageOrderByType.Asc)
                    entry.icon = 'arrow_downward';
            } else {
                entry.order = null;
                entry.icon = null;
            }
        }
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

    public deleteSelectedImages() {
      this.dialog
        .open<ConfirmDialogComponent, ConfirmDialogModel, boolean>(
          ConfirmDialogComponent, {
            data: {
              title: 'Delete image',
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
          this.runSearch(PerformType.Search);
        });
    }
}

