import { Injectable } from '@angular/core';
import { BehaviorSubject, of } from 'rxjs';
import { map, shareReplay, switchMap } from 'rxjs/operators';
import { AlbumApi } from '../../shared/services/api/album.api';
import { AuthStateService } from './auth-state.service';

@Injectable({ providedIn: 'root' })
export class MyAlbumsService {

  private reloadAlbums$ = new BehaviorSubject(null);

  private myAlbumsRes$ = this.reloadAlbums$.pipe(
    switchMap(() => this.authStateService.user$),
    switchMap((user) => user?.login
      ? this.albumsApi.search({ owner: user.login })
      : of({ albums: [], total: 0 })
    ),
    shareReplay(1)
  );

  public myAlbums$ = this.myAlbumsRes$.pipe(
    map(({ albums }) => albums),
    shareReplay(1)
  )

  public myAlbumsTotal$ = this.myAlbumsRes$.pipe(
    map(({ total }) => total),
    shareReplay(1)
  );

  public hasAlbums$ = this.myAlbumsTotal$.pipe(
    map((total) => total > 0),
    shareReplay(1)
  );

  constructor(private authStateService: AuthStateService, private albumsApi: AlbumApi) { }

  public reloadAlbums() {
    this.reloadAlbums$.next(null);
  }
}
