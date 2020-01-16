export interface Game {
    "turn": number,
    "p1": number,
    "p2": number,
    "p1Color": number,
    "board": Array<Piece>
}



export interface Piece {
    "animal": number;
    "color": number;
    "face_up": boolean;
}

export interface Point {
    "x": number;
    "y": number;
}

export interface GamePayload {
    "game": Game;
    "status": string;
    "message": string;
    "uid": number;

}



// {
//     "game": {
//         "board": [
//             {
//                 "animal": 2,
//                 "color": 1,
//                 "face_up": false
//             },
//             {
//                 "animal": 1,
//                 "color": 1,
//                 "face_up": false
//             },
//             {
//                 "animal": 4,
//                 "color": 2,
//                 "face_up": false
//             },
//             {
//                 "animal": 7,
//                 "color": 2,
//                 "face_up": false
//             },
//             {
//                 "animal": 8,
//                 "color": 1,
//                 "face_up": false
//             },
//             {
//                 "animal": 2,
//                 "color": 2,
//                 "face_up": false
//             },
//             {
//                 "animal": 8,
//                 "color": 2,
//                 "face_up": false
//             },
//             {
//                 "animal": 3,
//                 "color": 1,
//                 "face_up": false
//             },
//             {
//                 "animal": 3,
//                 "color": 2,
//                 "face_up": false
//             },
//             {
//                 "animal": 1,
//                 "color": 2,
//                 "face_up": false
//             },
//             {
//                 "animal": 5,
//                 "color": 1,
//                 "face_up": false
//             },
//             {
//                 "animal": 4,
//                 "color": 1,
//                 "face_up": false
//             },
//             {
//                 "animal": 7,
//                 "color": 1,
//                 "face_up": false
//             },
//             {
//                 "animal": 6,
//                 "color": 2,
//                 "face_up": false
//             },
//             {
//                 "animal": 5,
//                 "color": 2,
//                 "face_up": false
//             },
//             {
//                 "animal": 6,
//                 "color": 1,
//                 "face_up": false
//             }
//         ],
//             "p1": 1,
//                 "p2": 2,
//                     "p1Color": "None",
//                         "turn": 0
//     },
//     "message": "success fetch game",
//         "status": "S"
// }