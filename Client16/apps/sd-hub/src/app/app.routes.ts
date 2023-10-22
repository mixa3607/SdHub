import { Routes } from "@angular/router";
import { AdminGuard } from "./core/guards/admin.guard";

export const appRoutes: Routes = [
  {
    path: "i",
    loadChildren: () => import("./pages/image/image.module").then(m => m.ImageModule)
  }, {
    path: "image",
    loadChildren: () => import("./pages/image/image.module").then(m => m.ImageModule)
  },
  {
    path: "a",
    loadChildren: () => import("./pages/album/album.module").then(m => m.AlbumModule)
  }, {
    path: "album",
    redirectTo: "a"
  },
  {
    path: "g",
    loadChildren: () => import("./pages/grid/grid.module").then(m => m.GridModule)
  }, {
    path: "grid",
    redirectTo: "g"
  },
  {
    path: "gen",
    loadChildren: () => import("./pages/generated/generated.module").then(m => m.GeneratedModule)
  },
  {
    path: "bin",
    loadChildren: () => import("./pages/bins/bins.module").then(m => m.BinsModule)
  },
  {
    path: "about",
    loadChildren: () => import("./pages/about/about.module").then(m => m.AboutModule)
  },
  {
    path: "auth",
    loadChildren: () => import("./pages/auth/auth.module").then(m => m.AuthModule)
  },
  {
    path: "user",
    loadChildren: () => import("./pages/user/user.module").then(m => m.UserModule)
  },
  {
    path: "upload",
    loadChildren: () => import("./pages/upload/upload.module").then(m => m.UploadModule)
  },
  {
    path: "admin",
    loadChildren: () => import("./pages/admin/admin.module").then(m => m.AdminModule),
    canActivate: [AdminGuard]
  },
  {
    path: "**",
    redirectTo: "gen",
    pathMatch: "full"
  }
];

