import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {
    ISearchImageRequest,
    ISearchImageResponse,
    SearchImageInFieldType,
    SearchImageOrderByFieldType,
    SearchImageOrderByType
} from "apps/SdHub/src/app/models/autogen/image.models";
import {PerformType, SearchArgsService} from "apps/SdHub/src/app/pages/generated/search-args.service";
import {first, switchMap, tap} from "rxjs";
import {UntilDestroy, untilDestroyed} from "@ngneat/until-destroy";
import {ImageApi} from "apps/SdHub/src/app/shared/api/image.api";
import {HttpErrorResponse} from "@angular/common/http";
import {httpErrorResponseHandler} from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import {ToastrService} from "ngx-toastr";

@UntilDestroy()
@Component({
    selector: 'search-in-images',
    templateUrl: './search-in-images.component.html',
    styleUrls: ['./search-in-images.component.scss'],
})
export class SearchInImagesComponent implements OnInit {
    private _searchText = '';
    get searchText(): string {
        return this._searchText;
    }

    @Input() set searchText(value: string) {
        const oldVal = this._searchText;
        this._searchText = value;
        if (oldVal != value)
            this.argsService.updateImagesArgs({searchText: value});
    }

    @Output() searchTextChange = new EventEmitter<string>();

    public readonly orderBy: { name: string, field: SearchImageOrderByFieldType, order: SearchImageOrderByType | null, icon: string | null }[] = [
        {
            name: 'Upload date',
            field: SearchImageOrderByFieldType.UploadDate,
            order: null,
            icon: null,
        },
        {
            name: 'User name',
            field: SearchImageOrderByFieldType.UserName,
            order: null,
            icon: null,
        },
    ];
    public readonly softwareCheckboxes: { id: string, value: string, checked: boolean }[] = [
        {id: 'AutomaticWebUi', value: 'AutomaticWebUi', checked: false},
        {id: 'DreamStudio', value: 'DreamStudio', checked: false},
        {id: 'NovelAi', value: 'NovelAi', checked: false},
        {id: 'Unknown', value: '-----', checked: false},
    ];
    public readonly searchFieldsCheckboxes: { id: SearchImageInFieldType, value: string, checked: boolean }[] = [
        {id: SearchImageInFieldType.Prompt, value: 'Prompt', checked: false},
        {id: SearchImageInFieldType.Name, value: 'Name', checked: false},
        {id: SearchImageInFieldType.Description, value: 'Description', checked: false},
        {id: SearchImageInFieldType.User, value: 'User name', checked: false},
    ];
    public alsoFromGrids = false;
    public onlyFromRegisteredUsers = true;
    public searchAsRegexp = false;
    public loading = false;
    public searchResult: ISearchImageResponse | null = null;
    public pageSize = 50;
    public page = 0;
    public totalPages = 1;

    constructor(private argsService: SearchArgsService,
                private imageApi: ImageApi,
                private toastr: ToastrService,) {
        argsService.resetImagesArgs();
        argsService.searchInImagesArgs$.pipe(untilDestroyed(this)).subscribe(x => this.applyArgs(x));
        argsService.searchPerform$.pipe(untilDestroyed(this)).subscribe(x => this.performSearch(x));
        argsService.searchPerform$.next(PerformType.Search);
    }

    ngOnInit(): void {
    }

    private performSearch(type: PerformType): void {
        this.loading = true;
        this.argsService.searchInImagesArgs$
            .pipe(first(),
                tap(x => {
                    x.take = this.pageSize;
                    if (type === PerformType.Search) {
                        x.skip = 0;
                    } else if (type === PerformType.Pagination) {
                        x.skip = this.page * this.pageSize;
                    }
                }),
                switchMap(x => this.imageApi.search(x)))
            .subscribe({
                next: resp => {
                    this.loading = false;
                    this.searchResult = resp;
                    this.totalPages = Math.floor(resp.total / this.pageSize) + ((resp.total % this.pageSize) === 0 ? 0 : 1);
                    if (type === PerformType.Search) {
                        this.page = 0;
                    }
                },
                error: (err: HttpErrorResponse) => {
                    this.searchResult = null;
                    this.loading = false;
                    httpErrorResponseHandler(err, this.toastr);
                }
            })
    }

    private applyArgs(args: ISearchImageRequest): void {
        this.searchText = args.searchText;
        this.searchTextChange.emit(args.searchText);

        this.onlyFromRegisteredUsers = args.onlyFromRegisteredUsers;
        this.alsoFromGrids = args.alsoFromGrids;
        this.searchAsRegexp = args.searchAsRegexp;

        // order by
        for (const entry of this.orderBy) {
            if (entry.field === args.orderByField) {
                entry.order = args.orderBy;
                if (entry.order === SearchImageOrderByType.Desc)
                    entry.icon = 'arrow_upward';
                else if (entry.order === SearchImageOrderByType.Asc)
                    entry.icon = 'arrow_downward';
            } else {
                entry.order = null;
                entry.icon = null;
            }
        }

        // soft
        for (const cb of this.softwareCheckboxes) {
            cb.checked = args.softwares.some(x => x === cb.id);
        }

        // fields
        for (const cb of this.searchFieldsCheckboxes) {
            cb.checked = args.fields.some(x => x === cb.id);
        }
    }

    public onOrderBtnClick(field: SearchImageOrderByFieldType): void {
        this.argsService.searchInImagesArgs$.pipe(first()).subscribe(x => {
            this.argsService.updateImagesArgs({
                orderByField: field,
                orderBy: x.orderByField === field && x.orderBy === SearchImageOrderByType.Asc
                    ? SearchImageOrderByType.Desc
                    : SearchImageOrderByType.Asc
            });
        });
    }

    public onFormChange(): void {
        this.argsService.updateImagesArgs({
            onlyFromRegisteredUsers: this.onlyFromRegisteredUsers,
            alsoFromGrids: this.alsoFromGrids,
            searchAsRegexp: this.searchAsRegexp,
            orderBy: this.orderBy.find(x => x.order != null)!.order!,
            orderByField: this.orderBy.find(x => x.order != null)!.field!,
            fields: this.searchFieldsCheckboxes.filter(x => x.checked).map(x => x.id),
            softwares: this.softwareCheckboxes.filter(x => x.checked).map(x => x.id),
        });
    }

    public onPageChange(): void {
        this.argsService.searchPerform$.next(PerformType.Pagination);
    }
}
