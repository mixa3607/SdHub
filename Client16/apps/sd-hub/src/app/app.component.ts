import { Component } from '@angular/core';
import { AuthService } from "apps/sd-hub/src/app/core/services/auth.service";

@Component({
  selector: 'sd-hub-root',
  template: `
    <router-outlet></router-outlet>`
})
export class AppComponent {
  // auth service required! this resolve trigger auth init!
  constructor(private authService: AuthService) {
  }
}
