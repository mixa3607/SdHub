import {Inject, Injectable} from '@angular/core';
import {delay, map, tap} from 'rxjs/operators';
import {AuthStateService} from "./auth-state.service";
import {LocalStorageService} from "./local-storage.service";
import {UserApi} from "apps/SdHub/src/app/shared/api/user.api";
import {ILoginByPasswordRequest} from "apps/SdHub/src/app/models/autogen/user.models";
import {NGXLogger} from "ngx-logger";

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    constructor(private userApi: UserApi,
                private state: AuthStateService,
                private localStorage: LocalStorageService,
                private logger: NGXLogger) {
        this.init();
    }

    public init(): void {
        const {jwt, refresh} = this.state.getJwtAndRefreshToken();
        this.logger.info('Read jwt and refresh from storage', jwt, refresh);
        if (jwt != null && refresh != null) {
            this.getMe().subscribe(x => {
                this.state.setUser(x);
                this.state.isAuthFinished$.next(true);
            });
            this.state.setJwtAndRefreshToken(jwt, refresh);
        } else {
            this.state.setUser(null);
            this.state.isAuthFinished$.next(true);
        }
    }

    public login(req: ILoginByPasswordRequest) {
        return this.userApi.loginByPassword(req).pipe(
            tap(x => {
                this.state.setJwtAndRefreshToken(x.jwtToken, x.refreshToken);
                this.state.setUser(x.user);
            }),
            map(x => x.user)
        );
    }

    public getMe() {
        return this.userApi.getMe({}).pipe(
            tap(x => {
                this.state.setUser(x.user);
            }),
            map(x => x.user)
        );
    }

    public logout() {
        this.state.setJwtAndRefreshToken(null, null);
        this.state.setUser(null);
    }
}
