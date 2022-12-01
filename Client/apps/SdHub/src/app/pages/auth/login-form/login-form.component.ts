import {Component, OnInit, ViewChild} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ToastrService} from "ngx-toastr";
import {getErrorMessage} from "../../../shared/form-error-handling/handlers";
import {
    IConfirmEmailRequest,
    ILoginByPasswordRequest, ISendEmailConfirmationEmailRequest,
} from "apps/SdHub/src/app/models/autogen/user.models";
import {CaptchaType} from "apps/SdHub/src/app/models/autogen/misc.models";
import {AuthService} from "apps/SdHub/src/app/core/services/auth.service";
import {HttpErrorResponse} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {
    httpErrorResponseHandler,
    modelStateContainsError
} from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import {UserApi} from "apps/SdHub/src/app/shared/services/api/user.api";
import {RecaptchaComponent} from "ng-recaptcha";
import * as stream from "stream";
import {environment} from "apps/SdHub/src/environments/environment";

@Component({
    selector: 'app-login-form',
    templateUrl: './login-form.component.html',
    styleUrls: ['./login-form.component.scss'],
})
export class LoginFormComponent implements OnInit {
    public loginForm: FormGroup;
    public emailConfirmForm: FormGroup;
    public hidePassword = true;
    public loading = false;
    public isEmailConfirmationMode = false;
    public sendConfirmationCodeRequired = false;
    public captchaDisabled = environment.settings.disableCaptcha;

    @ViewChild('captchaElem', {static: false}) captchaElem?: RecaptchaComponent;

    constructor(private formBuilder: FormBuilder,
                private toastr: ToastrService,
                private router: Router,
                private userApi: UserApi,
                private authService: AuthService,
                private route: ActivatedRoute) {
        this.loginForm = this.formBuilder.group({
            login: ['', Validators.compose([
                Validators.required,
                Validators.minLength(5),
                Validators.maxLength(30),
            ])],
            password: ['', Validators.compose([
                Validators.required,
                Validators.minLength(8),
                Validators.maxLength(100)
            ])],
            recaptcha: this.captchaDisabled
                ? undefined : [null, Validators.compose([
                    Validators.required,
                ])],
        }, {validators: []});

        this.emailConfirmForm = this.formBuilder.group({
            login: ['', Validators.compose([
                Validators.required,
                Validators.minLength(5),
                Validators.maxLength(30),
            ])],
            code: ['', Validators.compose([
                Validators.required,
            ])],
        }, {validators: []});

        this.route.queryParams.subscribe(params => {
            if (typeof params['login'] === 'string') {
                this.loginForm.controls['login'].setValue(params['login']);
                this.emailConfirmForm.controls['login'].setValue(params['login']);
            }
            if (params['confirm'] === 'true') {
                this.isEmailConfirmationMode = true;
            }
        });
    }

    public ngOnInit(): void {
    }

    public getErrorMessage(formControl: AbstractControl): string {
        return getErrorMessage(formControl);
    }

    public onLoginClick(): void {
        this.loading = true;
        const formVal = this.loginForm.value;

        if (!this.sendConfirmationCodeRequired) {
            const req: ILoginByPasswordRequest = {
                captchaType: CaptchaType.ReCaptchaV2,
                login: formVal.login as string,
                password: formVal.password as string,
                captchaCode: formVal.recaptcha as string,
            }
            this.authService.login(req).subscribe({
                next: x => {
                    this.loading = false;
                    this.toastr.success('Login as ' + x.login);
                    void this.router.navigate(['/']);
                },
                error: (err: HttpErrorResponse) => {
                    this.loading = false;
                    httpErrorResponseHandler(err, this.toastr);

                    this.captchaElem?.reset();
                    if (modelStateContainsError(err, 'EMAIL_NOT_CONFIRMED')) {
                        this.sendConfirmationCodeRequired = true;
                        this.emailConfirmForm.controls['login'].setValue(req.login);
                    }
                }
            })
        } else {
            const req: ISendEmailConfirmationEmailRequest = {
                captchaType: CaptchaType.ReCaptchaV2,
                login: formVal.login as string,
                captchaCode: formVal.recaptcha as string,
            }
            this.userApi.sendEmailConfirmationEmail(req).subscribe({
                next: x => {
                    this.loading = false;
                    this.sendConfirmationCodeRequired = false;
                    this.isEmailConfirmationMode = true;
                    this.toastr.success('Check your email');
                },
                error: (err: HttpErrorResponse) => {
                    this.loading = false;
                    httpErrorResponseHandler(err, this.toastr);
                }
            })
        }
    }

    public onEmailConfirmClick(): void {
        this.loading = true;
        const formVal = this.emailConfirmForm.value;
        const req: IConfirmEmailRequest = {
            login: formVal.login as string,
            code: formVal.code as string,
        }
        this.userApi.confirmEmail(req).subscribe({
            next: x => {
                this.loading = false;
                this.isEmailConfirmationMode = false;
                this.toastr.success('Email confirmed. Now you can login');
            },
            error: (err: HttpErrorResponse) => {
                this.loading = false;
                httpErrorResponseHandler(err, this.toastr);
            }
        })
    }
}
