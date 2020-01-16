using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace backend {
    public enum Status {
        E,
        S
    }
    public class GamePayload {
        public Game game { set; get; }
        public Status status { set; get; }
        public string message { set; get; }

        public string toString() {
            JObject ans = new JObject();
            ans["game"] = game == null ? null : game.toJSON();
            ans["message"] = message;
            ans["status"] = status.ToString();
            return ans.ToString();
        }
    }
    public class Game {
        public Board board;
        public int p1;
        public int p2;
        public int gid;
        public string p1Name { get; set; }
        public string p2Name { get; set; }
        public int turn;
        public ItemColor p1Color;

        public Game(int gid, int p1, int p2, string p1Name, string p2Name, int turn, int p1Color, string board) {
            this.gid = gid;
            this.p1 = p1;
            this.p2 = p2;
            this.p1Name = p1Name;
            this.p2Name = p2Name;
            this.turn = turn;
            this.p1Color = (ItemColor) p1Color;
            this.board = new Board(board);
        }

        public bool validateMove(int uid, Point src, Point dst) {
            return true; //TODOL implement
        }

        public void makeMove(int uid, Point src, Point dst) {
            //TODOL imopelment
            //figure out the move type

            if (turn == 0) {
                makeFirstMove(uid, src, dst);
            } else {
                //
            }
        }

        public void makeFirstMove(int uid, Point src, Point dst) {
            //TODOL imopelment
            //set color field
        }

        public JObject toJSON(){
            JObject ans = new JObject();
            ans["board"] = JObject.Parse(this.board.ToJson())["board"];
            ans["p1"] = p1;
            ans["p2"] = p2;
            ans["p1Color"] = p1Color.ToString();
            ans["turn"] = turn;
            return ans;
        }

    }
    public class GamePlay {
        DBHandler dbHandler = new DBHandler();
        public Board makeABoard() {
            Board board = new Board();
            return board;
        }

        public async Task<GamePayload> makeGameAsync(int p1_uid) {
            //try to find pending game assigned if exists
            int gid = await dbHandler.hasPendingGame(p1_uid);
            Console.WriteLine($"makeGame found a gid {gid} for {p1_uid}");
            var ans = new GamePayload();
            if (gid > 0) {
                var game = await dbHandler.getGame(gid);
                ans.game = game;
                ans.status = Status.S;
                ans.message = "success fetch game";
                Console.WriteLine($"found a gid {gid} for {p1_uid}");

            } else {
                await dbHandler.markUserAvailable(p1_uid);
                int gameId = await dbHandler.createGameAsync(p1_uid);
                if (gameId == -1) {
                    ans.game = null;
                    ans.status = Status.E;
                    ans.message = "still looking for a teamate";
                    Console.WriteLine($"still looking for a teamate for {p1_uid}");
                } else {
                    var game = await dbHandler.getGame(gid);
                    ans.game = game;
                    ans.status = Status.S;
                    ans.message = "success fetch game";
                }

            }

            return ans;

        }

        public async Task<GamePayload> makeMoveAsync(int uid, Point src, Point dst) {
            //try to find pending game assigned if exists
            int gid = await dbHandler.getGidForUser(uid);
            var ans = new GamePayload();
            if (gid > 0) {

                var prevGame = await dbHandler.getGame(gid);
                bool isValid = prevGame.validateMove(uid, src, dst);
                if (!isValid) {
                    ans.game = prevGame;
                    ans.status = Status.E;
                    ans.message = $"invalid move for user {uid}";
                } else {
                    prevGame.makeMove(uid, src, dst);
                    await dbHandler.updateGame(prevGame);
                    ans.game = prevGame;
                    ans.status = Status.S;
                    ans.message = "success fetch game";
                }
            } else {
                Console.WriteLine($"invalid gid for user {uid}");
                ans.game = null;
                ans.status = Status.E;
                ans.message = $"invalid gid for user {uid}";
            }

            return ans;

        }

    }
}