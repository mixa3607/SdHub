import { Component } from "@angular/core";
import { AppConfigService } from "../../../shared/services/app-config.service";

interface INavTab {
  label: string;
  link: string;
}

@Component({
  selector: "app-auth-page",
  templateUrl: "./auth-page.component.html",
  styleUrls: ["./auth-page.component.scss"]
})
export class AuthPageComponent {
  public navTabs: INavTab[] = [];

  constructor(cfgServide: AppConfigService) {
    this.navTabs.push(
      ...[
        {
          label: "Login",
          link: "./login"
        },
        ...(cfgServide.config.disableUsersRegistration
          ? []
          : [
            {
              label: "Register",
              link: "./register"
            }
          ]),
        {
          label: "Recover",
          link: "./recover"
        }
      ]
    );
  }
}
