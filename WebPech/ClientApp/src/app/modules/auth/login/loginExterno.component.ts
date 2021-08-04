import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private _isUserAuthenticatedSubject = new BehaviorSubject<boolean>(false);

  isUserAuthenticated: Observable<boolean> = this._isUserAuthenticatedSubject.asObservable();

  constructor(@Inject(DOCUMENT) private document: Document, private httpClient: HttpClient) { }

  updateUserAuthenticationStatus(){
    return this.httpClient.get<boolean>(`http://localhost:5001/home/isAuthenticated`, {withCredentials: true}).pipe(tap(isAuthenticated => {
      this._isUserAuthenticatedSubject.next(isAuthenticated);
    }));
  }

  setUserAsNotAuthenticated(){
    this._isUserAuthenticatedSubject.next(false);
  }

  login() {
    this.document.location.href = "http://localhost:5001/login";
  }

  logout() {
    this.document.location.href = "http://localhost:5001/logout";
  }

}
