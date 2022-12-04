import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {PerformType} from "apps/SdHub/src/app/pages/generated/search-args.service";
import {HttpErrorResponse} from "@angular/common/http";
import {httpErrorResponseHandler} from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import {ToastrService} from "ngx-toastr";
import {
    ISearchAlbumResponse,
    SearchAlbumOrderByFieldType,
    SearchAlbumOrderByType
} from "apps/SdHub/src/app/models/autogen/album.models";
import {AlbumApi} from "apps/SdHub/src/app/shared/services/api/album.api";
import {AddAlbumDialogComponent} from "apps/SdHub/src/app/pages/user/add-album-dialog/add-album-dialog.component";
import {MatDialog} from "@angular/material/dialog";

@Component({
    selector: 'user-albums',
    templateUrl: './user-albums.component.html',
    styleUrls: ['./user-albums.component.scss'],
})
export class UserAlbumsComponent implements OnInit {
    get userLogin(): string | null {
        return this._userLogin;
    }

    @Input() set userLogin(value: string | null) {
        if (value == this._userLogin)
            return;
        this._userLogin = value;
        this.runSearch(PerformType.Search);
    }

    @Input() enableAddButton = false;

    private _userLogin: string | null = null;

    @ViewChild('scrollTo', {read: ElementRef}) scrollTo?: ElementRef;

    public readonly pageSize = 50;
    public loading = false;
    public searchResult: ISearchAlbumResponse | null = null;
    public page = 0;
    public totalPages = 1;

    constructor(private albumApi: AlbumApi,
                private toastr: ToastrService,
                private matDialog: MatDialog,) {
    }

    ngOnInit(): void {
    }

    public onPageChange(): void {
        this.runSearch(PerformType.Pagination);
    }

    public onCreateNewClick(): void {
        AddAlbumDialogComponent.open({}, this.matDialog).afterClosed().subscribe(x => {
            if (x?.createdAlbum != null)
                this.runSearch(PerformType.Search);
        });
    }

    private runSearch(type: PerformType): void {
        let take = this.pageSize;
        let skip = 0;
        if (type === PerformType.Search) {
            skip = 0;
        } else if (type === PerformType.Pagination) {
            skip = this.page * this.pageSize;
        }
        this.albumApi.search({
            owner: this._userLogin!,
            skip,
            take,
            orderBy: SearchAlbumOrderByType.Asc,
            orderByField: SearchAlbumOrderByFieldType.UploadDate,
            fields: []
        }).subscribe({
            next: resp => {
                this.loading = false;
                this.searchResult = resp;
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
