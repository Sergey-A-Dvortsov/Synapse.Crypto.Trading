using NLog;
using Synapse.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synapse.Crypto.Trading
{
    public abstract class OrderBook
    {

        public OrderBook(InstrumentTypes type, string symbol, double ticksize, int decimals)
        {
            Type = type;
            Symbol = symbol;
            TickSize = ticksize;
            Decimals = decimals;
            Asks = [];
            Bids = new (Comparer<double>.Create( static (a, b) => b.CompareTo(a)));
        }

        public OrderBook(InstrumentTypes type, string symbol) : this(type, symbol, double.NaN, -1)
        {
        }

        public Logger logger = LogManager.GetCurrentClassLogger();

        public string Symbol { get; private set; }

        public string BaseSymbol { get; set; }
        public string QuoteSymbol { get; set; }

        public InstrumentTypes Type { get; private set; }

        public double TickSize { get; set; }

        public int Decimals { get; set; }

        public bool Valid { get; set; }

        public SortedDictionary<double, double> Asks { get; private set; }

        public SortedDictionary<double, double> Bids { get; private set; }

        public Quote BestAsk
        {
            get
            {
                var kvp = Asks.FirstOrDefault();
                return new Quote(kvp.Key, kvp.Value );
            }
        }

        public Quote BestBid
        {
            get
            {
                var kvp = Bids.FirstOrDefault();
                return new Quote(kvp.Key, kvp.Value);
            }
        }

        //public abstract DateTime UpdateTime { get; }

        public DateTime SnapshotTime { get; set; }

        public bool SnapshotReceived { get; set; }

        public TimeSpan Delay { get; set; }

        public int Depth;

        public bool UpdateWithSnapshot(Dictionary<BookSides, double[]> prices)
        {
            return true;
        }

        /// <summary>
        /// Возвращает взвешенную по объему цену для заданного объема amount (USD) и стороны книги side.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public double GetWAPrice(double amount, BookSides side)
        {
            double sum = 0;
            double multisum = 0;
            var quotes = side == BookSides.Ask ? Asks.ToArray() : Bids.ToArray();  

               foreach (var kvp in quotes)
                {
                    multisum += kvp.Key * kvp.Value;
                    sum += kvp.Value;

                    if (multisum >= amount)
                    {
                        double rest = amount - multisum;
                        double restsum = rest / kvp.Key;
                        return Math.Round((multisum + rest) / (sum + restsum), Decimals);
                    }
                }

            return double.NaN;

        }


    }
}
