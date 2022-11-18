import {Component} from '@angular/core';
import {ThemePalette} from "@angular/material/core";

export interface Task {
    name: string;
    completed: boolean | null;
    subtasks?: Task[];
}

@Component({
    selector: 'app-about-page',
    templateUrl: './about-page.component.html',
    styleUrls: ['./about-page.component.scss']
})
export class AboutPageComponent {
    public tasks: Task[] = [
        {
            name: 'Upload',
            completed: null,
            subtasks: [
                {name: 'Over site', completed: true},
                {name: 'Over api', completed: true},
                {name: 'Limits', completed: true},
                {name: 'Image grids', completed: false},
            ],
        },
        {name: 'Search', completed: false},
        {name: 'Albums', completed: false},
        {name: 'User card', completed: false},
        {name: 'Random', completed: false},
        {name: 'Better ui', completed: false},
        {name: 'Image viewer', completed: false},
        {name: 'Grids viewer', completed: false},
        {
            name: 'Detectors',
            completed: null,
            subtasks: [
                {name: 'Automatic1111', completed: true},
                {name: 'Stable diffusion', completed: true},
                {name: 'Naifu', completed: false},
            ]
        }
    ];

    constructor() {
    }

}
