import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from 'apps/SdHub/src/app/pages/auth/auth-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { AuthPageComponent } from './auth-page/auth-page.component';
import { LoginFormComponent } from './login-form/login-form.component';
import { RegisterFormComponent } from './register-form/register-form.component';
import { RecaptchaFormsModule, RecaptchaModule } from 'ng-recaptcha';
import { RecoverFormComponent } from './recover-form/recover-form.component';

@NgModule({
  declarations: [
    AuthPageComponent,
    LoginFormComponent,
    RegisterFormComponent,
    RecoverFormComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    AuthRoutingModule,
    RecaptchaModule,
    RecaptchaFormsModule,
  ],
})
export class AuthModule {}
