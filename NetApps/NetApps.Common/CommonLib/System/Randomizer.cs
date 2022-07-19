using System.Linq;

namespace System
{
    public class Randomizer
    {
        public static string GetRandomValue(params string[] args)
        {
            var i = rand.Next(args.Length);
            return args[i];
        }

        private static readonly Random rand = new Random();
        public static string GenerateRandomString(int len)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // 0123456789";
            var charArr = Enumerable.Repeat(chars, len).Select(s => s[rand.Next(s.Length)]).ToArray();
            return new string(charArr);
        }

    }
}
