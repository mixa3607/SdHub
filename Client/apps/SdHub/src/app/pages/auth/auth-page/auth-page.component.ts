import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {environment} from "apps/SdHub/src/environments/environment";

interface INavTab {
    label: string;
    link: string;
}

@Component({
    selector: 'app-auth-page',
    templateUrl: './auth-page.component.html',
    styleUrls: ['./auth-page.component.scss'],
})
export class AuthPageComponent implements OnInit {
    public navTabs: INavTab[] = [
        {
            label: 'Login',
            link: './login',
        },
        ...environment.settings.disableUsersRegistration
            ? [] : [{
                label: 'Register',
                link: './register',
            }],
        {
            label: 'Recover',
            link: './recover',
        }
    ];

    constructor(private router: Router) {
    }

    ngOnInit(): void {
    }
}
