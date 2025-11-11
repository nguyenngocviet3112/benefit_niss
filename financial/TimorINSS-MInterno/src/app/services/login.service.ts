import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from 'src/environments/environment';
import { Utilizador } from '../response-models/utilizador-response';
import { LoginRequest } from '../request-models/login-request';
import { RecoverPasswordRequest, RecoverSetPasswordRequest } from '../request-models/recoverPassword-request';
import CreateNISSInfoRequest from '../request-models/createNISSInfo-request';
import { ApiHelperService } from './api-helper.service';

@Injectable({
    providedIn: 'root'
  })
export class LoginService {

    constructor(
        private router: Router,
        private http: HttpClient,
        private api: ApiHelperService
    ) {}

    public login(request: LoginRequest) : Observable<{user: Utilizador, token: string, totalCount: number}> {

        return this.api.post('login/InternalAuthenticate', request)
            .pipe(map((response : any) => {
                // store user details and jwt token in local storage to keep user logged in between page refreshes
                // sessionStorage.setItem('user', JSON.stringify(user));
                // return user;
                return {
                    user :
                    {
                        id: response.user.id,
                        username: response.user.username,
                        niss: response.user.niss,
                        isInternal: false,
                        idEntidade: response.user.idEntidade,
                        permissions: response.user.permissions,
                        perfil: response.user.perfil
                    },
                    token : response.token,
                    totalCount : response.totalCount
                }
            }));
    }

    public recoverPassword(request: RecoverPasswordRequest){
        return this.api.post('login/InternalRecoverPassword', request);
    }

    public firstAcess(request: RecoverPasswordRequest){
        return this.api.post('login/InternalFirstAcess', request);
    }

    public setUpPassword(request: RecoverSetPasswordRequest){
        return this.api.post('login/SetUpPassword', request);
    }

    public createUser(request: RecoverSetPasswordRequest){
        return this.api.post('login/CreateInternalUser', request);
    }

    public createNISSInfor(request: CreateNISSInfoRequest){
        return this.api.post('login/CreateNissInfor', request);
    }

    public logout() {
        // remove user from local storage and set current user to null
        sessionStorage.removeItem('user');
        this.router.navigate(['']);
    }

    public register(user: Utilizador) {
        return this.api.post('users/register', user);
    }

    public downloadLogs() {
        const request: any = {};
        return this.api.post('login/Logs', request);
    }

    public validToken(request:{token: string, isRecover: boolean}) {
        return this.api.post('login/ValidToken', request);
    }
}
