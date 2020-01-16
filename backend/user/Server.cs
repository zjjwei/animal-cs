using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace backend {
    public class ChessServer {
        private GamePlay table = new GamePlay();

        private string CreatePayload(string status, string msg) {

            JObject payload = new JObject(
                new JProperty("status", status), new JProperty("message", msg));
            return Newtonsoft.Json.JsonConvert.SerializeObject(payload);
        }

        public async Task SignUp(HttpContext context) {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            Console.WriteLine("got input " + body);
            JObject inputJson = JObject.Parse(body);
            string username = (string) inputJson["username"];
            string pass = (string) inputJson["pass"];
            string email = (string) inputJson["email"];
            string msg = "";
            string status = "";
            User user = new User();
            try {
                await user.SignUp(username, pass, email);
                msg = "Success. Accounts created! " + username;
                status = "S";
            } catch (Exception e) {
                msg = "Sorry. Failed to created account for " + username + ": " + e.Message;
                status = "E";
            }
            await context.Response.WriteAsync(CreatePayload(status, msg));
        }

        public async Task SignOut(HttpContext context) {
            int uid = await Authenticator.GetUserFromRequest(context);
            context.Response.Cookies.Delete("uid");
            User user = new User();
            string msg = "";
            string status = "";
            try {
                await user.SignOut(uid);
                msg = "Success. Logged out!";
                status = "S";
            } catch (Exception e) {
                msg = "Sorry. Failed to log out";
                status = "E";
            }
            await context.Response.WriteAsync(CreatePayload(status, msg));
        }

        public async Task SignIn(HttpContext context) {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            Console.WriteLine("got input " + body);
            JObject inputJson = JObject.Parse(body);
            string username = (string) inputJson["username"];
            string pass = (string) inputJson["pass"];
            string msg = "";
            string status = "";
            User user = new User();
            try {
                var uid = await user.SignIn(username, pass);
                context.Response.Cookies.Append("uid", uid.ToString());
                status = "S";
                msg = $"Success! {username} signed in!";
            } catch (Exception e) {
                msg = $"Failed to log in: {e.Message}";
                status = "E";
            }

            // await context.SignInAsync();  
            await context.Response.WriteAsync(CreatePayload(status, msg));
        }

        public async Task StartGame(HttpContext context, int uid) {
            var game = await table.makeGameAsync(uid);
            await context.Response.WriteAsync(game.toString());
        }

        public async Task makeMove(HttpContext context, int uid) {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            Console.WriteLine("got input " + body);
            JObject inputJson = JObject.Parse(body);
            var src = (JObject) inputJson["src"];
            var dst = (JObject) inputJson["dst"];
            Point srcP = new Point((int) src["x"], (int) src["y"]);
            Point dstP = new Point((int) dst["x"], (int) dst["y"]);
            var game = await table.makeMoveAsync(uid, srcP, dstP);
            await context.Response.WriteAsync(game.toString());
        }

    }
}