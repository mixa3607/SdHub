import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { Clipboard } from "@angular/cdk/clipboard";
import { ToastrService } from "ngx-toastr";
import { AuthStateService } from "apps/SdHub/src/app/core/services/auth-state.service";
import { MatDialog } from "@angular/material/dialog";
import { ModelApi } from "apps/SdHub/src/app/shared/services/api/model.api";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { HttpErrorResponse } from "@angular/common/http";
import { httpErrorResponseHandler } from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import { BehaviorSubject } from "rxjs";
import { IImageModel, SdVersion } from "apps/SdHub/src/app/models/autogen/misc.models";
import { bytesToHuman } from "apps/SdHub/src/app/shared/utils/bytes";
import { IModelModel } from "apps/SdHub/src/app/models/autogen/model.models";

@UntilDestroy()
@Component({
  selector: 'model-page',
  templateUrl: './model-page.component.html',
  styleUrls: ['./model-page.component.scss'],
})
export class ModelPageComponent implements OnInit {
  public loadType: 'ok' | 'error' | null = null;
  public isAdmin = false;
  public showEditButton = false;
  public loading$ = new BehaviorSubject<boolean>(false);
  public model: IModelModel|null = null;

  constructor(private modelApi: ModelApi,
              private route: ActivatedRoute,
              private router: Router,
              private clipboard: Clipboard,
              private toastr: ToastrService,
              private authState: AuthStateService,
              private dialog: MatDialog) {
    route.paramMap
      .pipe(untilDestroyed(this))
      .subscribe(x => this.loadCard(x.get('shortCode')));
    authState.isAdmin$
      .pipe(untilDestroyed(this))
      .subscribe(x => {
        this.isAdmin = x;
        this.showEditButton = true;
      });
  }

  ngOnInit(): void {
  }

  public sdVerToStr(ver: SdVersion): string{
    return SdVersion[ver];
  }

  private loadCard(idStr: string | null): void {
    const id = Number(idStr?.split('-')[0]);
    if (id == 0 || isNaN(id))
      return;
    this.loading$.next(true);
    this.modelApi.get({id}).subscribe({
      next: resp => {
        this.loadType = "ok";
        this.model = resp;
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
}
