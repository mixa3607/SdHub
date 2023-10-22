import { BrowserModule } from "@angular/platform-browser";
import { APP_INITIALIZER, isDevMode, NgModule, SecurityContext } from "@angular/core";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { AppComponent } from "./app.component";
import { CoreModule } from "./core/core.module";
import { SharedModule } from "./shared/shared.module";
import { CustomMaterialModule } from "./custom-material/custom-material.module";
import { INGXLoggerConfig, LoggerModule, NgxLoggerLevel, TOKEN_LOGGER_CONFIG } from "ngx-logger";
import { ToastrModule } from "ngx-toastr";
import { RECAPTCHA_SETTINGS } from "ng-recaptcha";
import { MarkdownModule, MarkedOptions } from "ngx-markdown";
import { HttpClient } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import { appRoutes } from "./app.routes";
import { AppConfigService } from "./shared/services/app-config.service";
import { lastValueFrom } from "rxjs";
import { RecaptchaSettings } from "ng-recaptcha/lib/recaptcha-settings";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    RouterModule.forRoot(appRoutes, { initialNavigation: "enabledBlocking" }),
    ToastrModule.forRoot({
      positionClass: "toast-bottom-right"
    }),
    MarkdownModule.forRoot({
      loader: HttpClient,
      sanitize: SecurityContext.NONE,
      markedOptions: {
        provide: MarkedOptions,
        useValue: {
          gfm: true,
          breaks: true,
          pedantic: false,
          smartLists: true,
          smartypants: false,
          mangle: true
        }
      }
    }),
    BrowserModule,
    BrowserAnimationsModule,
    CoreModule,
    SharedModule,
    CustomMaterialModule.forRoot(),
    LoggerModule.forRoot({} as INGXLoggerConfig)
  ],
  bootstrap: [AppComponent],
  providers: [
    {
      provide: APP_INITIALIZER,
      multi: true,
      useFactory: (cfg: AppConfigService) => () => cfg.initialize(),
      deps: [AppConfigService]
    },
    {
      provide: TOKEN_LOGGER_CONFIG,
      useFactory: () => ({
        level: isDevMode() ? NgxLoggerLevel.LOG : NgxLoggerLevel.INFO
      } as INGXLoggerConfig)
    },
    {
      provide: RECAPTCHA_SETTINGS,
      useFactory: (cfg: AppConfigService) => ({
        siteKey: cfg.config.recaptchaSiteKey
      } as RecaptchaSettings),
      deps: [AppConfigService]
    }
  ]
})
export class AppModule {
}
