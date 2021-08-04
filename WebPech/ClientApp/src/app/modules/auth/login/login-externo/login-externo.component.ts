import { ResponseModel } from './../../_models/resoponse';
import { Component, Inject, OnInit } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of, Subject, throwError} from 'rxjs';
import { environment } from 'src/environments/environment';
import { map, catchError, finalize, tap } from 'rxjs/operators';
import { BehaviorSubject, Subscription } from 'rxjs';

@Component({
  selector: 'app-login-externo',
  templateUrl: './login-externo.component.html',
  styleUrls: ['./login-externo.component.scss']
})

export class LoginExternoComponent implements OnInit {
  isLoginExterno: boolean = true;
  isAltenticated: boolean = false;

  constructor(
     @Inject(DOCUMENT) private document: Document, private httpClient: HttpClient,
     private http: HttpClient) { 
     this.versao();
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
    const url = `${environment.apiUrlAuth}/isAuthenticated`;
    this.http
      .get(url).subscribe((response : ResponseModel) => {
        this.isAltenticated = (response.message == "true");
         //alert(this.isAltenticated);
      }, (error) => {alert(JSON.stringify(error))}
   );
    return this.isAltenticated;
  }

}
