import {Component, OnInit} from '@angular/core';
import {combineLatest, takeUntil} from "rxjs";
import {AuthStateService} from "apps/SdHub/src/app/core/services/auth-state.service";
import {UntilDestroy, untilDestroyed} from "@ngneat/until-destroy";
import {IUserModel} from "apps/SdHub/src/app/models/autogen/misc.models";
import {AuthService} from "apps/SdHub/src/app/core/services/auth.service";

@UntilDestroy()
@Component({
    selector: 'app-layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {
    public userName: string = 'Anon';
    public isAnonymous = true;
    public userInfoIsOpen = false;
    public user: IUserModel | null = null;

    constructor(
        public authService: AuthService,
        public authStateService: AuthStateService,) {

        combineLatest([authStateService.isAuthenticated$, this.authStateService.user$])
            .pipe(untilDestroyed(this))
            .subscribe(([isAu, user]) => {
                if (isAu && user != null)
                    this.userName = user.login;
                else if (!isAu)
                    this.userName = "Anon";
                else
                    this.userName = "Loading...";
                this.isAnonymous = !isAu;
                this.user = user;
            });
    }

    ngOnInit(): void {
    }

    public onLogoutClick(): void {
        this.authService.logout();
    }
}
