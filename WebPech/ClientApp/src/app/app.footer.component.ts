import {Component} from '@angular/core';
import {AppComponent} from './app.component';

@Component({
    selector: 'app-footer',
    template: `
        <div class="layout-footer">
            <div class="footer-logo-container">
                <img id="footer-logo"   [src]="'assets/layout/images/logo-'+ (app.colorScheme === 'light' ? 'dark' : 'light') + '-footer.png'" alt="bexx-layout"/>
                <span class="app-name">Pechinchador</span>
            </div>
            <div>
                <a class="contact-icons">
                    <i class="socicon-facebook"></i>
                </a>
                <a class="contact-icons">
                    <i class="socicon-instagram"></i>
                </a>
                <a class="contact-icons">
                    <i class="socicon-twitter"></i>
                </a>
            </div>
            <span class="copyright">
                &#169; BEXX - 2021
            </span>
        </div>
        `
})
export class AppFooterComponent {
    constructor(public app: AppComponent) {}
}
