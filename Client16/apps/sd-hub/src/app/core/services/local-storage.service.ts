import {Inject, Injectable} from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  constructor(@Inject('LOCALSTORAGE') private localStorage: Storage) {
  }

  public getJwtToken(): string | null {
    return this.get('a_jwt-token');
  }

  public setJwtToken(val: string | null) {
    this.set(val, 'a_jwt-token');
  }

  public getRefreshToken(): string | null {
    return this.get('a_refresh-token');
  }

  public setRefreshToken(val: string | null) {
    this.set(val, 'a_refresh-token');
  }

  private get<T>(key: string): T | null {
    const str = this.localStorage.getItem(key);
    if (str == null)
      return null;
    return JSON.parse(str) as T;
  }

  private set<T>(val: T | null, key: string): void {
    if (val == null) {
      this.localStorage.removeItem(key);
      return;
    }
    this.localStorage.setItem(key, JSON.stringify(val));
  }
}
