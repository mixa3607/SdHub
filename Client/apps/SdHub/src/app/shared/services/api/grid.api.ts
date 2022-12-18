import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import {
  IDeleteGridRequest,
  IDeleteGridResponse,
  IEditGridRequest,
  IEditGridResponse,
  IGetGridRequest,
  IGetGridResponse, IGridModel,
  ISearchGridRequest
} from "apps/SdHub/src/app/models/autogen/grid.models";
import { IPaginationResponse } from "apps/SdHub/src/app/models/autogen/misc.models";

@Injectable({
  providedIn: "root"
})
export class GridApi {
  public readonly base = "/api/v1/grid"

  public constructor(private http: HttpClient) {
  }

  public get(req: IGetGridRequest) {
    return this.http.get<IGetGridResponse>(this.base + '/get', {
      params: req as any
    });
  }

  public delete(req: IDeleteGridRequest) {
    return this.http.post<IDeleteGridResponse>(this.base + '/delete', req);
  }

  public edit(req: IEditGridRequest) {
    return this.http.post<IEditGridResponse>(this.base + '/edit', req);
  }

  public search(req: Partial<ISearchGridRequest>) {
    return this.http.post<IPaginationResponse<IGridModel>>(this.base + '/search', req);
  }
}
