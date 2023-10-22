import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { BehaviorSubject } from "rxjs";
import { HttpErrorResponse } from "@angular/common/http";
import { IGetImageRequest } from "../../../models/autogen/image.models";
import { IImageModel, IImageParsedMetadataTagModel, IUserModel } from "../../../models/autogen/misc.models";
import { Clipboard } from "@angular/cdk/clipboard";
import { ToastrService } from "ngx-toastr";
import { ImageApi } from "apps/sd-hub/src/app/shared/services/api/image.api";
import { httpErrorResponseHandler } from "apps/sd-hub/src/app/shared/http-error-handling/handlers";
import {
  ManageTokenInputModalComponent
} from "apps/sd-hub/src/app/pages/image/manage-token-input-modal/manage-token-input-modal.component";
import { MatDialog } from "@angular/material/dialog";
import { bytesToHuman } from "apps/sd-hub/src/app/shared/utils/bytes";
import {
  ImageViewerDialogComponent
} from "apps/sd-hub/src/app/shared/components/image-viewer-dialog/image-viewer-dialog.component";
import { AuthStateService } from "apps/sd-hub/src/app/core/services/auth-state.service";

interface IGroupedTags {
  software: string,
  tags: IImageParsedMetadataTagModel[],
}

interface IEditImage {
  name?: string;
  description?: string;
}

@UntilDestroy()
@Component({
  selector: 'app-image-page',
  templateUrl: './image-page.component.html',
  styleUrls: ['./image-page.component.scss']
})
export class ImagePageComponent implements OnInit {
  public user: IUserModel | null = null;
  public loading$ = new BehaviorSubject<boolean>(false);
  public imageInfo: IImageModel | null = null;
  public originalImageHumanSize: string = '0B';
  public loadType: 'ok' | 'error' | null = null;
  public groupedTags: IGroupedTags[] = [];
  public manageToken: string | null = null;

  public editMode: boolean = false;
  public editData: IEditImage = {};
  public showEditButton = false;

  constructor(private imageApi: ImageApi,
              private route: ActivatedRoute,
              private router: Router,
              private clipboard: Clipboard,
              private toastr: ToastrService,
              private authState: AuthStateService,
              private dialog: MatDialog) {
    route.paramMap
      .pipe(untilDestroyed(this))
      .subscribe(x => this.loadImageCard(x.get('shortCode')));
    authState.user$
      .pipe(untilDestroyed(this))
      .subscribe(x => {
        this.user = x;
      });
  }

  ngOnInit(): void {
  }


  private loadImageCard(shortCode: string | null): void {
    if (shortCode == null)
      return;
    this.loading$.next(true);
    const req: IGetImageRequest = {
      shortToken: shortCode
    }
    this.imageApi.get(req).subscribe({
      next: resp => {
        this.loadType = "ok";
        this.applyLoadedImage(resp.image);
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

  private applyLoadedImage(image: IImageModel): void {
    this.imageInfo = image;
    this.groupedTags = [];
    for (const tag of image.parsedMetadata.tags) {
      let group = this.groupedTags.find(x => x.software == tag.software);
      if (group == null) {
        group = {software: tag.software, tags: []};
        this.groupedTags.push(group);
      }
      group.tags.push(tag);
    }
    this.originalImageHumanSize = bytesToHuman(image.originalImage!.size);
    this.showEditButton = image.owner.isAnonymous || (this.user?.guid === image.owner.guid);
  }

  public onCopyShortLinkClick(): void {
    if (this.imageInfo?.shortUrl == null)
      return;
    this.clipboard.copy(this.imageInfo.shortUrl);
    this.toastr.success('Short link copy to clipboard');
  }

  public onCopyTagClick(tag: IImageParsedMetadataTagModel): void {
    this.clipboard.copy(tag.value);
    this.toastr.success(`Value of ${tag.name} copied to clipboard`);
  }

  public onEditClick(): void {
    if (this.imageInfo?.owner.isAnonymous) {
      this.toastr.error('Manage token required');
      this.showManageTokenInputDialog();
    } else {
      this.editData.name = this.imageInfo?.name;
      this.editData.description = this.imageInfo?.description;
      this.editMode = true;
    }
  }

  private showManageTokenInputDialog(): void {
    ManageTokenInputModalComponent.open({}, this.dialog).afterClosed()
      .subscribe(x => {
        if (x?.manageToken == null)
          return;
        this.imageApi.checkManageToken({manageToken: x.manageToken, shortToken: this.imageInfo?.shortToken!})
          .subscribe({
            next: resp => {
              if (resp.isValid) {
                this.editData.name = this.imageInfo?.name;
                this.editData.description = this.imageInfo?.description;
                this.editMode = true;
              } else {
                this.toastr.error('Token is invalid');
              }
            },
            error: (err: HttpErrorResponse) => {
              httpErrorResponseHandler(err, this.toastr);
            }
          });
        this.manageToken = x.manageToken;
      });
  }

  public onSaveClick(): void {
    const req = {
      image: {...this.editData, shortToken: this.imageInfo?.shortToken!},
      manageToken: this.manageToken!
    };
    this.imageApi.edit(req).subscribe({
      next: resp => {
        if (resp.success) {
          this.editMode = false;
          this.applyLoadedImage(resp.image);
          this.toastr.success(resp.reason, 'All changes saved');
        } else {
          this.toastr.error(resp.reason, 'Cant save changes');
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
    this.imageApi.delete({manageToken: this.manageToken!, shortToken: this.imageInfo?.shortToken!}).subscribe({
      next: resp => {
        if (resp.success) {
          this.editMode = false;
          this.toastr.success('Image deleted');
          void this.router.navigate(['/']);
        } else {
          this.toastr.error(resp.reason, 'Cant save changes');
        }
      },
      error: (err: HttpErrorResponse) => {
        this.editMode = false;
        httpErrorResponseHandler(err, this.toastr);
      }
    });
  }

  public onOpenImageViewerClick(): void {
    ImageViewerDialogComponent.open({imageInfo: this.imageInfo!}, this.dialog);
  }
}
