import { Component, OnInit } from '@angular/core';
import { interval } from "rxjs/internal/observable/interval";

import { startWith, switchMap } from "rxjs/operators";

import { AuthService } from '../../backend.service';
import { Game, GamePayload, Point, Piece } from '../../game';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})

export class HomeComponent {
  public status: string; // S/E
  public hasGame = false;
  public game:Game;
  // private board:Board;
  public pieceList:Piece[][];
  public size = 4;
  // private p1: number;
  // private p2: number;
  public uid: number;
  // private p1Color: number;
  // private turn: number;


  public src : Point;
  public dst : Point;
  public animalMap = ["None","Elephant","Lion","Tiger","Cheetah", "Wolf","Dog","Cat","Mouse"];
  constructor(private userService: AuthService) {
    this.pieceList = [];
    for(var i=0;i<this.size;i++){
      this.pieceList[i] = [];
    }
  }

  // public parseGame(table:GamePayload){
  //   this.board = table.game.board;
  //   var p1 = table.game.p1;
  //   var p2 = table.game.p2;
  //   var uid = table.uid;
  //   var p1Color = table.game.p1Color;

  // }
  public renderGame(table: GamePayload): void {
    //todo: implement it
    console.log("rendering");
    this.game = <Game> table.game;

    this.game.board.forEach((x) => {
      console.log(x.animal);
      console.log(x.color);
      console.log(x.face_up);
    });

    var k = 0;
    for(var i=0;i<this.size;i++){
      for(var j=0;j<this.size;j++){
        this.pieceList[i][j] = this.game.board[k];
        k++;
      }
    }
  };

  public quit():void{
    this.hasGame = false;
    //TODO: implement quit logic
  }
  public handshake():void{
    
    this.hasGame = false;
    //TODO: implement quit logic
  }

  private composeMovePayload(){
    var ans = {};
    ans["src"] = {"x": this.src.x, "y": this.src.y};
    ans["dst"] = {"x": this.dst.x, "y": this.dst.y};
    return <JSON> ans;
  }
  public move():void{
    //TODO: implement quit logic
    if(this.src == null || this.dst == null){
      alert("error: you must pick two different points for a move");
    }else{
      var payload = this.composeMovePayload();
      this.userService.makeAMove(payload);
    }
  }
  public flip():void{
    //TODO: implement quit logic
    // if(this.dst == null || this.src != null){
    //   if()
    // }

  }

  getGame() {
    this.userService.startAGame().then((response) => {
      var res = <GamePayload>response;
      if (res.status === 'S') {
        console.log("success");
        this.status = 'S';
        this.hasGame = true;
        this.uid = res.uid;
        console.log('hasGame is true');
        //render game
        this.renderGame(res);
      } else {
        console.log('failed to get agame ' + res.message);
        console.log("should try again later");

      }
    }, (error) => {
      console.log("get game failed");
      console.log(error.statusText);
    });
  }


  // longPoll() {
  //   let game = interval(5000)
  //     .pipe(
  //       startWith(0),
  //       switchMap(() => this.userService.startAGame())
  //       .subscribe(res => {
  //       console.log("got res" + JSON.stringify(res));
  //       this.status = res.status;
  //       if(this.status == "S"){
  //         //success making a game,
  //         //render the game and stop long polling
  //         game.unsubscribe();
  //       }else{
  //         console.log("failed to find a match -- retrying in 5s...");
  //       }
  //       })
  //   ;
  // }

  //   public getTeamMatchingProgress(url:string): any {
  //     let progress = interval(1500)
  //         .switchMap(() => this.userService.startAGame())
  //         .subscribe(
  //         (data) => {              
  //             if (!data.message) {
  //                 this.toastyCommunicationService.addSuccesResponseToast("done");
  //                 progress.unsubscribe();
  //             }            
  //         },
  //         error => this.handleError(error));
  // }
}
