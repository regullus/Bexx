import { Component, Inject, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription, Observable, of, Subject, throwError, BehaviorSubject } from 'rxjs';
import { first } from 'rxjs/operators';
import { UserModel } from '../../_models/user.model';
import { AuthService } from '../../_services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ResponseModel } from './../../_models/resoponse';
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map, catchError, finalize, tap } from 'rxjs/operators';

@Component({
  selector: 'app-login-externo',
  templateUrl: './login-externo.component.html',
  styleUrls: ['./login-externo.component.scss']
})

export class LoginExternoComponent implements OnInit {
  isLoginExterno: boolean = true;
  isAlthenticated: boolean = false;
  hasError: boolean;
  isLoading$: Observable<boolean>;
  returnUrl: string;
  // private fields
  private unsubscribe: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    @Inject(DOCUMENT) private document: Document, private httpClient: HttpClient,     
    private http: HttpClient
  ) { 
    this.versao();
    this.isLoading$ = this.authService.isLoading$;
    // redirect to home if already logged in
    if (this.authService.currentUserValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit(): void {
    if(this.isAuthenticated()) {
      console.log("ok")    
    }
    else {
      const url = `${environment.loginURL}`; 
      this.document.location.href = url;
    }
  }

  versao() {
    const url = `${environment.apiUrlAuth}/versao`; 
    this.http
      .get(url).subscribe((response : any) => {
         alert(JSON.stringify(response)); 
      }, (error) => {alert(JSON.stringify(error))}
   );
    return "Ok";
  }

  isAuthenticated() {
  //   const url = `${environment.apiUrlAuth}/isAuthenticated`;
  //   this.http
  //     .get(url).subscribe((response : ResponseModel) => {
  //       this.isAlthenticated = (response.message == "true");
  //        //alert(this.isAltenticated);
  //     }, (error) => {alert(JSON.stringify(error))}
  //  );
    return true; //this.isAlthenticated;
  }

  // submit() {
  //   this.hasError = false;
  //   const loginSubscr = this.authService
  //     .login(this.f.email.value, this.f.password.value)
  //     .pipe(first())
  //     .subscribe((user: UserModel) => {
  //       if (user) {
  //         this.router.navigate([this.  returnUrl]);
  //       } else {
  //         this.hasError = true;
  //       }
  //     });
  //   this.unsubscribe.push(loginSubscr);
  // }

}
