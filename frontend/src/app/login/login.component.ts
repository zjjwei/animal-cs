import { Component, OnInit } from '@angular/core';
import {AuthService} from '../../backend.service'
import {AuthPayload} from '../../auth-payload';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {

  constructor(private AuthService: AuthService, private router: Router) { }
  ngOnInit(): void {
    
  }
  private username: string;
  private pass: string;
  private email: string;
  private isSignedIn:boolean;

  signUp() {
    console.log(this.username);
    console.log(this.pass);
    console.log(this.email);
    var data:any = {username: this.username, pass: this.pass, email: this.email};
    this.AuthService.signUp(<JSON> data).then((response)=>{
      if(response.status === 'S'){
        console.log("success");
        this.isSignedIn = true;
        var uid = 1; //TODO: read uid from server
        this.router.navigate(["/counter"]);
      }else{
        console.log('failed to sign up ' + response.message);
      }
    }, (error) => {
      console.log("sign up failed");
      console.log(error.statusText);
    });
  }


  
  signIn() {
    console.log(this.username);
    console.log(this.pass);
    var data:any = {username: this.username, pass: this.pass, email: this.email};
    this.AuthService.signIn(<JSON> data).then((response)=>{
      if(response.status === 'S'){
        console.log("success");
        var uid = 1; //TODO: read uid from server
        // this.router.navigate(["/counter"],{ queryParams: { 'uid':uid}});
        this.router.navigate(["/counter"]);
      }else{
        console.log('failed to sign in ' + response.message);
      }
    }, (error) => {
      console.log("sign ip failed");
      console.log(error.statusText);
    });
  }
}
