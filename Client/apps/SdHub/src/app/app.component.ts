import { Component } from '@angular/core';
import { AuthService } from "apps/SdHub/src/app/core/services/auth.service";

@Component({
  selector: 'app-root',
  template: `
    <router-outlet></router-outlet>`
})
export class AppComponent {
  // auth service required! this resolve trigger auth init!
  constructor(private authService: AuthService) {
  }
}
