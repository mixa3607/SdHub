import {BrowserModule} from '@angular/platform-browser';
import {NgModule, SecurityContext} from '@angular/core';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import {AppComponent} from './app.component';
import {CoreModule} from './core/core.module';
import {SharedModule} from './shared/shared.module';
import {CustomMaterialModule} from './custom-material/custom-material.module';
import {AppRoutingModule} from './app-routing.module';
import {LoggerModule} from 'ngx-logger';
import {environment} from '../environments/environment';
import {ToastrModule} from "ngx-toastr";
import {RECAPTCHA_SETTINGS, RecaptchaSettings} from "ng-recaptcha";
import {MarkdownModule, MarkedOptions} from "ngx-markdown";
import {HttpClient} from "@angular/common/http";

console.log(environment.settings);

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        ToastrModule.forRoot({
            positionClass: 'toast-bottom-right',
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
                    mangle: true,
                },
            },
        }),
        BrowserModule,
        BrowserAnimationsModule,
        CoreModule,
        SharedModule,
        CustomMaterialModule.forRoot(),
        AppRoutingModule,
        LoggerModule.forRoot({
            serverLoggingUrl: `http://my-api/logs`,
            level: environment.logLevel,
            serverLogLevel: environment.serverLogLevel
        })
    ],
    bootstrap: [AppComponent],
    providers: [
        {
            provide: RECAPTCHA_SETTINGS,
            useFactory: () => ({siteKey: environment.settings.recaptchaSiteKey}),
        },
    ],
})
export class AppModule {
}
