import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ToastrService} from "ngx-toastr";
import {getErrorMessage} from "../../../shared/form-error-handling/handlers";
import {ILoginByPasswordRequest} from "apps/SdHub/src/app/models/autogen/user.models";
import {CaptchaType} from "apps/SdHub/src/app/models/autogen/misc.models";
import {AuthService} from "apps/SdHub/src/app/core/services/auth.service";
import {HttpErrorResponse} from "@angular/common/http";
import {Router} from "@angular/router";
import {httpErrorResponseHandler} from "apps/SdHub/src/app/shared/http-error-handling/handlers";

@Component({
    selector: 'app-login-form',
    templateUrl: './login-form.component.html',
    styleUrls: ['./login-form.component.scss'],
})
export class LoginFormComponent implements OnInit {
    public form: FormGroup;
    public hidePassword = true;
    public loading = false;

    constructor(private formBuilder: FormBuilder,
                private toastr: ToastrService,
                private router: Router,
                private authService: AuthService) {
        this.form = this.formBuilder.group({
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
            recaptcha: [null, Validators.compose([
                Validators.required,
            ])],
        }, {validators: []});
    }

    public ngOnInit(): void {
    }

    public getErrorMessage(formControl: AbstractControl): string {
        return getErrorMessage(formControl);
    }

    public onSubmit(): void {
        this.loading = true;
        const formVal = this.form.value;
        const req: ILoginByPasswordRequest = {
            captchaType: CaptchaType.ReCaptchaV2,
            login: formVal.login as string,
            password: formVal.password as string,
            captchaCode: formVal.recaptcha as string,
        }
        formVal.password = '';
        formVal.recaptcha = null;
        this.form.setValue(formVal);
        this.authService.login(req).subscribe({
            next: x => {
                this.loading = false;
                this.toastr.success('Login as ' + x.login);
                void this.router.navigate(['/']);
            },
            error: (err: HttpErrorResponse) => {
                this.loading = false;
                httpErrorResponseHandler(err, this.toastr);
            }
        })
    }
}
