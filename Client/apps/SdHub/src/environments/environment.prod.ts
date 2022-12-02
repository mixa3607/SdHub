import {NgxLoggerLevel} from 'ngx-logger';
import {IFrontendSettings} from "apps/SdHub/src/app/models/autogen/misc.models";

export const environment = {
    production: true,
    logLevel: NgxLoggerLevel.TRACE,
    serverLogLevel: NgxLoggerLevel.OFF,
    clientBranch: '<not_set>',
    clientSha: '<not_set>',
    settings: {} as IFrontendSettings,
};
