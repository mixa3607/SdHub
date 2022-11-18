import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import * as moment from 'moment';

import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthGuard implements CanActivate {

  constructor(private router: Router,
              private authService: AuthService) { }

  canActivate() {
    /*const user = this.authService.getCurrentUser();

    if (user && user.expiration) {

      if (moment() < moment(user.expiration)) {
        return true;
      } else {
        this.notificationService.openSnackBar('Your session has expired');
        void this.router.navigate(['auth/login']);
        return false;
      }
    }

    void this.router.navigate(['auth/login']);
    return false;*/
    return true;
  }
}
