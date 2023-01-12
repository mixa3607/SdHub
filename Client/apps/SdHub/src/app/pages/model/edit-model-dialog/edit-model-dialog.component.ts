import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogConfig, MatDialogRef } from "@angular/material/dialog";
import { IModelModel } from "apps/SdHub/src/app/models/autogen/model.models";
import { ModelApi } from "apps/SdHub/src/app/shared/services/api/model.api";

export interface IEditModelData {
  model: IModelModel;
}

export interface IEditModelResult {
  model: IModelModel;
}

@Component({
  selector: 'edit-model-dialog',
  templateUrl: './edit-model-dialog.component.html',
  styleUrls: ['./edit-model-dialog.component.scss'],
})
export class EditModelDialogComponent {
  constructor(public dialogRef: MatDialogRef<EditModelDialogComponent, IEditModelResult>,
              private modelApi: ModelApi,
              @Inject(MAT_DIALOG_DATA) public data: IEditModelData) {
  }


  public onSaveClick(): void {

  }


  public static open(data: IEditModelData, dialog: MatDialog) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = true;
    dialogConfig.data = data;

    return dialog.open<EditModelDialogComponent, IEditModelData, IEditModelResult>(EditModelDialogComponent, dialogConfig);
  }
}
