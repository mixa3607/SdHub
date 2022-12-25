import { Component, OnInit } from '@angular/core';
import {
  IImageModel,
  IUserModel, IUserSimpleModel
} from "apps/SdHub/src/app/models/autogen/misc.models";
import { BehaviorSubject } from "rxjs";
import { ActivatedRoute, Router } from "@angular/router";
import { Clipboard } from "@angular/cdk/clipboard";
import { ToastrService } from "ngx-toastr";
import { AuthStateService } from "apps/SdHub/src/app/core/services/auth-state.service";
import { MatDialog } from "@angular/material/dialog";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { IGetImageRequest } from "apps/SdHub/src/app/models/autogen/image.models";
import { HttpErrorResponse } from "@angular/common/http";
import { httpErrorResponseHandler } from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import { GridApi } from "apps/SdHub/src/app/shared/services/api/grid.api";
import { IGridModel } from "apps/SdHub/src/app/models/autogen/grid.models";
import {
  ImageViewerDialogComponent
} from "apps/SdHub/src/app/shared/components/image-viewer-dialog/image-viewer-dialog.component";

interface IEditGrid {
  name?: string;
  description?: string;
}

@UntilDestroy()
@Component({
  selector: 'app-grid-page',
  templateUrl: './grid-page.component.html',
  styleUrls: ['./grid-page.component.scss'],
})
export class GridPageComponent implements OnInit {
  public user: IUserModel | null = null;
  public loading$ = new BehaviorSubject<boolean>(false);
  public loadType: 'ok' | 'error' | null = null;

  public editMode: boolean = false;
  public editData: IEditGrid = {};
  public showEditButton = false;

  public name: string = '';
  public description: string = '';
  public thumbnailUrl: string = '';
  public createdAt: string = '';
  public shortUrl: string = '';
  public shortToken: string = '';
  public owner: IUserSimpleModel | null = null;
  public xTiles: number = -1;
  public yTiles: number = -1;
  public info: IGridModel | null = null;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private clipboard: Clipboard,
              private toastr: ToastrService,
              private authState: AuthStateService,
              private dialog: MatDialog,
              private gridApi: GridApi) {
    route.paramMap
      .pipe(untilDestroyed(this))
      .subscribe(x => this.loadGridCard(x.get('shortCode')));
    authState.user$
      .pipe(untilDestroyed(this))
      .subscribe(x => {
        this.user = x;
      });
  }

  ngOnInit(): void {
  }

  private loadGridCard(shortCode: string | null): void {
    if (shortCode == null)
      return;
    this.loading$.next(true);
    const req: IGetImageRequest = {
      shortToken: shortCode
    }
    this.gridApi.get(req).subscribe({
      next: resp => {
        this.loadType = "ok";
        this.applyLoadedGrid(resp.grid);
        this.loading$.next(false);
      },
      error: (err: HttpErrorResponse) => {
        this.showEditButton = false;
        this.loadType = "error";
        this.loading$.next(false);
        httpErrorResponseHandler(err, this.toastr);
      }
    })
  }

  private applyLoadedGrid(grid: IGridModel): void {
    this.info = grid;
    this.name = grid.name ?? '';
    this.shortToken = grid.shortToken ?? '';
    this.description = grid.description ?? '';
    this.thumbnailUrl = grid.thumbImage?.directUrl ?? '';
    this.shortUrl = grid.shortUrl ?? '';
    this.createdAt = grid.createdAt;
    this.owner = grid.owner;
    this.xTiles = grid.xTiles;
    this.yTiles = grid.yTiles;
    this.showEditButton = this.user?.guid === grid.owner.guid;
  }

  public onCopyShortLinkClick(): void {
    if (this.shortUrl == '')
      return;
    this.clipboard.copy(this.shortUrl);
    this.toastr.success('Short link copy to clipboard');
  }

  public onEditClick(): void {
    this.editData.name = this.name;
    this.editData.description = this.description;
    this.editMode = true;
  }

  public onSaveClick(): void {
    const req = {...this.editData, shortToken: this.shortToken};
    this.gridApi.edit(req).subscribe({
      next: resp => {
        if (resp.success) {
          this.editMode = false;
          this.applyLoadedGrid(resp.grid);
          this.toastr.success(resp.reason, 'All changes saved');
        } else {
          this.toastr.error(resp.reason, 'Can\'t save changes');
        }
      },
      error: (err: HttpErrorResponse) => {
        this.editMode = false;
        httpErrorResponseHandler(err, this.toastr);
      }
    });
  }

  public onCancelClick(): void {
    this.editMode = false;
  }

  public onDeleteClick(): void {
    this.gridApi.delete({shortToken: this.shortToken}).subscribe({
      next: resp => {
        if (resp.success) {
          this.editMode = false;
          this.toastr.success('Grid deleted');
          void this.router.navigate(['/']);
        } else {
          this.toastr.error(resp.reason, 'Can\'t save changes');
        }
      },
      error: (err: HttpErrorResponse) => {
        this.editMode = false;
        httpErrorResponseHandler(err, this.toastr);
      }
    });
  }

  public onOpenImageViewerClick(): void {
    ImageViewerDialogComponent.open({gridInfo: this.info!}, this.dialog);
  }
}
