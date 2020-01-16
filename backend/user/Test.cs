using System;
using Newtonsoft.Json.Linq;

namespace backend {

    public static class TestJSON {
        public static void test() {

            string jsonText = @"{
  'short': {
    'original': 'http://www.foo.com/',
    'short': 'krehqk',
    'error': {
      'code': 0,
      'msg': 'No action taken'
    }
  }
}";
            JObject json = JObject.Parse(jsonText);
            var Original = (string) json["short"]["original"];
            Console.WriteLine(Original);

        }
    }

}