import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';

import {LayoutComponent} from '../../shared/layout/layout.component';
import {ImagePageComponent} from "./image-page/image-page.component";

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            {path: ':shortCode', component: ImagePageComponent},
            {
                path: '**',
                redirectTo: '/upload',
                pathMatch: 'full'
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ImageRoutingModule {
}
