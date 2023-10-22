import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IPaginationResponse } from "apps/sd-hub/src/app/models/autogen/misc.models";
import { IFileModel, IImportFileRequest, ISearchFileRequest } from "apps/sd-hub/src/app/models/autogen/file.models";

@Injectable({
  providedIn: "root"
})
export class FileAdminApi {
  public readonly base = "/api/v1/admin/files"

  public constructor(private http: HttpClient) {
  }

  public search(req: ISearchFileRequest) {
    return this.http.post<IPaginationResponse<IFileModel>>(this.base + '/search', req);
  }

  public import(req: IImportFileRequest) {
    return this.http.post<IFileModel>(this.base + '/import', req);
  }
}
