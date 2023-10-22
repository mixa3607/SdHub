import { Injectable } from '@angular/core';
import { BehaviorSubject, of, pipe } from 'rxjs';
import { map, shareReplay, switchMap, tap } from 'rxjs/operators';
import { AlbumApi } from '../../shared/services/api/album.api';
import { AuthStateService } from './auth-state.service';
import { ToastrService } from "ngx-toastr";
import { httpErrorResponseHandler } from "apps/sd-hub/src/app/shared/http-error-handling/handlers";

@Injectable({providedIn: 'root'})
export class MyAlbumsService {

  private reloadAlbums$ = new BehaviorSubject(null);

  private myAlbumsRes$ = this.reloadAlbums$.pipe(
    switchMap(() => this.authStateService.user$),
    switchMap((user) => user?.login
      ? this.albumsApi.search({owner: user.login})
      : of({items: [], total: 0})
    ),
    tap({error: err => httpErrorResponseHandler(err, this.toastr)}),
    shareReplay(1)
  );

  public myAlbums$ = this.myAlbumsRes$.pipe(
    map(({items}) => items),
    shareReplay(1)
  )

  public myAlbumsTotal$ = this.myAlbumsRes$.pipe(
    map(({total}) => total),
    shareReplay(1)
  );

  public hasAlbums$ = this.myAlbumsTotal$.pipe(
    map((total) => total > 0),
    shareReplay(1)
  );

  constructor(private authStateService: AuthStateService,
              private albumsApi: AlbumApi,
              private toastr: ToastrService,) {
  }

  public reloadAlbums() {
    this.reloadAlbums$.next(null);
  }
}
