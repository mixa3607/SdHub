import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { combineLatest, takeUntil } from "rxjs";
import { AuthStateService } from "apps/SdHub/src/app/core/services/auth-state.service";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { IUserModel } from "apps/SdHub/src/app/models/autogen/misc.models";
import { AuthService } from "apps/SdHub/src/app/core/services/auth.service";
import { environment } from "apps/SdHub/src/environments/environment";
import { BreakpointObserver, MediaMatcher } from "@angular/cdk/layout";
import { MatSidenav } from "@angular/material/sidenav";
import { UserRoleTypes } from "apps/SdHub/src/app/models/autogen/user.models";

@UntilDestroy()
@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSidenav, {static: true}) sidenav!: MatSidenav;
  public isMobile = false;
  public isAdmin = false;
  public isExpandAdmin = false;

  public userName: string = 'Anon';
  public isAnonymous = true;
  public userInfoIsOpen = false;
  public user: IUserModel | null = null;
  public registrationEnabled = !environment.settings.disableUsersRegistration;

  constructor(
    private observer: BreakpointObserver,
    private media: MediaMatcher,
    public authService: AuthService,
    public authStateService: AuthStateService,) {

    combineLatest([authStateService.isAuthenticated$, this.authStateService.user$])
      .pipe(untilDestroyed(this))
      .subscribe(([isAu, user]) => {
        if (isAu && user != null){
          this.userName = user.login;
          this.isAdmin = user.roles.indexOf(UserRoleTypes.Admin) != -1;
        }
        else if (!isAu)
          this.userName = "Anon";
        else
          this.userName = "Loading...";
        this.isAnonymous = !isAu;
        this.user = user;
      });
  }

  ngOnInit(): void {
    this.observer.observe(['(max-width: 1000px)']).subscribe((res) => {
      this.isMobile = res.matches;
      if (this.isMobile) {
        this.sidenav.mode = 'over';
        void this.sidenav.close();
      } else {
        this.sidenav.mode = 'side';
        void this.sidenav.open();
      }
    });
  }

  public toggleNav(): void{
    this.sidenav?.toggle();
  }

  ngAfterViewInit() {
  }

  public onLogoutClick(): void {
    this.authService.logout();
  }
}
