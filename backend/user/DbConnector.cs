using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace backend {
    public class DBConnection {

        public static MySqlConnection NewConnection() {
            var connection = new MySqlConnection("Server=localhost; database=animal; UID=root; password=mypass");
            connection.Open();
            return connection;
        }
        public static async Task<MySqlConnection> NewConnectionAsync() {
            var connection = new MySqlConnection("Server=localhost; database=animal; UID=root; password=mypass");
            await connection.OpenAsync();
            return connection;
        }
    }
}