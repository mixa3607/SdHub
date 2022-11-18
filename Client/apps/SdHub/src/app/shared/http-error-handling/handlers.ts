import {IServerErrorResponse} from "apps/SdHub/src/app/models/autogen/misc.models";
import {ToastrService} from "ngx-toastr";
import {HttpErrorResponse} from "@angular/common/http";

// все возможные коды ошибок из ModelState. Используется в методе showErrorCode.
// Сами ошибки генерируются на бэке в контрукции `ModelState.AddModelError("something", "<ERROR_CODE>"); return BadRequest(ModelState);`
// Если в списке её нет то выведется как есть
export const httpErrorsDescriptions: { [code: string]: string } = {
    USER_NOT_FOUND: 'Пользователь не найден',
    CALCULATION_GUID_REQUIRED: 'Треубется указать GUID расчёта',
    CALCULATION_NOT_FOUND: 'Пустой расчёт',
    NOT_OWNER_OF_CALCULATION: 'Вы не являетесь владельцем расчёта',
    LOT_NOT_FOUND: 'Лот не найден',
    LOT_FORBIDDEN: 'Не являетесь владельцем лота'
};

export const modelStateContainsError = (errs: { [propName: string]: string[] }, value: string, key: string|null = null): boolean => {
    for (const errKey in errs) {
        if (!errs.hasOwnProperty(errKey))
            continue;
        if (key != null && key !== errKey)
            continue;
        if (errs[errKey]?.some(x => x === value))
            return true;
    }
    return false;
};

export const modelStateErrorToString = (error: string|null): string|'' =>{
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
        if(httpError.modelState)
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

export const httpErrorResponseHandler = (error: IServerErrorResponse|HttpErrorResponse, notificationsService: ToastrService,
                                         handlers: ((httpError: IServerErrorResponse, service: ToastrService) => boolean)[]|null = null): boolean => {
    if (error instanceof HttpErrorResponse)
        error = error.error as IServerErrorResponse;
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