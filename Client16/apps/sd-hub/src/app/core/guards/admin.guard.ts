import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { AuthStateService } from "apps/sd-hub/src/app/core/services/auth-state.service";

@Injectable()
export class AdminGuard implements CanActivate {

  constructor(private router: Router,
              private authState: AuthStateService) {
  }

  canActivate() {
    return this.authState.isAdmin$;
  }
}
