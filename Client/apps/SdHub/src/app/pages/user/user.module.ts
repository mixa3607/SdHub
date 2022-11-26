import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserRoutingModule } from 'apps/SdHub/src/app/pages/user/user-routing.module';
import { UserPageComponent } from 'apps/SdHub/src/app/pages/user/user-page/user-page.component';
import { SharedModule } from '../../shared/shared.module';
import {MarkdownModule} from "ngx-markdown";

@NgModule({
  declarations: [UserPageComponent],
    imports: [
        CommonModule,
        SharedModule,
        UserRoutingModule,
        MarkdownModule
    ]
})
export class UserModule { }
