import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";

interface INavTab {
    label: string;
    link: string;
    index: number;
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
            index: 0
        },
        {
            label: 'Register',
            link: './register',
            index: 1
        },
        {
            label: 'Recover',
            link: './recover',
            index: 2
        }
    ];

    constructor(private router: Router) {
    }

    ngOnInit(): void {
    }
}
