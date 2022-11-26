import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {IUploadResponse} from "apps/SdHub/src/app/models/autogen/upload.models";

@Injectable({
    providedIn: "root"
})
export class UploadApi {
    public readonly base = "/api/v1"

    public constructor(private http: HttpClient) {
    }

    public upload(form: FormData) {
        return this.http.post<IUploadResponse>(this.base + '/upload', form);
    }

    public uploadAuth(form: FormData) {
        return this.http.post<IUploadResponse>(this.base + '/uploadAuth', form);
    }
}

