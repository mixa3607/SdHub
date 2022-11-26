import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {AuthPageComponent} from "apps/SdHub/src/app/pages/auth/auth-page/auth-page.component";
import {LoginFormComponent} from "apps/SdHub/src/app/pages/auth/login-form/login-form.component";
import {RegisterFormComponent} from "apps/SdHub/src/app/pages/auth/register-form/register-form.component";
import {RecoverFormComponent} from "apps/SdHub/src/app/pages/auth/recover-form/recover-form.component";
import {LayoutComponent} from "apps/SdHub/src/app/shared/components/layout/layout.component";

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            {
                path: '',
                component: AuthPageComponent,
                children: [
                    {
                        path: 'login',
                        component: LoginFormComponent
                    }, {
                        path: 'register',
                        component: RegisterFormComponent
                    }, {
                        path: 'recover',
                        component: RecoverFormComponent
                    }, {
                        path: '**',
                        redirectTo: 'login'
                    }
                ]
            }
        ]
    }

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AuthRoutingModule {
}
