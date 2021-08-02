import {Component, OnInit} from '@angular/core';
import {AppComponent} from './app.component';
import {AppMainComponent} from './app.main.component';

@Component({
    selector: 'app-menu',
    templateUrl: './app.menu.component.html'
})
export class AppMenuComponent implements OnInit {

    public model: any[];

    constructor(public app: AppComponent, public appMain: AppMainComponent) {}

    ngOnInit() {
        this.model = [
            {
                label: 'Home', icon: 'mdi mdi-home', routerLink: ['/'],
            },
            {
                label: 'Deck', icon: 'mdi mdi-anvil',
                items: [
                    {label: 'Importar', icon: 'mdi mdi-arrow-down-thin-circle-outline', routerLink: ['/deck/importar']},
                    {label: 'Analisar', icon: 'mdi mdi-file', routerLink: ['/deck/analizar']},
                ]
            },
            {
                label: 'KPIs', icon: 'mdi mdi-chart-line', routerLink: ['/kpis'],
            },
            {
                label: 'Exclusividade', icon: 'mdi mdi-link',
                items: [
                    {label: 'Destinatário', icon: 'mdi mdi-account', routerLink: ['/exclusividade/importar']},
                    {label: 'Cidade', icon: 'mdi mdi-city', routerLink: ['/exclusividade/importar']},
                    {label: 'Região', icon: 'mdi mdi-web', routerLink: ['/exclusividade/analizar']},
                    {label: 'Cli, Faixa, UF', icon: 'mdi mdi-map', routerLink: ['/exclusividade/analizar']},
                ]
            },
            {
                label: 'Log', icon: 'mdi mdi-clipboard-edit-outline', routerLink: ['/uikit'],
            },
            {
                label: 'Utilities', icon: 'fa fa-compass', routerLink: ['utilities'],
                items: [
                    {label: 'Display', icon: 'fa fa-desktop', routerLink: ['utilities/display']},
                    {label: 'Elevation', icon: 'fa fa-link', routerLink: ['utilities/elevation']},
                    {label: 'FlexBox', icon: 'fa fa-directions', routerLink: ['utilities/flexbox']},
                    {label: 'Icons', icon: 'fa fa-meh-blank', routerLink: ['utilities/icons']},
                    {label: 'Text', icon: 'fa fa-file', routerLink: ['utilities/text']},
                    {label: 'Widgets', icon: 'fa fa-layer-group', routerLink: ['utilities/widgets']},
                    {label: 'Grid System', icon: 'fa fa-th-large', routerLink: ['utilities/grid']},
                    {label: 'Spacing', icon: 'fa fa-arrow-right', routerLink: ['utilities/spacing']},
                    {label: 'Typography', icon: 'fa fa-align-center', routerLink: ['utilities/typography']}
                ]
            }
        ];
    }
}
