import {enableProdMode} from '@angular/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';
import {AppModule} from './app/app.module';
import {environment} from './environments/environment';
import {IFrontendSettings} from "apps/SdHub/src/app/models/autogen/misc.models";

(async () => {
    const response = await fetch('/api/v1/settings');
    const config = await response.json() as IFrontendSettings;

    environment.settings = config;
    console.log('Load settings from server', config);

    if (environment.production) {
        enableProdMode();
    }

    platformBrowserDynamic().bootstrapModule(AppModule)
        .catch(err => console.error(err));
})();