import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {
    IConfirmEmailRequest, IConfirmEmailResponse, IEditUserRequest, IEditUserResponse,
    IGetMeRequest,
    IGetMeResponse, IGetUserRequest, IGetUserResponse,
    ILoginByPasswordRequest,
    ILoginByRefreshTokenRequest,
    ILoginResponse,
    IRegisterRequest,
    IRegisterResponse,
    IResetPasswordRequest,
    IResetPasswordResponse, ISendEmailConfirmationEmailRequest, ISendEmailConfirmationEmailResponse,
    ISendResetPasswordEmailRequest,
    ISendResetPasswordEmailResponse
} from "apps/sd-hub/src/app/models/autogen/user.models";

@Injectable({
    providedIn: "root"
})
export class UserApi {
    public readonly base = "/api/v1/user"

    public constructor(private http: HttpClient) {
    }

    public getMe(req: IGetMeRequest) {
        return this.http.get<IGetMeResponse>(this.base + '/getMe', {
            params: req as any
        });
    }

    public get(req: IGetUserRequest) {
        return this.http.get<IGetUserResponse>(this.base + '/get', {
            params: req as any
        });
    }

    public edit(req: IEditUserRequest) {
        return this.http.post<IEditUserResponse>(this.base + '/edit', req);
    }

    public register(req: IRegisterRequest) {
        return this.http.post<IRegisterResponse>(this.base + '/register', req);
    }

    public loginByPassword(req: ILoginByPasswordRequest) {
        return this.http.post<ILoginResponse>(this.base + '/loginByPassword', req);
    }

    public loginByRefreshToken(req: ILoginByRefreshTokenRequest) {
        return this.http.post<ILoginResponse>(this.base + '/loginByRefreshToken', req);
    }

    public sendEmailConfirmationEmail(req: ISendEmailConfirmationEmailRequest) {
        return this.http.post<ISendEmailConfirmationEmailResponse>(this.base + '/sendEmailConfirmationEmail', req);
    }

    public confirmEmail(req: IConfirmEmailRequest) {
        return this.http.post<IConfirmEmailResponse>(this.base + '/confirmEmail', req);
    }

    public sendResetPasswordEmail(req: ISendResetPasswordEmailRequest) {
        return this.http.post<ISendResetPasswordEmailResponse>(this.base + '/sendResetPasswordEmail', req);
    }

    public resetPassword(req: IResetPasswordRequest) {
        return this.http.post<IResetPasswordResponse>(this.base + '/resetPassword', req);
    }
}
