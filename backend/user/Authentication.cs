using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace backend {
    class Authenticator {
        public static async Task<int> GetUserFromRequest(HttpContext context) {
            if (context.Request.Cookies.ContainsKey("uid")) {
                var uid = Convert.ToInt32(context.Request.Cookies["uid"]);
                return uid;
            }
            return -1;
        }
    }
}