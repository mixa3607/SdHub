import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {getErrorMessage} from "../../../shared/form-error-handling/handlers";
import {ToastrService} from "ngx-toastr";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {CustomValidators} from "apps/SdHub/src/app/shared/validators/customValidators";
import {UserApi} from "apps/SdHub/src/app/shared/services/api/user.api";
import {ILoginByPasswordRequest, IRegisterRequest} from "apps/SdHub/src/app/models/autogen/user.models";
import {CaptchaType} from "apps/SdHub/src/app/models/autogen/misc.models";
import {httpErrorResponseHandler} from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import {Router} from "@angular/router";


@Component({
    selector: 'app-register-form',
    templateUrl: './register-form.component.html',
    styleUrls: ['./register-form.component.scss'],
})
export class RegisterFormComponent implements OnInit {
    public form: FormGroup;
    public hidePassword = true;
    public loading = false;

    constructor(private formBuilder: FormBuilder,
                private userApi: UserApi,
                private toastr: ToastrService,
                private router: Router,) {
        this.form = this.formBuilder.group({
            login: ['', Validators.compose([
                Validators.required,
                Validators.minLength(6),
                Validators.maxLength(30),
            ])],
            email: ['', Validators.compose([
                Validators.required,
                Validators.email,
            ])],
            password: ['', Validators.compose([
                Validators.required,
                Validators.minLength(8),
                Validators.maxLength(100)
            ])],
            confirmPassword: ['', Validators.compose([
                Validators.required,
                Validators.minLength(8),
                Validators.maxLength(100)
            ])],
            recaptcha: [null, Validators.compose([
                Validators.required,
            ])],
        }, {validators: [CustomValidators.MatchingPasswords]});
    }

    ngOnInit(): void {
    }

    public onRegisterClick(): void {
        const formVal = this.form.value;
        const req: IRegisterRequest = {
            captchaType: CaptchaType.ReCaptchaV2,
            captchaCode: formVal.recaptcha as string,
            login: formVal.login as string,
            password: formVal.password as string,
            email: formVal.email as string
        }
        this.userApi.register(req).subscribe({
            next: x => {
                this.loading = false;
                this.toastr.success('Success register. Check your email for confirmation code');
                void this.router.navigate(['/auth/login'], {queryParams: {login: req.login, confirm: true}});
            },
            error: (err: HttpErrorResponse) => {
                this.loading = false;
                httpErrorResponseHandler(err, this.toastr);
            }
        });
    }

    getErrorMessage(formControl: AbstractControl): string {
        return getErrorMessage(formControl);
    }
}
