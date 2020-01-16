using System.Threading.Tasks;

namespace backend {
    public class User {
        DBHandler dbHandler = new DBHandler();
        public async Task SignUp(string username, string pass, string email) {
            await dbHandler.CreateUserAsync(username, pass, email);
        }

        public async Task<int> SignIn(string username, string pass) {
            return await dbHandler.SignInAsync(username, pass);
        }

        public async Task SignOut(int uid) {
            await dbHandler.SignOutAsync(uid);
        }
    }
}