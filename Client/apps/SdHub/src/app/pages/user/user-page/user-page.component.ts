import {Component} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {ImageApi} from "apps/SdHub/src/app/shared/api/image.api";
import {Clipboard} from "@angular/cdk/clipboard";
import {ToastrService} from "ngx-toastr";
import {MatDialog} from "@angular/material/dialog";
import {UntilDestroy, untilDestroyed} from "@ngneat/until-destroy";
import {AuthStateService} from "apps/SdHub/src/app/core/services/auth-state.service";
import {combineLatest, filter, first, switchMap, tap} from "rxjs";
import {map} from "rxjs/operators";
import {UserApi} from "apps/SdHub/src/app/shared/api/user.api";
import {IUserModel} from "apps/SdHub/src/app/models/autogen/misc.models";
import {HttpErrorResponse} from "@angular/common/http";
import {httpErrorResponseHandler} from "apps/SdHub/src/app/shared/http-error-handling/handlers";

@UntilDestroy()
@Component({
    selector: 'app-search-page',
    templateUrl: './user-page.component.html',
    styleUrls: ['./user-page.component.scss']
})
export class UserPageComponent {
    public user: IUserModel | null = null;
    public loading: boolean = false;
    public editMode = false;

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
}
