import { HttpErrorResponse } from "@angular/common/http";
import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialog, MatDialogConfig, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { IAlbumModel, ICreateAlbumRequest } from "apps/SdHub/src/app/models/autogen/album.models";
import { httpErrorResponseHandler } from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import { AlbumApi } from "apps/SdHub/src/app/shared/services/api/album.api";
import { ToastrService } from "ngx-toastr";
import { MyAlbumsService } from '../../../core/services/my-albums.service';
import { getErrorMessage } from '../../../shared/form-error-handling/handlers';

export interface IAddAlbumDialogData {

}

export interface IAddAlbumDialogResult {
    createdAlbum?: IAlbumModel;
}

@Component({
    selector: 'add-album-dialog',
    templateUrl: './add-album-dialog.component.html',
    styleUrls: ['./add-album-dialog.component.scss'],
})
export class AddAlbumDialogComponent implements OnInit {
    public form: FormGroup;
    public loading = false;

    constructor(private formBuilder: FormBuilder,
                private albumApi: AlbumApi,
                private myAlbumsService: MyAlbumsService,
                private toastr: ToastrService,
                public dialogRef: MatDialogRef<AddAlbumDialogComponent, IAddAlbumDialogResult>,
                @Inject(MAT_DIALOG_DATA) public data: IAddAlbumDialogData) {
        this.form = this.formBuilder.group({
            name: ['', Validators.compose([
                Validators.required,
                Validators.maxLength(90),
            ])],
            description: ['', Validators.compose([])],
        }, {validators: []});
    }

    ngOnInit(): void {
    }

    public getErrorMessage(formControl: AbstractControl): string {
        return getErrorMessage(formControl);
    }

    public onConfirm(): void {
        this.loading = true;
        const formVal = this.form.value;
        const req: ICreateAlbumRequest = {
            name: formVal.name as string,
            description: formVal.description as string,
        }
        this.albumApi.create(req).subscribe({
            next: x => {
                this.loading = false;
                this.myAlbumsService.reloadAlbums();
                this.toastr.success('Album created');
                this.dialogRef.close({createdAlbum: x});
            },
            error: (err: HttpErrorResponse) => {
                this.loading = false;
                httpErrorResponseHandler(err, this.toastr);
            }
        });
    }

    public static open(data: IAddAlbumDialogData, dialog: MatDialog) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = false;
        dialogConfig.autoFocus = true;
        dialogConfig.data = data;

        return dialog.open<AddAlbumDialogComponent, IAddAlbumDialogData, IAddAlbumDialogResult>(AddAlbumDialogComponent, dialogConfig);
    }
}
