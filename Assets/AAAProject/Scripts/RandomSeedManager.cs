using System;
using System.Collections.Generic;
using System.Linq;

namespace AAAProject.Scripts
{
    public static class RandomKeyManager
    {
        public static string GetRandomKey()
        {
            Random random = new Random();
            string seed = "abcdefghijklmnopqrstuvwxyz0123456789";
 
            var seq = RandomSequence(random, seed, 10);
            char[] chars = seq.ToArray();
            string key = "";
            for (int i = 0; i < chars.Length; i++)
            {
                key += chars[i];
            }
            return key;
        }
        
        private static IEnumerable<char> RandomSequence(Random r, string seed, int length)
        {
            for (int i = 0; i < length; i++)
            {
                yield return seed[r.Next(0, seed.Length)];
            }
        }
    }
}
