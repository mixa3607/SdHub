import {IServerErrorResponse} from "apps/SdHub/src/app/models/autogen/misc.models";
import {ToastrService} from "ngx-toastr";
import {HttpErrorResponse} from "@angular/common/http";

export const httpErrorsDescriptions: { [code: string]: string } = {
    USER_NOT_FOUND: 'Пользователь не найден',
};

export const modelStateContainsError = (errs: { [propName: string]: string[] } | HttpErrorResponse, value: string, key: string | null = null): boolean => {
    if (errs instanceof HttpErrorResponse)
        errs = (errs.error as IServerErrorResponse)?.modelState;

    if (errs == null)
        return false;

    console.log(errs);
    for (const errKey in errs) {
        if (!errs.hasOwnProperty(errKey))
            continue;
        if (key != null && key !== errKey)
            continue;
        if (errs[errKey]?.map(x => x.split('$', 2)[0]).some(x => x === value))
            return true;
    }
    return false;
};

export const modelStateErrorToString = (error: string | null): string | '' => {
    if (error == null || error == '')
        return '';

    const nameAndArgs = error.split('$', 2);
    const name = httpErrorsDescriptions[nameAndArgs[0]] ?? nameAndArgs[0];

    if (nameAndArgs.length === 1)
        return name;

    //TODO придумать сопособ форматировать
    const args = nameAndArgs[1].split('&');
    return name;
}

const handleAnyError = (httpError: IServerErrorResponse, notificationsService: ToastrService): boolean => {
    notificationsService.error(httpError.title, httpError.message);
    return true;
};

const handle400Error = (httpError: IServerErrorResponse, notificationsService: ToastrService): boolean => {
    if (httpError.statusCode === 400) {
        if (httpError.modelState)
            showHttpModelStateErrors(httpError.modelState, 'Неверные данные', notificationsService);
        else
            showErrorCode(httpError.message, httpError.title, notificationsService);
        return true;
    }
    return false;
};

const defaultHandlersPipeline: ((httpError: IServerErrorResponse, notificationsService: ToastrService) => boolean)[] =
    [handle400Error, handleAnyError];

const showHttpModelStateErrors = (errs: { [propName: string]: string[] }, errsTitle: string, notificationsService: ToastrService): void => {
    for (let key in errs) {
        if (errs.hasOwnProperty(key)) {
            errs[key].forEach(code => {
                showErrorCode(code, errsTitle, notificationsService);
            });
        }
    }
};

const showErrorCode = (errorCode: string, title: string, notificationsService: ToastrService): void => {
    const codeStr = modelStateErrorToString(errorCode);
    notificationsService.info(codeStr, title);
};

export const httpErrorResponseHandler = (error: IServerErrorResponse | HttpErrorResponse, notificationsService: ToastrService,
                                         handlers: ((httpError: IServerErrorResponse, service: ToastrService) => boolean)[] | null = null): boolean => {
    if (error instanceof HttpErrorResponse){
        const srvErr = error.error as IServerErrorResponse ?? {};
        srvErr.statusCode ??= error.status;
        srvErr.title ??= error.name;
        srvErr.message ??= error.message;
        error = srvErr;
    }
    if (handlers == null)
        handlers = defaultHandlersPipeline;
    let handled = false;
    for (let handler of handlers) {
        handled = handler(error, notificationsService);
        if (handled)
            break;
    }
    return handled;
};