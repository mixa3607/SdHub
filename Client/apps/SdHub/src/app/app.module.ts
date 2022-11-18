import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
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

const globalSettings: RecaptchaSettings = {siteKey: '6LdMtMYiAAAAAMYBK_wIrw0b-65U5jGKkW9jGSoW'};

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        ToastrModule.forRoot({
            positionClass: 'toast-bottom-right',
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
            useValue: globalSettings,
        },
    ],
})
export class AppModule {
}
