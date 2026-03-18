using Synapse.General;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace Synapse.Crypto.Trading
{
    public readonly record struct Trade(int ID, DateTime Time, double Price, double Size, string Side)
    {

        public static Trade Parse(string line)
        {
            var arr = line.Split(',');
            int id = int.Parse(arr[0]);
            long ts = long.Parse(arr[1]);
            DateTime time = DateTimeOffset.FromUnixTimeMilliseconds(ts).UtcDateTime;
            double price = double.Parse(arr[2], CultureInfo.InvariantCulture);
            double size = double.Parse(arr[3], CultureInfo.InvariantCulture);
            string side = arr[4];
            return new Trade(id, time, price, size, side);
        }

        public static Trade PerpParse(string line)
        {
            //0 timestamp,
            //1 symbol,
            //2 side,
            //3 size,
            //4 price,
            //5 tickDirection,
            //6 trdMatchID,
            //7 grossValue,
            //8 homeNotional,
            //9 foreignNotional,
            //10 RPI
            var arr = line.Split(',');

            //int id = int.Parse(arr[6]);
            int id = 0;
            double ts = double.Parse(arr[0], CultureInfo.InvariantCulture);
            DateTime time = ts.UnixTimeToDateTimeFromPartSeconds();
            string side = arr[2];
            double size = double.Parse(arr[3], CultureInfo.InvariantCulture);
            double price = double.Parse(arr[4], CultureInfo.InvariantCulture);
            return new Trade(id, time, price, size, side);
        }

        public static Trade BfxParse(string line)
        {
            // #,TIME,PRICE,AMOUNT,PAIR
            // 1871630547,14-02-26 23:59:24.198,69783,0.00573349,BTC / UST

            var arr = line.Split(',');

            int id = int.Parse(arr[0]);
            DateTime time = DateTime.Parse(arr[1]);
            double size = double.Parse(arr[3], CultureInfo.InvariantCulture);
            string side = size > 0 ? "b" : "s";
            double price = double.Parse(arr[2], CultureInfo.InvariantCulture);
            return new Trade(id, time, price, Math.Abs(size), side);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{ID};");
            sb.Append($"{Time.ToString("dd.MM.yyyy HH:mm:ss.ffff")};");
            sb.Append($"{Price};");
            sb.Append($"{Size};");
            sb.Append($"{Side}");
            return sb.ToString();
        }   


    }


    //    timestamp,symbol,side,size,price,tickDirection,trdMatchID,grossValue,homeNotional,foreignNotional,RPI
    //1771027200.1338,BTCUSDT,Buy,0.087,68819.20,ZeroPlusTick,fe4e12a8-31f4-5097-bf48-5f85315221f2,5.9872704e+11,0.087,5987.270399999999,0


    public readonly record struct AggTrade
    {
        public int ID { get; init; }
        public DateTime Time { get; init; }
        public DateTime LocalTime { get; init; }
        public double StartPrice { get; init; }
        public double EndPrice { get; init; }
        public double AvgPrice { get; init; }
        public double PriceDelta
        {
            get => Math.Abs(EndPrice - StartPrice);
        }

        public double MaxPrice
        {
            get => Math.Max(EndPrice, StartPrice);
        }

        public double MinPrice
        {
            get => Math.Min(EndPrice, StartPrice);
        }

        public int Count { get; init; }

        /// <summary>
        /// Объем в базовой валюте
        /// </summary>
        public double Size { get; init; }
        /// <summary>
        /// Объем в валюте котирования
        /// </summary>
        public double Volume { get; init; }
        public double MaxSize { get; init; }
        public double MinSize { get; init; }
        public string Side { get; init; }
        public double LgVol { get; init; }
        public double LgRank { get; init; }

        public AggTrade GetCloneWithRank(double rank)
        {
            var trade = new AggTrade()
            {
                ID = this.ID,
                Time = this.Time,
                LocalTime = this.LocalTime,
                StartPrice = this.StartPrice,
                EndPrice = this.EndPrice,
                AvgPrice = this.AvgPrice,
                Count = this.Count,
                Size = this.Size,
                MaxSize = this.MaxSize,
                MinSize = this.MinSize,
                Volume = this.Volume,
                Side = this.Side,
                LgVol = this.LgVol,
                LgRank = rank
            };

            return trade;

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{ID};"); //0
            sb.Append($"{Time.ToString("dd.MM.yyyy HH:mm:ss.ffff")};"); //1
            sb.Append($"{LocalTime.ToString("dd.MM.yyyy HH:mm:ss.ffff")};"); //2
            sb.Append($"{StartPrice};"); //3
            sb.Append($"{EndPrice};"); //4
            sb.Append($"{AvgPrice};"); //5
            sb.Append($"{Count};"); //6
            sb.Append($"{Size};"); //7
            sb.Append($"{MaxSize};"); //8
            sb.Append($"{MinSize};"); //9
            sb.Append($"{Volume};"); //10
            sb.Append($"{Side};"); //11
            sb.Append($"{LgVol};"); //12
            sb.Append($"{LgRank}"); //13
            return sb.ToString();
        }

        public static AggTrade Parse(string line, int decimals)
        {
            var arr = line.Split(';');

            double startPrice = Math.Round(double.Parse(arr[3], CultureInfo.CurrentCulture), decimals);
            double endPrice = Math.Round(double.Parse(arr[4], CultureInfo.CurrentCulture), decimals);

            return new()
            {
                ID = int.Parse(arr[0]),
                Time = DateTime.Parse(arr[1]),
                StartPrice = startPrice,
                EndPrice = endPrice,
       
                AvgPrice = Math.Round(double.Parse(arr[5], CultureInfo.CurrentCulture), decimals),
                Count = int.Parse(arr[6]),
                Size = double.Parse(arr[7], CultureInfo.CurrentCulture),
                MaxSize = double.Parse(arr[8], CultureInfo.CurrentCulture),
                MinSize = double.Parse(arr[9], CultureInfo.CurrentCulture),
                Volume = double.Parse(arr[10], CultureInfo.CurrentCulture),
                Side = arr[11],
                LgVol = double.Parse(arr[12], CultureInfo.CurrentCulture),
                LgRank = Math.Round(double.Parse(arr[13], CultureInfo.CurrentCulture), 2)
            };


        }

    }

    public class AggTradeCreator
    {

        public AggTradeCreator(Trade trade, int id, DateTime? localtime = null)
        {
            ID = id;
            Time = trade.Time;
            LocalTime = localtime ?? DateTime.MinValue;
            StartPrice = trade.Price;
            EndPrice = trade.Price;
            AvgPrice = trade.Price;
            Count = 1;
            Size = trade.Size;
            MaxSize = trade.Size;
            MinSize = trade.Size;
            Volume = trade.Size * trade.Price;
            Side = trade.Side;
        }

        public AggTradeCreator(int id, DateTime time, double price, double volume, string side, DateTime localtime)
        {
            ID = id;
            Time = time;
            LocalTime = localtime;
            StartPrice = price;
            EndPrice = price;
            AvgPrice = price;
            Count = 1;
            Size = volume;
            MaxSize = volume;
            MinSize = volume;
            Side = side;
        }


        public int ID { set; get; }
        public DateTime Time { set; get; }
        public DateTime LocalTime { set; get; }
        public double StartPrice { set; get; }
        public double EndPrice { set; get; }
        public double AvgPrice { set; get; }
        public int Count { set; get; }
        public double Size { set; get; }
        public double MaxSize { set; get; }
        public double MinSize { set; get; }
        public double Volume { set; get; }
        public string Side { set; get; }

        public void Aggregate(Trade t)
        {
            EndPrice = t.Price;
            AvgPrice = ((AvgPrice * Size) + (t.Price * t.Size)) / (Size + t.Size);
            Count += 1;
            Size += t.Size;
            MaxSize = Math.Max(MaxSize, t.Size);
            MinSize = Math.Min(MinSize, t.Size);
            Volume += t.Size * t.Price;    
        }

        public void Aggregate(double price, double size)
        {
            EndPrice = price;
            AvgPrice = ((AvgPrice * Size) + (price * size)) / (Size + size);
            Count += 1;
            Size += size;
            MaxSize = Math.Max(MaxSize, size);
            MinSize = Math.Min(MinSize, size);
            Volume += size * price;
        }

        public AggTrade Create()
        {
            return new AggTrade
            {
                ID = this.ID,
                Time = this.Time,
                LocalTime = this.LocalTime,
                StartPrice = this.StartPrice,
                EndPrice = this.EndPrice,
                AvgPrice = this.AvgPrice,
                Count = this.Count,
                Size = this.Size,
                MaxSize = this.MaxSize,
                MinSize = this.MinSize,
                Volume = this.Volume,
                Side = this.Side,
                LgVol = Math.Log(Size)
            };
        }

    }

}
