import { Injectable } from "@angular/core";
import { BehaviorSubject, combineLatest, filter, first, ReplaySubject } from "rxjs";
import { IUserModel } from "apps/SdHub/src/app/models/autogen/misc.models";
import { LocalStorageService } from "apps/SdHub/src/app/core/services/local-storage.service";
import { NGXLogger } from "ngx-logger";
import { map } from "rxjs/operators";
import { UserRoleTypes } from "apps/SdHub/src/app/models/autogen/user.models";

@Injectable({
  providedIn: 'root'
})
export class AuthStateService {
  public isAuthenticated$ = new ReplaySubject<boolean>(1);
  public user$ = new BehaviorSubject<IUserModel | null>(null);
  public jwtToken$ = new BehaviorSubject<string | null>(null);
  public refreshToken$ = new BehaviorSubject<string | null>(null);
  public isAuthFinished$ = new BehaviorSubject<boolean>(false);
  public isAdmin$ = combineLatest([this.isAuthenticated$, this.user$])
    .pipe(
      filter(([isAu, user]) => isAu),
      first(),
      map(([isAu, user]) => {
        const isAdmin = user!.roles.indexOf(UserRoleTypes.Admin) > -1;
        return isAdmin;
      })
    );

  constructor(private localStorage: LocalStorageService,
              private logger: NGXLogger) {
  }

  public setJwtAndRefreshToken(jwt: string | null, refresh: string | null): void {
    this.logger.log('Set jwt and refresh', jwt, refresh);

    this.localStorage.setJwtToken(jwt);
    this.localStorage.setRefreshToken(refresh);
    this.jwtToken$.next(jwt);
    this.refreshToken$.next(refresh);
  }

  public setUser(user: IUserModel | null): void {
    if (user == null) {
      this.user$.next(null);
      this.isAuthenticated$.next(false);
    } else {
      this.user$.next(user);
      this.isAuthenticated$.next(true);
    }
  }

  public getJwtAndRefreshToken(): { jwt: string | null, refresh: string | null } {
    const jwt = this.localStorage.getJwtToken();
    const refresh = this.localStorage.getRefreshToken();
    return {jwt, refresh};
  }
}
