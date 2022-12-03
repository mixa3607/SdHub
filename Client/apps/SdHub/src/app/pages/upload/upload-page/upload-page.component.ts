import {Component, OnInit} from '@angular/core';
import {NgxFileDropEntry} from "ngx-file-drop";
import {DomSanitizer, SafeUrl} from "@angular/platform-browser";
import {IServerErrorResponse, IUploadedFileModel} from "../../../models/autogen/misc.models";
import {Clipboard} from "@angular/cdk/clipboard";
import {ToastrService} from "ngx-toastr";
import {HttpClient, HttpErrorResponse, HttpEventType, HttpHeaders, HttpResponse} from "@angular/common/http";
import {IUploadResponse} from "../../../models/autogen/upload.models";
import {httpErrorResponseHandler} from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import {UploadApi} from "apps/SdHub/src/app/shared/services/api/upload.api";
import {bytesToHuman} from "apps/SdHub/src/app/shared/utils/bytes";
import {AuthService} from "apps/SdHub/src/app/core/services/auth.service";
import {AuthStateService} from "apps/SdHub/src/app/core/services/auth-state.service";
import {filter, map, Observable, of, shareReplay, tap} from "rxjs";
import {UntilDestroy, untilDestroyed} from '@ngneat/until-destroy';

interface IImageForUpload {
    file: File | null,
    fileEntry: NgxFileDropEntry,
    objectUrl: SafeUrl | null,
    objectUrlError?: string | null,
    name: string,
    size: number,
    sizeHuman: string,
    uploadingError?: string | null,
    uploaded: 'yes' | 'no' | 'error' | 'in_progress',
    uploadedFile: IUploadedFileModel | null
}

type UploadingStatus =
  | null
  | { type: 'uploading', progress: number } // progress is a value between 0 and 100
  | { type: 'server-processing' }

@UntilDestroy()
@Component({
    selector: 'app-upload-page',
    templateUrl: './upload-page.component.html',
    styleUrls: ['./upload-page.component.scss']
})
export class UploadPageComponent implements OnInit {
    public uploading: boolean = false;
    public isAuthenticated: boolean = false;
    public uploadingStatus$: Observable<UploadingStatus> = of(null);

    constructor(private sanitizer: DomSanitizer,
                private clipboard: Clipboard,
                private toastr: ToastrService,
                private authState: AuthStateService,
                private uploadApi: UploadApi) {
        this.authState.isAuthenticated$.pipe(tap(x => console.log(x))).subscribe(x => this.isAuthenticated = x);
    }

    ngOnInit(): void {
    }

    public files: IImageForUpload[] = [];

    public dropped(files: NgxFileDropEntry[]) {
        for (const droppedFile of files.filter(x => x.fileEntry.isFile)) {
            const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
            const imageForUpload: IImageForUpload = {
                fileEntry: droppedFile,
                name: droppedFile.fileEntry.name,
                objectUrl: null,
                uploaded: 'no',
                size: 0,
                sizeHuman: '',
                uploadedFile: null,
                file: null
            }
            this.files.push(imageForUpload);
            fileEntry.file((file: File) => {
                console.log(file.type)
                if (file.type?.startsWith('image') != true) {
                    this.toastr.error('Only images allow!');
                    this.files = this.files.filter(x => x != imageForUpload);
                } else {
                    imageForUpload.objectUrl = this.sanitize(URL.createObjectURL(file));
                }
                imageForUpload.size = file.size;
                imageForUpload.sizeHuman = bytesToHuman(file.size);
                imageForUpload.file = file;
            }, err => {
                console.log(err);
                imageForUpload.objectUrlError = 'Cant get file from browser';
            })
        }
    }

    public sanitize(url: string): SafeUrl {
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }

    public waitUpload(): number {
        return this.files.filter(x => x.uploaded == "no").length;
    }

    public onUploadClick(): void {
        this.uploading = true;
        const formData = new FormData();
        const selectedImages: IImageForUpload[] = [];
        for (const fileF of this.files.filter(x => x.uploaded == 'no' && x.file != null)) {
            selectedImages.push(fileF);
            fileF.uploaded = 'in_progress';
            formData.append('files', fileF.file!, fileF.fileEntry.relativePath);
        }

        this.toastr.info('Begin upload ' + selectedImages.length + ' images');
        console.log(this.isAuthenticated, 'asasasa');

        const request$ = (this.isAuthenticated
            ? this.uploadApi.uploadAuth(formData)
            : this.uploadApi.upload(formData)
          ).pipe(untilDestroyed(this), shareReplay());

        this.uploadingStatus$ = request$.pipe(
          map(data => {
            if (data.type !== HttpEventType.UploadProgress) {
              return null;
            }

            if (data.loaded === data.total) {
              return { type: 'server-processing' };
            }

            return {
              type: 'uploading',
              progress: (data.loaded / data.total!) * 100
            };
          }),
        )

        request$
            .pipe(
                filter((data) => data.type === HttpEventType.Response),
                map((data) => (data as HttpResponse<IUploadResponse>).body!)
            )
            .subscribe((data) => {
                console.log(data);
                for (let i = 0; i < data.files.length; i++) {
                    const uploadedFile = data.files[i];
                    const waitUploadFile = selectedImages[i];
                    waitUploadFile.uploaded = uploadedFile.uploaded ? 'yes' : "error";
                    waitUploadFile.uploadingError = uploadedFile.reason;
                    waitUploadFile.uploadedFile = uploadedFile;
                }
                this.uploading = false;
                this.toastr.success('Success upload ' + selectedImages.filter(x => x.uploaded == 'yes').length + ' images');
            }, (err: HttpErrorResponse) => {
                httpErrorResponseHandler(err, this.toastr);
                for (let i = 0; i < selectedImages.length; i++) {
                    const waitUploadFile = selectedImages[i];
                    waitUploadFile.uploaded = "error";
                    waitUploadFile.uploadingError = err.message;
                    waitUploadFile.uploadedFile = null;
                }
                this.uploading = false;
            })
    }

    public onClearClick(): void {
        if (this.uploading)
            return;
        this.files = [];
    }

    public onCopyShortLinkClick(img: IImageForUpload): void {
        if (img?.uploadedFile?.image?.shortUrl == null)
            return;
        this.clipboard.copy(img?.uploadedFile?.image?.shortUrl);
        this.toastr.success('Short link copy to clipboard');
    }
}
