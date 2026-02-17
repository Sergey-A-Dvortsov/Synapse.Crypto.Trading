using NLog;
using Synapse.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synapse.Crypto.Trading
{
    public abstract class OrderBook
    {

        private readonly object _lock = new();

        private TimeSpan spanSum = TimeSpan.Zero; 

        public OrderBook(InstrumentTypes type, string symbol, double ticksize, int decimals)
        {
            Type = type;
            Symbol = symbol;
            TickSize = ticksize;
            Decimals = decimals;
            Asks = [];
            Bids = new(Comparer<double>.Create(static (a, b) => b.CompareTo(a)));
        }

        public OrderBook(InstrumentTypes type, string symbol) : this(type, symbol, double.NaN, -1)
        {
        }

        /// <summary>
        /// Число событий
        /// </summary>
        public long EventCount { set; get; }

        private DateTime _lastTime;
        public DateTime LastTime 
        { 
            get => _lastTime;
            set 
            {
                spanSum += LastTime != DateTime.MinValue ? value - LastTime : spanSum;
                _lastTime = value;
            } 
        }

        public TimeSpan AvgEventInterval
        {
            get => EventCount != 0 ? spanSum / EventCount : TimeSpan.Zero; 
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

        //public Quote? BestAsk
        //{
        //    get
        //    {
        //        lock (_lock)
        //        {
        //            if(Asks.Count == 0) return null;

        //            try
        //            {

        //            }
        //            catch (Exception ex)
        //            {

        //                throw;
        //            }
        //            var ask = Asks.First();
        //            return new Quote(ask.Key, ask.Value);
        //        }
        //    }
        //}

        public Quote? BestAsk { set; get; }

        public Quote? BestBid { set; get; }

        //public Quote? BestBid
        //{
        //    get
        //    {
        //        lock (_lock)
        //        {
        //            if (Bids.Count == 0) return null;
        //            var bid = Bids.First();
        //            return new Quote(bid.Key, bid.Value);
        //        }
        //    }
        //}

        //public abstract DateTime UpdateTime { get; }

        public DateTime SnapshotTime { get; set; }

        public bool SnapshotReceived { get; set; }

        public TimeSpan Delay { get; set; }

        public int Depth;

        //public bool UpdateWithSnapshot(Dictionary<BookSides, double[]> prices)
        //{
        //    return true;
        //}

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
            KeyValuePair<double, double>[]? quotes = null;

            if (side == BookSides.Ask)
            {
                if (Asks.Count == 0) return double.NaN;
                lock (_lock)
                {
                    quotes = [.. Asks];
                }
            }
            else if (side == BookSides.Bid)
            {
                if (Bids.Count == 0) return double.NaN;
                lock (_lock)
                {
                    quotes = [.. Bids];
                }
            }

            if (quotes == null) return double.NaN;

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
