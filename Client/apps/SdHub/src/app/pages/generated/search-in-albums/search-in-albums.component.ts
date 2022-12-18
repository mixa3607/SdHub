import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {Observable} from "rxjs";
import {UntilDestroy, untilDestroyed} from "@ngneat/until-destroy";
import {HttpErrorResponse} from "@angular/common/http";
import {httpErrorResponseHandler} from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import {ToastrService} from "ngx-toastr";
import {AlbumApi} from "apps/SdHub/src/app/shared/services/api/album.api";
import {
  IAlbumModel,
  SearchAlbumInFieldType,
  SearchAlbumOrderByFieldType,
  SearchAlbumOrderByType
} from "apps/SdHub/src/app/models/autogen/album.models";
import {PerformType} from "apps/SdHub/src/app/pages/generated/search-page/search-page.component";
import { IPaginationResponse } from "apps/SdHub/src/app/models/autogen/misc.models";

@UntilDestroy()
@Component({
    selector: 'search-in-albums',
    templateUrl: './search-in-albums.component.html',
    styleUrls: ['./search-in-albums.component.scss'],
})
export class SearchInAlbumsComponent implements OnInit {
    public readonly orderBy: { name: string, field: SearchAlbumOrderByFieldType, order: SearchAlbumOrderByType | null, icon: string | null }[] = [
        {
            name: 'Upload date',
            field: SearchAlbumOrderByFieldType.UploadDate,
            order: SearchAlbumOrderByType.Asc,
            icon: 'arrow_downward',
        },
        {
            name: 'User name',
            field: SearchAlbumOrderByFieldType.UserName,
            order: null,
            icon: null,
        },
    ];
    public readonly searchFieldsCheckboxes: { id: SearchAlbumInFieldType, value: string, checked: boolean }[] = [
        {id: SearchAlbumInFieldType.Name, value: 'Name', checked: true},
        {id: SearchAlbumInFieldType.Description, value: 'Description', checked: true},
        {id: SearchAlbumInFieldType.User, value: 'User name', checked: false},
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
    public loading = false;
    public searchResult: IPaginationResponse<IAlbumModel> | null = null;
    public pageSize = 50;
    public page = 0;
    public totalPages = 1;

    constructor(private albumApi: AlbumApi,
                private toastr: ToastrService,) {
    }

    ngOnInit(): void {
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
            skip,
            take,
            orderBy: this.orderBy.find(x => x.order != null)!.order!,
            orderByField: this.orderBy.find(x => x.order != null)!.field!,
            fields: this.searchFieldsCheckboxes.filter(x => x.checked).map(x => x.id),
            searchText: this.searchText,
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
                this.searchResult = null;
                httpErrorResponseHandler(err, this.toastr);
            }
        });
    }

    public onPageChange(): void {
        this.runSearch(PerformType.Pagination);
    }

    public onOrderBtnClick(field: SearchAlbumOrderByFieldType): void {
        for (const entry of this.orderBy) {
            if (entry.field === field) {
                entry.order = entry.order === SearchAlbumOrderByType.Asc
                    ? SearchAlbumOrderByType.Desc
                    : SearchAlbumOrderByType.Asc;
                if (entry.order === SearchAlbumOrderByType.Desc)
                    entry.icon = 'arrow_upward';
                else if (entry.order === SearchAlbumOrderByType.Asc)
                    entry.icon = 'arrow_downward';
            } else {
                entry.order = null;
                entry.icon = null;
            }
        }
    }
}
