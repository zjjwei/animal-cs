using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace backend {
    public class DBHandler {

        /**
        username must be non-null, non-empty
        pass must be non-null, non-empty
        */
        public bool CreateUser(string username, string pass, string email) {
            try {
                using var dbCon = DBConnection.NewConnection();

                //suppose col0 and col1 are defined as VARCHAR in the DB
                string query = "SELECT count(uid) from user where username=@val1";
                using var cmd = new MySqlCommand(query, dbCon);
                cmd.Parameters.AddWithValue("@val1", username);

                object result = cmd.ExecuteScalar();
                if (result is int) {
                    int r = (int) result;
                    Console.WriteLine("Number of duplicates is : " + r + " for user " + username);
                    if (r > 0) {
                        Console.WriteLine("returning");
                        return false;
                    }
                }
                //try to insert to the db

                Console.WriteLine("no duplicate found, going to insert");
                // TODO: try to insert hashed password
                string insert_query = "insert into user (username, pass, email) values(@username, @pass, @email)";
                string hashedPass = SecurePasswordHasher.Hash(pass);
                using var insert_cmd = new MySqlCommand(insert_query, dbCon);
                insert_cmd.Parameters.AddWithValue("@username", username);
                insert_cmd.Parameters.AddWithValue("@pass", hashedPass);
                insert_cmd.Parameters.AddWithValue("@email", email);
                insert_cmd.ExecuteNonQuery();

                Console.WriteLine("Done adding user " + username);
                return true;
            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                return false;
            }
        }

        public async Task CreateUserAsync(string username, string pass, string email) {
            try {
                await using var dbCon = await DBConnection.NewConnectionAsync();

                //suppose col0 and col1 are defined as VARCHAR in the DB
                string query = "SELECT count(uid) from user where username=@val1";
                await using var cmd = new MySqlCommand(query, dbCon);
                cmd.Parameters.AddWithValue("@val1", username);

                object result = await cmd.ExecuteScalarAsync();
                if (result is int) {
                    int r = (int) result;
                    Console.WriteLine("Number of duplicates is : " + r + " for user " + username);
                    if (r > 0) {
                        Console.WriteLine("returning");
                        throw new Exception("Duplicate user");
                    }
                }
                //try to insert to the db

                Console.WriteLine("no duplicate found, going to insert");
                // TODO: try to insert hashed password
                string insert_query = "insert into user (username, pass, email) values(@username, @pass, @email)";
                string hashedPass = SecurePasswordHasher.Hash(pass);
                await using var insert_cmd = new MySqlCommand(insert_query, dbCon);
                insert_cmd.Parameters.AddWithValue("@username", username);
                insert_cmd.Parameters.AddWithValue("@pass", hashedPass);
                insert_cmd.Parameters.AddWithValue("@email", email);
                await insert_cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Done adding user " + username);
            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                throw e;
            }
        }

        public bool SignIn(string username, string pass) {

            try {
                using var dbCon = DBConnection.NewConnection();
                string query = "SELECT pass from user where username=@val1";
                var cmd = new MySqlCommand(query, dbCon);
                cmd.Parameters.AddWithValue("@val1", username);
                object result = cmd.ExecuteScalar();
                if (result != null) {
                    string r = (string) result;
                    if (!SecurePasswordHasher.Verify(pass, r)) {
                        Console.WriteLine("returning as password not match");
                        return false;
                    } else {
                        //update the state of this user 

                        Console.WriteLine("password correct, updating values");
                        // TODO: try to insert hashed password
                        bool logged_in = true;
                        string insert_query = "update user set last_login_time=CURRENT_TIMESTAMP, logged_in=@logged_in where username=@username ";
                        var insert_cmd = new MySqlCommand(insert_query, dbCon);
                        insert_cmd.Parameters.AddWithValue("@logged_in", logged_in);
                        insert_cmd.Parameters.AddWithValue("@username", username);
                        insert_cmd.ExecuteNonQuery();

                        Console.WriteLine("Done updating user status" + username);
                        return true;

                    }
                } else {
                    Console.WriteLine($"username {username} does not exit in table");
                    return false;
                }
            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                return false;
            }
        }

        public async Task<int> SignInAsync(string username, string pass) {
            try {
                await using var dbCon = await DBConnection.NewConnectionAsync();
                string query = "SELECT pass, uid from user where username=@val1";
                await using var cmd = new MySqlCommand(query, dbCon);
                cmd.Parameters.AddWithValue("@val1", username);

                int uid = -1;
                await using(var reader = await cmd.ExecuteReaderAsync()) {
                    if (!await reader.ReadAsync()) {
                        Console.WriteLine($"username {username} does not exit in table");
                        throw new Exception($"No such user {username}");
                    }
                    string realHash = reader.GetString(0);
                    uid = reader.GetInt32(1);
                    if (!SecurePasswordHasher.Verify(pass, realHash)) {
                        Console.WriteLine("password not match");
                        throw new Exception("Invalid password");
                    }
                }

                //update the state for this user 
                Console.WriteLine("password correct, updating values");
                string insert_query = "update user set last_login_time=CURRENT_TIMESTAMP, logged_in=true where username=@username ";
                await using var insert_cmd = new MySqlCommand(insert_query, dbCon);
                insert_cmd.Parameters.AddWithValue("@username", username);
                await insert_cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Done updating user status" + username);
                return uid;
            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                throw;
            }
        }

        public async Task SignOutAsync(int uid) {
            try {
                await using var dbCon = await DBConnection.NewConnectionAsync();
                string query = "SELECT count(uid) from user where uid=@uid";
                await using var cmd = new MySqlCommand(query, dbCon);
                cmd.Parameters.AddWithValue("@uid", uid);

                object result = await cmd.ExecuteScalarAsync();
                if (result is int) {
                    int r = (int) result;
                    if (r == 0) {
                        Console.WriteLine("invalid uid: " + r + " for user " + uid);
                        throw new Exception("invalid uid");
                    }
                }

                //TODO: quit his game(s)

                //update the state for this user 
                Console.WriteLine("uid correct, updating values");
                string update_query = "update user set last_logout_time=CURRENT_TIMESTAMP, logged_in=false where uid=@uid";
                await using var insert_cmd = new MySqlCommand(update_query, dbCon);
                insert_cmd.Parameters.AddWithValue("@uid", uid);
                await insert_cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Done updating user status " + uid);
            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                throw;
            }
        }

        public async Task<int> getAvailableUser(int uid) {
            try {
                await using var dbCon = await DBConnection.NewConnectionAsync();
                //first check whether user has in-complete game, if so, return game
                //else 1) look for another user
                //     if found, 2) make a game 3) return game
                string query = "SELECT uid from user where uid !=@uid and want_play=true and logged_in=true and in_game = false order by last_login_time asc limit 1";
                await using var cmd = new MySqlCommand(query, dbCon);
                cmd.Parameters.AddWithValue("@uid", uid);
                bool foundMatch = false;
                int p2 = -1;
                object result = await cmd.ExecuteScalarAsync();
                if (result is int) {
                    int r = (int) result;
                    Console.WriteLine($"found a match, ({uid}, {r})");
                    foundMatch = true;
                    p2 = r;
                }
                if (p2 == -1) {
                    return -1;
                }
                //TODO: did not find available user

                //update the state for this pair of user
                Console.WriteLine("uid correct, updating values");
                string update_query = "update user set want_play=false and in_game=true where uid=@uid OR uid=@p2";
                await using var update_cmd = new MySqlCommand(update_query, dbCon);
                update_cmd.Parameters.AddWithValue("@uid", uid);
                await update_cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Done updating user status " + uid);
                return p2;
            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                throw;
            }
        }

        // public Game parseDbRequest(MySqlDataReader reader){

        // }
        public async Task<Game> getGame(int gid) {
            try {
                await using var dbCon = await DBConnection.NewConnectionAsync();
                //first check whether user has in-complete game, if so, return game
                //else return null;
                string query = "SELECT gid,p1,p2,p1_color,turn,board from game where gid=@gid";
                await using var cmd = new MySqlCommand(query, dbCon);
                cmd.Parameters.AddWithValue("@gid", gid);
                Game ans = null;
                bool found = false;
                await using(var reader = await cmd.ExecuteReaderAsync()) {
                    if (!await reader.ReadAsync()) {
                        Console.WriteLine($"gid {gid} does not exit in game");
                        throw new Exception($"No such game row {gid}");
                    }
                    int ggid = reader.GetInt32(0);
                    int p1 = reader.GetInt32(1);
                    int p2 = reader.GetInt32(2);
                    int p1Color = (reader.IsDBNull(3))? 0 : reader.GetInt32(3);
                    int turn = reader.GetInt32(4);
                    string board = reader.GetString(5);
                    ans = new Game(ggid, p1, p2, "", "", turn, p1Color, board);
                    found = true;
                }
                if (!found) {
                    return ans;
                }
                //find the usernames;
                string p1Name = await getUserNameAsync(ans.p1, dbCon);
                string p2Name = await getUserNameAsync(ans.p2, dbCon);
                ans.p1Name = p1Name;
                ans.p2Name = p2Name;
                return ans;

            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                throw;
            }
        }

        public async Task<int> hasPendingGame(int uid) {
            try {
                await using var dbCon = await DBConnection.NewConnectionAsync();
                string query = "SELECT gid from user where uid=@uid";
                await using var cmd = new MySqlCommand(query, dbCon);
                cmd.Parameters.AddWithValue("@uid", uid);
                object gid = await cmd.ExecuteScalarAsync();
                if (gid is int) {
                    return (int) gid;
                } else {
                    throw new Exception($"invalid gid with null for uid {uid}");
                }

            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                throw;
            }
        }

        public async Task updateGame(Game game) {
            int gid = game.gid;
            int turn = game.turn;
            if (turn == 1) {
                if (game.p1Color == ItemColor.None) {
                    throw new Exception("invalid step 1 game, color not set");
                } else {
                    //update board, turn, p1Color info to db
                }
            } else {
                //update board, turn info to db
            }

        }

        public async Task<string> getUserNameAsync(int uid, MySqlConnection dbCon) {
            string q = "SELECT username from user where uid =@uid";
            await using var cmd = new MySqlCommand(q, dbCon);
            cmd.Parameters.AddWithValue("@uid", uid);
            object name = await cmd.ExecuteScalarAsync();
            return name == null ? "" : (string) name;
        }
        public async Task<int> addGame(int p1, int p2, JObject board) {
            try {
                await using var dbCon = await DBConnection.NewConnectionAsync();
                //add a game row in Game table
                Console.WriteLine($"Adding a row in game table for ({p1},{p2}");
                string insert_query = "insert into game (p1,p2,board) values (@p1, @p2, @board)";
                await using var insert_cmd = new MySqlCommand(insert_query, dbCon);
                insert_cmd.Parameters.AddWithValue("@p1", p1);
                insert_cmd.Parameters.AddWithValue("@p2", p2);
                insert_cmd.Parameters.AddWithValue("@board", board);
                await insert_cmd.ExecuteNonQueryAsync();
                long id = insert_cmd.LastInsertedId;
                return (int) id;

            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                throw;
            }
        }

        public async Task markUserAvailable(int uid) {
            try {
                await using var dbCon = await DBConnection.NewConnectionAsync();
                //add a game row in Game table
                Console.WriteLine($"Makr user available{uid}");
                string query = "update user set want_play=true where uid=@uid";
                await using var insert_cmd = new MySqlCommand(query, dbCon);
                insert_cmd.Parameters.AddWithValue("@uid", uid);
                await insert_cmd.ExecuteNonQueryAsync();

            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                throw;
            }
        }

        public async Task<int> getGidForUser(int uid) {
            try {
                await using var dbCon = await DBConnection.NewConnectionAsync();
                //add a game row in Game table
                Console.WriteLine($"Makr user available{uid}");
                string query = "select gid from user where uid=@uid";
                await using var insert_cmd = new MySqlCommand(query, dbCon);
                insert_cmd.Parameters.AddWithValue("@uid", uid);
                var id = await insert_cmd.ExecuteScalarAsync();
                return (int) id;

            } catch (Exception e) {
                Console.WriteLine($"Cannot connect to db: {e.Message}");
                throw;
            }
        }

        public async Task<int> createGameAsync(int p1) {
            try {
                await using var dbCon = await DBConnection.NewConnectionAsync();

                MySqlCommand myCommand = dbCon.CreateCommand();
                MySqlTransaction myTrans;

                // Start a local transaction
                myTrans = dbCon.BeginTransaction();
                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                myCommand.Connection = dbCon;
                myCommand.Transaction = myTrans;

                try {
                    myCommand.CommandText = "select uid from user where uid!=@p1 and want_play=true and gid=0 order by last_login_time asc limit 1";
                    myCommand.Parameters.AddWithValue("@p1", p1);
                    Console.WriteLine("exceuting 1 " + myCommand.CommandText);
                    myCommand.Prepare();
                    var user = await myCommand.ExecuteScalarAsync();
                    int p2 = -1;
                    if (user is null) {
                        Console.WriteLine("cannot find matching user");
                        return -1;
                    } else {
                        p2 = (int) user;
                        Console.WriteLine($"found p2 {p2}");
                    }
                    string board = new Board().ToJson();
                    Console.WriteLine("json " + board.ToString());
                    myCommand.CommandText = "insert into game (p1,p2,board) VALUES (@p11,@p22,@board)";

                    myCommand.Parameters.AddWithValue("@p11", p1);
                    myCommand.Parameters.AddWithValue("@p22", p2);
                    myCommand.Parameters.AddWithValue("@board", board);
                    myCommand.Prepare();
                    Console.WriteLine("exceuting 2 {myCommand.CommandText} with values {p1},{p2}");
                    myCommand.ExecuteNonQuery();
                    int gid = (int) myCommand.LastInsertedId;

                    myCommand.CommandText = "update user set gid=@gid where uid=@p111 or uid =@p222";
                    myCommand.Parameters.AddWithValue("@p111", p1);
                    myCommand.Parameters.AddWithValue("@p222", p2);
                    myCommand.Parameters.AddWithValue("@gid", gid);

                    Console.WriteLine($"exceuting 3 {myCommand.CommandText} with values {p1},{p2},{gid}");
                    myCommand.Prepare();
                    myCommand.ExecuteNonQuery();
                    myTrans.Commit();
                    Console.WriteLine($"Created a game for {p1}, {p2} in database.");
                    return gid;

                } catch (Exception e) {
                    try {
                        myTrans.Rollback();
                    } catch (MySqlException ex) {
                        if (myTrans.Connection != null) {
                            Console.WriteLine("An exception of type " + ex.GetType() +
                                " was encountered while attempting to roll back the transaction.");
                        }
                        throw ex;
                    }

                    Console.WriteLine("An exception of type " + e.GetType() +
                        " was encountered while inserting the data.");
                    Console.WriteLine("Neither record was written to database.");
                    throw e;
                }
            } catch (Exception e) {
                Console.WriteLine("cannot connect to db");
                throw e;
            }
        }

    }

}