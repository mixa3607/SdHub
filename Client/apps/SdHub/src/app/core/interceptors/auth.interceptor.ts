import {Router} from '@angular/router';
import {BehaviorSubject, catchError, filter, first, Observable, switchMap, throwError} from 'rxjs';
import {Injectable} from '@angular/core';
import {HttpErrorResponse, HttpInterceptor} from '@angular/common/http';
import {HttpRequest} from '@angular/common/http';
import {HttpHandler} from '@angular/common/http';
import {HttpEvent} from '@angular/common/http';
import {AuthStateService} from "apps/SdHub/src/app/core/services/auth-state.service";
import {LocalStorageService} from "apps/SdHub/src/app/core/services/local-storage.service";
import {ILoginByRefreshTokenRequest} from "apps/SdHub/src/app/models/autogen/user.models";
import {UserApi} from "apps/SdHub/src/app/shared/api/user.api";
import {map} from "rxjs/operators";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    private isRefreshing = false;
    private refreshTokenSubject = new BehaviorSubject<string | null>(null);

    constructor(private authState: AuthStateService,
                private router: Router,
                private localStorage: LocalStorageService,
                private userApi: UserApi,) {
    }

    private addTokenHeader(request: HttpRequest<any>, token: string): HttpRequest<any> {
        if (token != null) {
            token = "Bearer " + token;
            return request.clone({headers: request.headers.set('Authorization', token)});
        }
        return request;
    }

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (this.authState.getJwtAndRefreshToken().jwt) {
            req = this.addTokenHeader(req, this.authState.getJwtAndRefreshToken().jwt!);
        }
        return next.handle(req).pipe(catchError(error => {
            if (error instanceof HttpErrorResponse && error.headers.get("WWW-Authenticate") != null) {
                return this.handle401Error(req, next);
            }
            return throwError(error);
        }));
    }

    private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (!this.isRefreshing) {
            this.isRefreshing = true;
            this.refreshTokenSubject.next(null);
            const {refresh, jwt} = this.authState.getJwtAndRefreshToken();
            if (refresh) {
                const req: ILoginByRefreshTokenRequest = {
                    refreshToken: refresh,
                };

                return this.userApi.loginByRefreshToken(req).pipe(
                    map(r => {
                        this.authState.setJwtAndRefreshToken(r.jwtToken, r.refreshToken);
                        this.authState.setUser(r.user);

                        this.refreshTokenSubject.next(r.refreshToken);
                        this.isRefreshing = false;
                        return r;
                    }),
                    catchError((err: HttpErrorResponse) => {
                        console.error('Failure refresh user', err);
                        if (err.status === 400) {
                            this.authState.setJwtAndRefreshToken(null, null);
                        }
                        this.isRefreshing = false;
                        return throwError(err);
                    }),
                    switchMap(r => next.handle(this.addTokenHeader(request, r.jwtToken)))
                );
            }
        }
        return this.refreshTokenSubject.pipe(
            filter(token => token !== null),
            first(),
            switchMap((token) => next.handle(this.addTokenHeader(request, this.authState.getJwtAndRefreshToken().jwt!)))
        );
    }

}
