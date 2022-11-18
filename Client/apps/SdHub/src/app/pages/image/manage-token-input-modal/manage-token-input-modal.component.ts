import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogConfig, MatDialogRef} from "@angular/material/dialog";
import {AbstractControl, FormControl, Validators} from "@angular/forms";
import {getErrorMessage} from "apps/SdHub/src/app/shared/form-error-handling/handlers";

export interface IManageTokenInputModalData {

}

export interface IManageTokenInputModalResult {
    manageToken?: string|null;
}

@Component({
    selector: 'app-manage-token-input-modal',
    templateUrl: './manage-token-input-modal.component.html',
    styleUrls: ['./manage-token-input-modal.component.scss'],
})
export class ManageTokenInputModalComponent {
    public manageToken = new FormControl('', [Validators.required]);

    constructor(public dialogRef: MatDialogRef<ManageTokenInputModalComponent, IManageTokenInputModalResult>,
                @Inject(MAT_DIALOG_DATA) public data: IManageTokenInputModalData) {
    }

    onConfirm(): void {
        if (this.manageToken.invalid)
            return;
        this.dialogRef.close({manageToken: this.manageToken.value});
    }

    public static open(data: IManageTokenInputModalData, dialog: MatDialog) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = false;
        dialogConfig.autoFocus = true;
        dialogConfig.data = data;

        return dialog.open<ManageTokenInputModalComponent, IManageTokenInputModalData, IManageTokenInputModalResult>(ManageTokenInputModalComponent, dialogConfig);
    }

    public getErrorMessage(formControl: AbstractControl): string {
        return getErrorMessage(formControl);
    }
}
