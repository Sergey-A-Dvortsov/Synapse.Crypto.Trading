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

        public static string[] QuoteSymbols()
        {
            return ["USD", "USDT", "USDC", "USDE", "BTC"];
        }

        /// <summary>
        /// Invert the trade direction
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

        /// <summary>
        /// Extracts the quote currency from the instrument name
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string GetQuoteSymbol(this string symbol)
        {
            foreach (var quote in QuoteSymbols())
            {
                if (symbol.EndsWith(quote)) return quote;
            }

            return string.Empty;

        }

        /// <summary>
        /// Calculates the minimum price increment (tick size) for a given price. For Bitfinex only.
        /// </summary>
        /// <param name="price">The price for which to calculate the tick size.</param>
        /// <param name="precision">The number of decimal places to use when determining the tick size. Must be at least 1. Defaults to 5 if not
        /// specified.</param>
        /// <returns>The calculated tick size as a double, representing the smallest allowable price increment for the specified
        /// price and precision.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="price"/> is less than or equal to zero, or if <paramref name="precision"/> is less
        /// than 1.</exception>
        public static double GetTickSize(this double price, int precision = 5)
        {
            //tickSize = 10 ^ (floor(log10(P)) - precision + 1)
            if (price <= 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price must be greater than zero.");
            if (precision < 1)
                throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be at least 1.");
            var log10 = Math.Log10((double)price);
            var floorLog10 = Math.Floor(log10);
            var tickSize = Math.Pow(10, floorLog10 - precision + 1);
            return tickSize;
        }


    }
}
