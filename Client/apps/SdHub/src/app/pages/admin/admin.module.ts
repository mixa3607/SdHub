import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';
import { AdminRoutingModule } from 'apps/SdHub/src/app/pages/admin/admin-routing.module';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { AdminUsersComponent } from './admin-users/admin-users.component';
import { AdminFilesComponent } from './admin-files/admin-files.component';
import { OverlayModule } from "@angular/cdk/overlay";
import { AdminModelsComponent } from "apps/SdHub/src/app/pages/admin/admin-models/admin-models.component";

@NgModule({
  declarations: [
    AdminUsersComponent,
    AdminFilesComponent,
    AdminModelsComponent,
  ],
  imports: [CommonModule, SharedModule, AdminRoutingModule, ClipboardModule, OverlayModule],
})
export class AdminModule {}
