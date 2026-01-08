using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Crypto.Trading
{
    // Copyright(c) [2026], [Sergey Dvortsov]
    public static class Helpers
    {

        /// <summary>
        /// The trade direction
        /// </summary>
        /// <param name="side">direction</param>
        /// <returns></returns>
        public static Sides Invert(this Sides side)
        {
            return side == Sides.Sell ? Sides.Buy : Sides.Sell;
        }

        /// <summary>
        /// Extracts the month number from the letter code
        /// </summary>
        /// <param name="smb">letter code</param>
        /// <returns></returns>
        private static int GetMonth(char smb)
        {
            //A-01;B-02;C-03;D-04;E-05;F-06;G-07;H-08;I-09;J-10;K-11;L-12;
            switch (smb)
            {
                case 'A':
                    return 1;
                case 'B':
                    return 2;
                case 'C':
                    return 3;
                case 'D':
                    return 4;
                case 'E':
                    return 5;
                case 'F':
                    return 6;
                case 'G':
                    return 7;
                case 'H':
                    return 8;
                case 'I':
                    return 9;
                case 'J':
                    return 10;
                case 'K':
                    return 11;
                case 'L':
                    return 12;
                default:
                    return 99;
            }

        }

        /// <summary>
        /// Extracts the year number from the letter code
        /// </summary>
        /// <param name="smb">letter code</param>
        /// <returns></returns>
        private static int GetYear(char smb)
        {
            //год A - 0; B - 1; C - 2; ; D - 3; E - 4; ; F - 5; G - 6; H - 7; K - 8; L - 9; 
            switch (smb)
            {
                case 'A':
                    return 0;
                case 'B':
                    return 1;
                case 'C':
                    return 2;
                case 'D':
                    return 3;
                case 'E':
                    return 4;
                case 'F':
                    return 5;
                case 'G':
                    return 6;
                case 'H':
                    return 7;
                case 'K':
                    return 8;
                case 'L':
                    return 9;
                default:
                    return 99;
            }

        }

        /// <summary>
        /// Creates a unique order identifier
        /// </summary>
        /// <returns></returns>
        public static ulong CreateUserId()
        {
            return (ulong)DateTime.UtcNow.TimeOfDay.TotalMilliseconds;
        }

        /// <summary>
        /// Creates a unique order identifier
        /// </summary>
        /// <returns></returns>
        public static ulong GetUnicId(this ref DateTime lasttime, ref int lastcnt)
        {
            var nowtime = DateTime.Now;

            if (Math.Round((nowtime - lasttime).TotalMilliseconds, 0) > 0)
                lastcnt = 0;
            else
                lastcnt += 1;
            var strid = string.Format("{0}{1:000}", DateTime.Now.ToString("yyMMddHHmmssfff"), lastcnt);
            lasttime = nowtime;

            return ulong.Parse(strid);

        }

    }
}
