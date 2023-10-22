import {HttpClient} from "@angular/common/http";
import {Injectable} from "@angular/core";
import {IFrontendSettings} from "../../models/autogen/misc.models";
import {Observable} from "rxjs";
import {tap} from "rxjs/operators";


@Injectable({
  providedIn: "root"
})
export class AppConfigService {
  private _config: IFrontendSettings | null = null;

  public get config(): IFrontendSettings {
    return this._config!;
  }

  public constructor(private http: HttpClient) {

  }

  public initialize(): Observable<unknown> {
    return this.getConfig().pipe(tap(x => this._config = x));
  }

  private getConfig() {
    return this.http.get<IFrontendSettings>('/api/v1/settings/get');
  }
}
