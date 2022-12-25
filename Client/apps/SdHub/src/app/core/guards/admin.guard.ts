import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { AuthStateService } from "apps/SdHub/src/app/core/services/auth-state.service";
import { combineLatest, filter, first } from "rxjs";
import { map } from "rxjs/operators";
import { UserRoleTypes } from "apps/SdHub/src/app/models/autogen/user.models";
import { untilDestroyed } from "@ngneat/until-destroy";

@Injectable()
export class AdminGuard implements CanActivate {

  constructor(private router: Router,
              private authState: AuthStateService) {
  }

  canActivate() {
    return combineLatest([this.authState.isAuthenticated$, this.authState.user$])
      .pipe(
        filter(([isAu, user]) => isAu),
        first(),
        map(([isAu, user]) => {
          const isAdmin = user!.roles.indexOf(UserRoleTypes.Admin) > -1;
          return isAdmin;
        })
      );
  }
}
