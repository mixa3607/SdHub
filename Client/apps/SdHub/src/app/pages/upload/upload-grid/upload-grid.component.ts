import {Component, OnInit} from '@angular/core';
import {UntilDestroy, untilDestroyed} from "@ngneat/until-destroy";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {getErrorMessage} from '../../../shared/form-error-handling/handlers';
import {filter, map, Observable, of, shareReplay} from "rxjs";
import {NgxFileDropEntry} from "ngx-file-drop";
import {bytesToHuman} from "apps/SdHub/src/app/shared/utils/bytes";
import {Clipboard} from "@angular/cdk/clipboard";
import {ToastrService} from "ngx-toastr";
import {AuthStateService} from "apps/SdHub/src/app/core/services/auth-state.service";
import {HttpErrorResponse, HttpEventType, HttpResponse} from "@angular/common/http";
import {UploadApi} from "apps/SdHub/src/app/shared/services/api/upload.api";
import {httpErrorResponseHandler} from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import {IGridModel, IUploadGridResponse} from "apps/SdHub/src/app/models/autogen/grid.models";

type UploadingStatus =
  | null
  | { type: 'uploading', progress: number } // progress is a value between 0 and 100
  | { type: 'server-processing' }

interface IBlobForUpload {
  file: File | null,
  fileEntry: NgxFileDropEntry,
  name: string,
  size: number,
  sizeHuman: string,
  uploadingError?: string | null,
  uploaded: 'yes' | 'no' | 'error' | 'in_progress',
  uploadedGrid?: IGridModel,
}

@UntilDestroy()
@Component({
  selector: 'upload-grid',
  templateUrl: './upload-grid.component.html',
  styleUrls: ['./upload-grid.component.scss'],
})
export class UploadGridComponent implements OnInit {
  public uploading: boolean = false;
  public isAuthenticated: boolean = false;
  public userLogin: string | null = null;
  public uploadingStatus$: Observable<UploadingStatus> = of(null);
  public uploadToAlbum: string | null = null;
  public fileForUpload: IBlobForUpload | null = null;

  public xForm: FormGroup;
  public yForm: FormGroup;

  constructor(private formBuilder: FormBuilder,
              private clipboard: Clipboard,
              private uploadApi: UploadApi,
              private authState: AuthStateService,
              private toastr: ToastrService,) {
    this.xForm = formBuilder.group({
      xTiles: [1, Validators.compose([
        Validators.required,
        Validators.min(0),
      ])],
      xValues: ['', Validators.compose([
        Validators.required
      ])]
    });
    this.yForm = formBuilder.group({
      yTiles: [1, Validators.compose([
        Validators.required,
        Validators.min(0),
      ])],
      yValues: ['', Validators.compose([
        Validators.required
      ])]
    });
    this.authState.isAuthenticated$
      .pipe(untilDestroyed(this))
      .subscribe(x => this.isAuthenticated = x);
    this.authState.user$
      .pipe(untilDestroyed(this))
      .subscribe(x => this.userLogin = x?.login ?? null);
  }

  ngOnInit(): void {
  }

  public dropped(files: NgxFileDropEntry[]) {
    if (files.length > 1)
      this.toastr.warning('Allow only one archive file');
    files = files.filter(x => x.fileEntry.isFile);
    if (files.length == 0)
      return;

    const file = files[0];
    const fileEntry = file.fileEntry as FileSystemFileEntry;

    const blobForUpload: IBlobForUpload = {
      fileEntry: file,
      name: fileEntry.name,
      uploaded: 'no',
      size: 0,
      sizeHuman: '',
      file: null
    };
    fileEntry.file((file: File) => {
      blobForUpload.size = file.size;
      blobForUpload.sizeHuman = bytesToHuman(file.size);
      blobForUpload.file = file;
    }, err => {
      this.toastr.error('Can\'t read file');
    });
    this.fileForUpload = blobForUpload;
  }

  public onClearClick(): void {
    this.fileForUpload = null;
    this.xForm.reset();
    this.yForm.reset();
  }

  public onUploadClick(): void {
    this.uploading = true;
    const formData = new FormData();
    this.fileForUpload!.uploaded = 'in_progress';
    formData.append('file', this.fileForUpload!.file!, this.fileForUpload!.fileEntry.relativePath);
    formData.append('AlbumShortToken', this.uploadToAlbum ?? '');
    formData.append('XTiles', this.xForm.get('xTiles')?.value);
    formData.append('YTiles', this.yForm.get('yTiles')?.value);

    const xValues = (this.xForm.get('xValues')?.value as string).split(',').map(x=>x.trim());
    for (const xValue of xValues) {
      formData.append('XValues', xValue);
    }

    const yValues = (this.yForm.get('yValues')?.value as string).split(',').map(x=>x.trim());
    for (const yValue of yValues) {
      formData.append('YValues', yValue);
    }

    this.toastr.info('Begin upload grid');
    const request$ = this.uploadApi.uploadGridAuth(formData).pipe(untilDestroyed(this), shareReplay());

    this.uploadingStatus$ = request$.pipe(
      map(data => {
        if (data.type !== HttpEventType.UploadProgress)
          return null;
        if (data.loaded === data.total)
          return {type: 'server-processing'};
        return {
          type: 'uploading',
          progress: (data.loaded / data.total!) * 100
        };
      }),
    );

    request$.pipe(
      filter((data) => data.type === HttpEventType.Response),
      map((data) => (data as HttpResponse<IUploadGridResponse>).body!)
    ).subscribe((data) => {
      this.fileForUpload!.uploaded = data.uploaded ? 'yes' : "error";
      this.fileForUpload!.uploadingError = data.reason;
      this.fileForUpload!.uploadedGrid = data.grid;
      this.uploading = false;
      if (data.uploaded)
        this.toastr.success('Success upload grid');
      else
        this.toastr.error(data.reason, 'Can\'t upload');
    }, (err: HttpErrorResponse) => {
      httpErrorResponseHandler(err, this.toastr);
      this.uploading = false;
    })
  }

  public getErrorMessage(formControl: AbstractControl): string {
    return getErrorMessage(formControl);
  }
}
