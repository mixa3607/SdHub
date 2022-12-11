import { HttpErrorResponse } from "@angular/common/http";
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import {
  ISearchImageResponse,
  SearchImageOrderByFieldType,
  SearchImageOrderByType
} from "apps/SdHub/src/app/models/autogen/image.models";
import { PerformType } from "apps/SdHub/src/app/pages/generated/search-page/search-page.component";
import { httpErrorResponseHandler } from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import { ImageApi } from "apps/SdHub/src/app/shared/services/api/image.api";
import { ToastrService } from "ngx-toastr";

@Component({
    selector: 'user-images',
    templateUrl: './user-images.component.html',
    styleUrls: ['./user-images.component.scss'],
})
export class UserImagesComponent implements OnInit {
    get userLogin(): string | null {
        return this._userLogin;
    }

    @Input() set userLogin(value: string | null) {
        if (value == this._userLogin)
            return;
        this._userLogin = value;
        this.runImageSearch(PerformType.Search);
    }

    private _userLogin: string | null = null;

    @ViewChild('scrollTo', {read: ElementRef}) scrollTo?: ElementRef;

    public readonly pageSize = 50;
    public loadingImages = false;
    public searchImagesResult: ISearchImageResponse | null = null;
    public page = 0;
    public totalPages = 1;
    public loading: boolean = false;

    constructor(private imageApi: ImageApi,
                private toastr: ToastrService,) {
    }

    ngOnInit(): void {
    }

    public onPageChange(): void {
        this.runImageSearch(PerformType.Pagination);
    }

    public onReloadImages(): void {
      this.runImageSearch(PerformType.Search);
    }

    private runImageSearch(type: PerformType): void {
        let take = this.pageSize;
        let skip = 0;
        if (type === PerformType.Search) {
            skip = 0;
        } else if (type === PerformType.Pagination) {
            skip = this.page * this.pageSize;
        }
        this.imageApi.search({
            owner: this._userLogin!,
            skip,
            take,
            orderBy: SearchImageOrderByType.Asc,
            orderByField: SearchImageOrderByFieldType.UploadDate,
            softwares: [],
            fields: []
        }).subscribe({
            next: resp => {
                this.loading = false;
                this.searchImagesResult = resp;
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
                httpErrorResponseHandler(err, this.toastr);
            }
        })
    }
}
