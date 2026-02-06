using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Crypto.Trading
{
    // Copyright(c) [2026], [Sergey Dvortsov]


    /// <summary>
    /// The orderbook parts
    /// </summary>
    public enum BookSides
    {
        Ask,
        Bid
    }

    /// <summary>
    /// The trade direction
    /// </summary>
    public enum Sides
    {
        Buy,
        Sell
    }

    /// <summary>
    /// The position direction
    /// </summary>
    public enum PositionSides
    {
        LONG,
        SHORT
    }

    /// <summary>
    /// The position state
    /// </summary>
    public enum PositionStates
    {
        Open,
        Close
    }

    /// <summary>
    /// Instrument type
    /// </summary>
    public enum InstrumentTypes
    {
        Spot,
        LinearPerpetual,
        LinearFuture,
        InversePerpetual,
        InverseFuture
    }


}
