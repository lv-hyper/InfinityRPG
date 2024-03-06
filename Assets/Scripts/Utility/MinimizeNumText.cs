using System;
using System.Numerics;
using System.Linq;

namespace InGame.Utility
{
    public static class MinimizeNumText
    {
        public static string Minimize(System.Numerics.BigInteger num)
        {
            string[] earlySurfix =
            {
                "K", "M", "B", "T", "Q"
            };

            int power =(int) Math.Floor(
                BigInteger.Log10(num)
            );
            string surfix = "";

            if (power >= 4)
            {
                int index = (power - 4) / 3;
                if (index < earlySurfix.Length)
                    surfix = earlySurfix[index];
                else
                {
                    int tmpIndex = index - earlySurfix.Length;
                    while (true)
                    {
                        int currentCharNum = tmpIndex % 26;
                        tmpIndex -= currentCharNum;
                        surfix += (char)(currentCharNum + 'a');

                        if (tmpIndex > 0)
                        {
                            tmpIndex /= 26;
                            tmpIndex -= 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    surfix = new string(surfix.Reverse().ToArray());
                }
                num = System.Numerics.BigInteger.Multiply(num, 100);
                for (int i = 0; i <= index; ++i)
                {
                    num = System.Numerics.BigInteger.Divide(num, 1000);
                }

                return string.Format("{0:0.00}", ((float)num) / 100) + surfix;
            }
            else return num.ToString();

            
        }
    }
}