import {Component, ElementRef, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {ImageApi} from "apps/SdHub/src/app/shared/services/api/image.api";
import {Clipboard} from "@angular/cdk/clipboard";
import {ToastrService} from "ngx-toastr";
import {MatDialog} from "@angular/material/dialog";
import {UntilDestroy, untilDestroyed} from "@ngneat/until-destroy";
import {AuthStateService} from "apps/SdHub/src/app/core/services/auth-state.service";
import {combineLatest, filter, first} from "rxjs";
import {map} from "rxjs/operators";
import {UserApi} from "apps/SdHub/src/app/shared/services/api/user.api";
import {IUserModel} from "apps/SdHub/src/app/models/autogen/misc.models";
import {HttpErrorResponse} from "@angular/common/http";
import {httpErrorResponseHandler} from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import {
    ISearchImageResponse,
    SearchImageOrderByFieldType,
    SearchImageOrderByType
} from "apps/SdHub/src/app/models/autogen/image.models";
import {PerformType} from "apps/SdHub/src/app/pages/generated/search-args.service";

type SearchTabType = 'image' | 'grid' | 'album';

@UntilDestroy()
@Component({
    selector: 'app-search-page',
    templateUrl: './user-page.component.html',
    styleUrls: ['./user-page.component.scss']
})
export class UserPageComponent {
    public user: IUserModel | null = null;
    public allowEdit = false;
    public loading: boolean = false;
    public editMode = false;
    public readonly tabs: { enable: boolean, name: string, type: SearchTabType }[] = [
        {name: 'Images', enable: true, type: 'image'},
        {name: 'Albums', enable: false, type: 'album'},
        {name: 'Grids', enable: false, type: 'grid'},
    ];
    public activeTab: SearchTabType = this.tabs.find(x => x.enable)!.type;
    @ViewChild('scrollTo', {read: ElementRef}) scrollTo?: ElementRef;

    public readonly pageSize = 50;
    public loadingImages = false;
    public searchImagesResult: ISearchImageResponse | null = null;
    public page = 0;
    public totalPages = 1;

    constructor(private route: ActivatedRoute,
                private authState: AuthStateService,
                private router: Router,
                private imageApi: ImageApi,
                private userApi: UserApi,
                private clipboard: Clipboard,
                private toastr: ToastrService,
                private dialog: MatDialog) {
        route.paramMap
            .pipe(untilDestroyed(this))
            .subscribe(x => this.loadUser(x.get('user')))
    }

    private runSearchOnActiveTab(type: PerformType): void {
        if (this.activeTab === "image") {
            this.runImageSearch(type);
        }
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
            owner: this.user?.login!,
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
                if (type === PerformType.Pagination){
                    this.scrollTo?.nativeElement?.scrollIntoView({behavior: "smooth"});
                }
            },
            error: (err: HttpErrorResponse) => {
                this.loading = false;
                httpErrorResponseHandler(err, this.toastr);
            }
        })
    }

    public loadUser(login: string | null): void {
        if (login == null) {
            combineLatest([this.authState.user$, this.authState.isAuthFinished$]).pipe(
                untilDestroyed(this),
                filter(([, isF]) => isF),
                first(),
                map(([u,]) => u)
            ).subscribe(x => {
                void this.router.navigate(['user', x!.login], {
                    replaceUrl: true
                });
            });
            return;
        }

        this.userApi.get({login})
            .subscribe({
                next: resp => {
                    this.loading = false;
                    resp.user.about ??= '';
                    this.user = resp.user;
                    this.runSearchOnActiveTab(PerformType.Search);
                    this.authState.user$
                        .pipe(first(), filter(x => x != null))
                        .subscribe(x => this.allowEdit = x!.loginNormalized == resp.user.loginNormalized);
                },
                error: (err: HttpErrorResponse) => {
                    this.user = null;
                    this.loading = false;
                    httpErrorResponseHandler(err, this.toastr);
                }
            });
    }

    public onEditClick(): void {
        this.editMode = !this.editMode;
    }

    public onSaveClick(): void {
        if (this.user == null)
            return;

        this.loading = true;
        this.userApi.edit(this.user).subscribe({
            next: resp => {
                this.loading = false;
                resp.user.about ??= '';
                this.user = resp.user;
                this.editMode = false;
            },
            error: (err: HttpErrorResponse) => {
                this.loading = false;
                httpErrorResponseHandler(err, this.toastr);
            }
        });
    }

    public onTabChange(tabName: SearchTabType): void {
        this.totalPages = 1;
        this.page = 0;
        this.searchImagesResult = null;
        this.activeTab = tabName;
        this.runSearchOnActiveTab(PerformType.Search);
    }

    public onPageChange(): void {
        this.runSearchOnActiveTab(PerformType.Pagination);
    }
}
