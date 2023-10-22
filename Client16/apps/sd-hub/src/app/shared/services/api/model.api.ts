import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import {
  IAddModelVersionRequest,
  ICreateModelRequest,
  IDeleteModelResponse,
  IDeleteModelVersionRequest,
  IEditModelRequest,
  IEditModelVersionRequest,
  IGetModelRequest,
  IModelModel,
  IModelVersionModel,
  ISearchModelRequest
} from "apps/sd-hub/src/app/models/autogen/model.models";
import { IPaginationResponse } from "apps/sd-hub/src/app/models/autogen/misc.models";
import { IAlbumModel, IDeleteAlbumResponse } from "apps/sd-hub/src/app/models/autogen/album.models";

@Injectable({
  providedIn: "root"
})
export class ModelApi {
  public readonly base = "/api/v1/model"

  public constructor(private http: HttpClient) {
  }

  public search(req: Partial<ISearchModelRequest>) {
    return this.http.post<IPaginationResponse<IModelModel>>(this.base + '/search', req);
  }

  public get(req: IGetModelRequest) {
    return this.http.get<IModelModel>(this.base + '/get', {
      params: req as any
    });
  }

  public create(req: ICreateModelRequest) {
    return this.http.post<IModelModel>(this.base + '/create', req);
  }

  public edit(req: IEditModelRequest) {
    return this.http.post<IAlbumModel>(this.base + '/edit', req);
  }

  public delete(req: IDeleteModelResponse) {
    return this.http.post<IDeleteAlbumResponse>(this.base + '/delete', req);
  }

  public addVersion(req: IAddModelVersionRequest) {
    return this.http.post<IModelVersionModel>(this.base + '/AddVersion', req);
  }

  public editVersion(req: IEditModelVersionRequest) {
    return this.http.post<IModelVersionModel>(this.base + '/EditVersion', req);
  }

  public deleteVersion(req: IDeleteModelVersionRequest) {
    return this.http.post(this.base + '/DeleteVersion', req);
  }
}
