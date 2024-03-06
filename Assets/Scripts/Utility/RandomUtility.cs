using System;

namespace InGame.Utility
{
    public static class RandomUtility
    {
        static Random randomInstance = null;
        private static Random GetRandomInstance()
        {
            if(randomInstance == null)
            {
                randomInstance = new Random();
            }

            return randomInstance;
        }
        public static long RandomRangeLong(long min, long max)
        {
            byte[] buf = new byte[8];
            GetRandomInstance().NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
    }
}