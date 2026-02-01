using NLog;
using Synapse.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Crypto.Trading
{
    /// <summary>
    /// Represents a price and size pair in a financial quote, such as a bid or ask in a market order book.
    /// </summary>
    /// <param name="Price">The quoted price for the asset. Typically represents the bid or ask price, depending on context.</param>
    /// <param name="Size">The quantity available at the quoted price. Must be a non-negative value.</param>
    public readonly record struct Quote(double Price, double Size);

    /// <summary>
    /// Represents an abstract order book for a financial instrument, providing access to bid and ask quotes, best
    /// prices, and book state information.
    /// </summary>
    /// <remarks>FastBook serves as a base class for managing and updating the state of an order book,
    /// including bid and ask arrays, best prices, and update timing. It is designed for high-performance scenarios
    /// where efficient access to order book data is required. Derived classes should implement the UpdateTime property
    /// to indicate when the book was last updated. Thread safety is not guaranteed; callers should ensure appropriate
    /// synchronization if accessing instances from multiple threads.</remarks>
    public abstract class FastBook
    {
        private readonly double ticksize;
        private readonly int decimals;
        private readonly double offset; // смещение от границ котировок книги заявок в %;

        public FastBook(string symbol, double ticksize, double offset = 0.01)
        {
            Symbol = symbol;
            this.ticksize = ticksize;
            this.decimals = ticksize.GetDecimals();
            this.offset = offset;
        }

        public Logger logger = LogManager.GetCurrentClassLogger();

        public string Symbol { get; private set; }

        public bool Valid { get; set; }

        public Quote[] Asks { get; private set; }

        public Quote[] Bids { get; private set; }

        public Quote BestAsk { get => Asks[BestAskIndex]; }

        public Quote BestBid { get => Bids[BestBidIndex]; }

        internal int BestAskIndex { get; set; }

        internal int BestBidIndex { get; set; }

        public abstract DateTime UpdateTime { get; }

        public TimeSpan Delay { get; set; }

        internal double ZeroAskPrice;
        internal double ZeroBidPrice;

        internal int Depth;

        public bool UpdateWithSnapshot(Dictionary<BookSides, double[]> prices)
        {

            var prcs = prices[BookSides.Ask];

            ZeroAskPrice = GetOffsetPrice(prcs[0], -1); // цена первой котировки будущего массива Asks
            double lastAsk = GetOffsetPrice(prcs[1], 1); // цена последней котировки будущего массива Asks
            int askdepth = (int)((lastAsk - ZeroAskPrice) / ticksize);

            prcs = prices[BookSides.Bid];
            ZeroBidPrice = GetOffsetPrice(prcs[0], 1); // цена первой котировки будущего массива Bids
            double lastBid = GetOffsetPrice(prcs[1], -1); // цена последней котировки будущего массива Bids

            int biddepth = (int)((ZeroBidPrice - lastBid) / ticksize);

            Depth = Math.Max(askdepth, biddepth); // размер будущих массивов

            Asks = new Quote[Depth];
            Bids = new Quote[Depth];

            for (int i = 0; i < Depth; i++)
            {
                double askprice = Math.Round(ZeroAskPrice + ticksize * i, decimals);
                double bidprice = Math.Round(ZeroBidPrice - ticksize * i, decimals);

                if (askprice == prices[BookSides.Ask][0])
                    BestAskIndex = i;

                if (bidprice == prices[BookSides.Bid][0])
                    BestBidIndex = i;

                Asks[i] = new Quote(askprice, 0);
                Bids[i] = new Quote(bidprice, 0);
            }

            return true;

        }

        /// <summary>
        ///  Возвращает цену со смещением offset от заданной цены вверх, если k=1, или вниз, если k=-1.
        /// </summary>
        /// <param name="price"></param>
        /// <param name="k">задает направление смещения, вверх (k=1), вниз, если k=-1</param>
        /// <returns></returns>
        private double GetOffsetPrice(double price, int k)
        {
            return Math.Round((price + k * (price * offset)).PriceRound(ticksize), decimals);
        }

        /// <summary>
        ///  Возвращает индекс котировки
        /// </summary>
        /// <param name="price">цена</param>
        /// <param name="side">Bid/Ask</param>
        /// <returns></returns>
        public int GetIndex(double price, BookSides side)
        {
            int index = side == BookSides.Ask ?
                (int)Math.Round(((price - ZeroAskPrice) / ticksize), 0) :
                (int)Math.Round(((ZeroBidPrice - price) / ticksize), 0);
            return index;
        }

    }
}
