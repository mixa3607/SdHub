import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import {
  IAddAlbumImagesRequest,
  IAddAlbumImagesResponse,
  IAlbumModel,
  ICreateAlbumRequest,
  IDeleteAlbumImagesRequest,
  IDeleteAlbumImagesResponse,
  IDeleteAlbumRequest,
  IDeleteAlbumResponse,
  IEditAlbumRequest,
  IGetAlbumRequest,
  IGetAlbumResponse,
  ISearchAlbumRequest
} from "apps/sd-hub/src/app/models/autogen/album.models";
import { IPaginationResponse } from "apps/sd-hub/src/app/models/autogen/misc.models";

@Injectable({
  providedIn: "root"
})
export class AlbumApi {
  public readonly base = "/api/v1/album"

  public constructor(private http: HttpClient) {
  }

  public search(req: Partial<ISearchAlbumRequest>) {
    return this.http.post<IPaginationResponse<IAlbumModel>>(this.base + '/search', req);
  }

  public get(req: IGetAlbumRequest) {
    return this.http.get<IGetAlbumResponse>(this.base + '/get', {
      params: req as any
    });
  }

  public create(req: ICreateAlbumRequest) {
    return this.http.post<IAlbumModel>(this.base + '/create', req);
  }

  public delete(req: IDeleteAlbumRequest) {
    return this.http.post<IDeleteAlbumResponse>(this.base + '/delete', req);
  }

  public edit(req: IEditAlbumRequest) {
    return this.http.post<IAlbumModel>(this.base + '/edit', req);
  }

  public addImages(req: IAddAlbumImagesRequest) {
    return this.http.post<IAddAlbumImagesResponse>(this.base + '/addImages', req);
  }

  public deleteImages(req: IDeleteAlbumImagesRequest) {
    return this.http.post<IDeleteAlbumImagesResponse>(this.base + '/deleteImages', req);
  }
}
