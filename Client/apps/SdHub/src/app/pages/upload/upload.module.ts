import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {SharedModule} from "../../shared/shared.module";
import {UploadRoutingModule} from "./upload-routing.module";
import { UploadPageComponent } from './upload-page/upload-page.component';
import {NgxFileDropModule} from "ngx-file-drop";



@NgModule({
  declarations: [
    UploadPageComponent
  ],
    imports: [
        CommonModule,
        SharedModule,
        UploadRoutingModule,
        NgxFileDropModule
    ]
})
export class UploadModule { }
