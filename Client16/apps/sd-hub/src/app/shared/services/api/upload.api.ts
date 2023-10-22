import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {IUploadResponse} from "apps/sd-hub/src/app/models/autogen/upload.models";
import { IUploadGridCheckInputRequest, IUploadGridResponse } from "apps/sd-hub/src/app/models/autogen/grid.models";

@Injectable({
  providedIn: "root"
})
export class UploadApi {
  public readonly base = "/api/v1"

  public constructor(private http: HttpClient) {
  }

  public upload(form: FormData) {
    return this.http.post<IUploadResponse>(this.base + '/upload', form, {reportProgress: true, observe: 'events'});
  }

  public uploadAuth(form: FormData) {
    return this.http.post<IUploadResponse>(this.base + '/uploadAuth', form, {reportProgress: true, observe: 'events'});
  }

  public uploadGridAuth(form: FormData) {
    return this.http.post<IUploadGridResponse>(this.base + '/uploadGridAuth', form, {reportProgress: true, observe: 'events'});
  }

  public uploadGridCheckInput(req: IUploadGridCheckInputRequest) {
    return this.http.post(this.base + '/UploadGridCheckInput', req);
  }
}

