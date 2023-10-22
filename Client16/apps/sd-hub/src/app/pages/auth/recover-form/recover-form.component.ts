import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {getErrorMessage} from '../../../shared/form-error-handling/handlers';
import {UserApi} from "apps/sd-hub/src/app/shared/services/api/user.api";
import {ToastrService} from "ngx-toastr";
import {Router} from "@angular/router";
import {CustomValidators} from "apps/sd-hub/src/app/shared/validators/customValidators";
import {
  IResetPasswordRequest,
  ISendResetPasswordEmailRequest
} from "apps/sd-hub/src/app/models/autogen/user.models";
import {CaptchaType} from "apps/sd-hub/src/app/models/autogen/misc.models";
import {HttpErrorResponse} from "@angular/common/http";
import {httpErrorResponseHandler} from "apps/sd-hub/src/app/shared/http-error-handling/handlers";
import {AppConfigService} from "../../../shared/services/app-config.service";

@Component({
  selector: 'app-recover-form',
  templateUrl: './recover-form.component.html',
  styleUrls: ['./recover-form.component.scss'],
})
export class RecoverFormComponent {
  public resetForm: FormGroup;
  public sendCodeForm: FormGroup;
  public hidePassword = true;
  public loading = false;
  public isSendingCodeMode = true;
  public captchaDisabled = false;

  constructor(private formBuilder: FormBuilder,
              private userApi: UserApi,
              private toastr: ToastrService,
              private router: Router,
              cfgServide: AppConfigService) {
    this.captchaDisabled = cfgServide.config.disableCaptcha;

    this.resetForm = this.formBuilder.group({
      login: ['', Validators.compose([
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(30),
      ])],
      code: ['', Validators.compose([
        Validators.required,
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
    }, {validators: [CustomValidators.MatchingPasswords]});
    this.sendCodeForm = this.formBuilder.group({
      login: ['', Validators.compose([
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(30),
      ])],
      recaptcha: this.captchaDisabled
        ? undefined : [null, Validators.compose([
          Validators.required,
        ])],
    });
  }

  public onResetClick(): void {
    const formVal = this.resetForm.value;
    const req: IResetPasswordRequest = {
      login: formVal.login as string,
      code: formVal.code as string,
      newPassword: formVal.password as string,
    }
    this.userApi.resetPassword(req).subscribe({
      next: x => {
        this.loading = false;
        this.toastr.success('Success. Now you can login with new password!');
        void this.router.navigate(['/auth/login'], {queryParams: {login: req.login}});
      },
      error: (err: HttpErrorResponse) => {
        this.loading = false;
        httpErrorResponseHandler(err, this.toastr);
      }
    });
  }

  public onSendCodeClick(): void {
    const formVal = this.sendCodeForm.value;
    const req: ISendResetPasswordEmailRequest = {
      captchaType: CaptchaType.ReCaptchaV2,
      captchaCode: formVal.recaptcha as string,
      login: formVal.login as string,
    }
    this.userApi.sendResetPasswordEmail(req).subscribe({
      next: x => {
        this.loading = false;
        this.toastr.success('Success. Check your email for confirmation code');
        this.resetForm.controls['login'].setValue(req.login);
        this.isSendingCodeMode = false;
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
