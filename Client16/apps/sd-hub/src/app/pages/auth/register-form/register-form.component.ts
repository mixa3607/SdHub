import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { getErrorMessage } from '../../../shared/form-error-handling/handlers';
import { ToastrService } from 'ngx-toastr';
import { HttpErrorResponse } from '@angular/common/http';
import { CustomValidators } from 'apps/sd-hub/src/app/shared/validators/customValidators';
import { UserApi } from 'apps/sd-hub/src/app/shared/services/api/user.api';
import { IRegisterRequest } from 'apps/sd-hub/src/app/models/autogen/user.models';
import { CaptchaType } from 'apps/sd-hub/src/app/models/autogen/misc.models';
import { httpErrorResponseHandler } from 'apps/sd-hub/src/app/shared/http-error-handling/handlers';
import { Router } from '@angular/router';
import { AppConfigService } from '../../../shared/services/app-config.service';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.scss'],
})
export class RegisterFormComponent implements OnInit {
  public form: FormGroup;
  public hidePassword = true;
  public loading = false;
  public captchaDisabled = false;

  constructor(
    private formBuilder: FormBuilder,
    private userApi: UserApi,
    private toastr: ToastrService,
    private router: Router,
    cfgServide: AppConfigService
  ) {
    this.captchaDisabled = cfgServide.config.disableCaptcha;
    this.form = this.formBuilder.group(
      {
        login: [
          '',
          Validators.compose([
            Validators.required,
            Validators.minLength(6),
            Validators.maxLength(30),
          ]),
        ],
        email: [
          '',
          Validators.compose([Validators.required, Validators.email]),
        ],
        password: [
          '',
          Validators.compose([
            Validators.required,
            Validators.minLength(8),
            Validators.maxLength(100),
          ]),
        ],
        confirmPassword: [
          '',
          Validators.compose([
            Validators.required,
            Validators.minLength(8),
            Validators.maxLength(100),
          ]),
        ],
        recaptcha: this.captchaDisabled
          ? undefined
          : [null, Validators.compose([Validators.required])],
      },
      { validators: [CustomValidators.MatchingPasswords] }
    );
  }

  ngOnInit(): void {}

  public onRegisterClick(): void {
    const formVal = this.form.value;
    const req: IRegisterRequest = {
      captchaType: CaptchaType.ReCaptchaV2,
      captchaCode: formVal.recaptcha as string,
      login: formVal.login as string,
      password: formVal.password as string,
      email: formVal.email as string,
    };
    this.userApi.register(req).subscribe({
      next: (x) => {
        this.loading = false;
        this.toastr.success(
          'Success register. Check your email for confirmation code'
        );
        void this.router.navigate(['/auth/login'], {
          queryParams: { login: req.login, confirm: true },
        });
      },
      error: (err: HttpErrorResponse) => {
        this.loading = false;
        httpErrorResponseHandler(err, this.toastr);
      },
    });
  }

  getErrorMessage(formControl: AbstractControl): string {
    return getErrorMessage(formControl);
  }
}
