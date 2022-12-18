import { Clipboard } from '@angular/cdk/clipboard';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { AuthStateService } from 'apps/SdHub/src/app/core/services/auth-state.service';
import { IAlbumModel } from 'apps/SdHub/src/app/models/autogen/album.models';
import {
  SearchImageOrderByFieldType,
  SearchImageOrderByType
} from 'apps/SdHub/src/app/models/autogen/image.models';
import { IImageModel, IImageOwnerModel, IPaginationResponse } from 'apps/SdHub/src/app/models/autogen/misc.models';
import { PerformType } from 'apps/SdHub/src/app/pages/generated/search-page/search-page.component';
import { httpErrorResponseHandler } from 'apps/SdHub/src/app/shared/http-error-handling/handlers';
import { AlbumApi } from 'apps/SdHub/src/app/shared/services/api/album.api';
import { ImageApi } from 'apps/SdHub/src/app/shared/services/api/image.api';
import { ToastrService } from 'ngx-toastr';
import { ImageSelectionService } from '../../../core/services/image-selection.service';
import { MyAlbumsService } from '../../../core/services/my-albums.service';

interface IAlbumEdit {
  name: string | null;
  description: string | null;
}

@UntilDestroy()
@Component({
  selector: 'album-page',
  templateUrl: './album-page.component.html',
  styleUrls: ['./album-page.component.scss'],
})
export class AlbumPageComponent implements OnInit {
  public loading = false;
  public editMode = false;
  public canEdit = false;
  public currentUser: string | null = null;
  public editData: IAlbumEdit = {name: null, description: null};

  public thumbUrl = '';
  public name = '';
  public description = '';
  public shortToken = '';
  public albumOwner: IImageOwnerModel | null = null;
  public album: IAlbumModel | null = null;

  @ViewChild('scrollTo', {read: ElementRef}) scrollTo?: ElementRef;

  public readonly pageSize = 50;
  public loadingImages = false;
  public searchResult: IPaginationResponse<IImageModel> | null = null;
  public page = 0;
  public totalPages = 1;

  constructor(private imageApi: ImageApi,
              private albumApi: AlbumApi,
              private toastr: ToastrService,
              private myAlbumsService: MyAlbumsService,
              private imageSelectionService: ImageSelectionService,
              private route: ActivatedRoute,
              private router: Router,
              private clipboard: Clipboard,
              private authState: AuthStateService,
              private dialog: MatDialog) {
    route.paramMap
      .pipe(untilDestroyed(this))
      .subscribe(x => this.loadAlbumCard(x.get('shortCode')));
    authState.user$
      .pipe(untilDestroyed(this))
      .subscribe(x => {
        this.currentUser = x?.login ?? null;
        this.canEdit = this.currentUser === this.albumOwner?.login;
      });
  }

  ngOnInit(): void {
  }

  private loadAlbumCard(shortToken: string | null): void {
    if (shortToken == null)
      return;

    this.albumApi.get({shortToken}).subscribe({
      next: resp => {
        this.applyLoadedAlbum(resp.album);
        this.loading = false;
        this.runImageSearch(PerformType.Search);
      },
      error: (err: HttpErrorResponse) => {
        this.canEdit = false;
        this.loading = false;
        httpErrorResponseHandler(err, this.toastr);
      }
    })
  }

  private applyLoadedAlbum(album: IAlbumModel): void {
    this.albumOwner = album.owner;
    this.description = album.description;
    this.name = album.name;
    this.thumbUrl = album.thumbImage?.directUrl ?? '';
    this.shortToken = album.shortToken;
    this.album = album;

    this.canEdit = this.currentUser === this.albumOwner?.login;
  }


  public onPageChange(): void {
    this.runImageSearch(PerformType.Pagination);
  }

  public onEditClick(): void {
    this.editData.name = this.name;
    this.editData.description = this.description;
    this.editMode = true;
  }

  public onCancelClick(): void {
    this.editMode = false;
  }

  public onReloadImages(): void {
    this.runImageSearch(PerformType.Search);
  }

  public onSaveClick(): void {
    this.albumApi.edit({
      shortToken: this.shortToken,
      name: this.editData.name ?? '',
      description: this.editData.description ?? ''
    }).subscribe({
      next: resp => {
        this.editMode = false;
        this.applyLoadedAlbum(resp);
        this.myAlbumsService.reloadAlbums();
        this.toastr.success('All changes saved');
      },
      error: (err: HttpErrorResponse) => {
        this.editMode = false;
        httpErrorResponseHandler(err, this.toastr);
      }
    });
  }

  public onDeleteClick(): void{
    this.albumApi.delete({
      shortToken: this.shortToken
    }).subscribe({
      next: resp => {
        this.editMode = false;
        this.myAlbumsService.reloadAlbums();
        this.toastr.success('Deleted (┬┬﹏┬┬)');
        void this.router.navigate(['/']);
      },
      error: (err: HttpErrorResponse) => {
        this.editMode = false;
        httpErrorResponseHandler(err, this.toastr);
      }
    });
  }

  private runImageSearch(type: PerformType): void {
    this.loadingImages = true;
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
      orderBy: SearchImageOrderByType.Asc,
      orderByField: SearchImageOrderByFieldType.UploadDate,
      softwares: [],
      fields: [],
      album: this.shortToken,
    }).subscribe({
      next: resp => {
        this.imageSelectionService.clearSelection();
        this.loadingImages = false;
        this.searchResult = resp;
        this.totalPages = Math.floor(resp.total / this.pageSize) + ((resp.total % this.pageSize) === 0 ? 0 : 1);
        if (type === PerformType.Search) {
          this.page = 0;
        }
        if (type === PerformType.Pagination) {
          this.scrollTo?.nativeElement?.scrollIntoView({behavior: 'smooth'});
        }
      },
      error: (err: HttpErrorResponse) => {
        this.loadingImages = false;
        httpErrorResponseHandler(err, this.toastr);
      }
    })
  }
}
