import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import {
  ICheckManageTokenRequest,
  ICheckManageTokenResponse,
  IDeleteImageRequest,
  IDeleteImageResponse,
  IEditImageRequest,
  IEditImageResponse,
  IGetImageRequest,
  IGetImageResponse,
  ISearchImageRequest,
  ISearchImageResponse
} from "apps/SdHub/src/app/models/autogen/image.models";

@Injectable({
    providedIn: "root"
})
export class ImageApi {
    public readonly base = "/api/v1/image"

    public constructor(private http: HttpClient) {
    }

    public checkManageToken(req: ICheckManageTokenRequest) {
        return this.http.get<ICheckManageTokenResponse>(this.base + '/checkManageToken', {
            params: req as any
        });
    }

    public get(req: IGetImageRequest) {
        return this.http.get<IGetImageResponse>(this.base + '/get', {
            params: req as any
        });
    }

    public delete(req: IDeleteImageRequest) {
        return this.http.post<IDeleteImageResponse>(this.base + '/delete', req);
    }

    public edit(req: IEditImageRequest) {
        return this.http.post<IEditImageResponse>(this.base + '/edit', req);
    }

    public search(req: Partial<ISearchImageRequest>) {
        return this.http.post<ISearchImageResponse>(this.base + '/search', req);
    }
}

