import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {
    IChangePasswordRequest,
    IChangePasswordResponse,
    IGetMeRequest,
    IGetMeResponse,
    ILoginByPasswordRequest,
    ILoginByRefreshTokenRequest,
    ILoginResponse,
    IRegisterRequest,
    IRegisterResponse
} from "apps/SdHub/src/app/models/autogen/user.models";

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

    public register(req: IRegisterRequest) {
        return this.http.post<IRegisterResponse>(this.base + '/register', req);
    }

    public loginByPassword(req: ILoginByPasswordRequest) {
        return this.http.post<ILoginResponse>(this.base + '/loginByPassword', req);
    }

    public loginByRefreshToken(req: ILoginByRefreshTokenRequest) {
        return this.http.post<ILoginResponse>(this.base + '/loginByRefreshToken', req);
    }

    public changePassword(req: IChangePasswordRequest) {
        return this.http.post<IChangePasswordResponse>(this.base + '/changePassword', req);
    }
}