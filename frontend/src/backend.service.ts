import { Injectable } from '@angular/core';
import { AuthPayload } from './auth-payload';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { GamePayload } from './game';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    'Access-Control-Allow-Origin': '*'
  }),
  withCredentials: true,
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private sign_up_url = "http://game.animalchess.com/sign_up"
  private sign_in_url = "http://game.animalchess.com/sign_in"
  private start_game_url = "http://game.animalchess.com/start_game"
  private make_move_url = "http://game.animalchess.com/make_move"
  private sign_out_url = "http://game.animalchess.com/sign_out"

  constructor(private http: HttpClient) {
  }

  public signIn = (user: JSON) => {
    return this.postToServer(user, this.sign_in_url);
  }

  public signUp = (user: JSON) => {
    return this.postToServer(user, this.sign_up_url);
  }

  private postToServer = (user: JSON, url: string) => {
    let promise = new Promise<AuthPayload>((resolve, reject) => {
      let body = JSON.stringify(user);
      this.http.post(url, body, httpOptions)
        .toPromise()
        .then((response) => {
          resolve(response as AuthPayload)
        }, (error) => {
          reject(error);
        })

    })
    return promise;
  }

  private postEmptyToServer = (url: string) => {
      return this.http.post(url, null,httpOptions);
  }


  //return a gamePayload
  public startAGame = () => {
    var url = this.start_game_url;
    let promise = new Promise<AuthPayload>((resolve, reject) => {
      this.http.post(url, null, httpOptions)
        .toPromise()
        .then((response) => {
          resolve(response as AuthPayload)
        }, (error) => {
          reject(error);
        })

    })
    return promise;
  }

    //return a gamePayload
    public makeAMove = (move:JSON) => {
      var url = this.make_move_url;
      let promise = new Promise<GamePayload>((resolve, reject) => {
        let body = JSON.stringify(move);
        this.http.post(url, body, httpOptions)
          .toPromise()
          .then((response) => {
            resolve(response as GamePayload)
          }, (error) => {
            reject(error);
          })
  
      })
      return promise;
    }
}
