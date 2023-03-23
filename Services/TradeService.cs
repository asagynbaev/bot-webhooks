using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Futures.FuturesData;
using bot_webhooks.Data;
using bot_webhooks.Helpers;
using bot_webhooks.Models;
using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;
using RestSharp;

namespace bot_webhooks.Services
{
    public class TradeService : ITradeRepo
    {
        private readonly WebHookContext _db;
        private IBinanceSocketClient _socketClient;
        private UpdateSubscription _subscription;
        private IBinanceClient _client;

        public TradeService(WebHookContext db, IBinanceSocketClient socketClient, IBinanceClient client)
        {
            _db = db;
            _socketClient = socketClient;
            _client = client;
        }

        public async Task<BinanceFutureAccountInfo> BinanceAccountInfo()
        {
            var response = await _client.FuturesUsdt.Account.GetAccountInfoAsync();
            var binanceFuture = JsonConvert.SerializeObject(response.Data);
            BinanceFutureAccountInfo resultInfo = JsonConvert.DeserializeObject<BinanceFutureAccountInfo>(binanceFuture);
            //await _db.BinanceFutureAccountInfos.AddAsync(resultInfo);
            //await _db.SaveChangesAsync();
            return resultInfo;
        }

        public async Task<BinanceFuturesPlacedOrder> BinanceFutureOrder()
        {
            var trade = await _client.FuturesUsdt.Order.PlaceOrderAsync(
                                  symbol: "BTCUSDT",
                                  side: OrderSide.Buy,
                                  type: OrderType.Limit,
                                  quantity: 1,
                                  price: 0.002m,
                                  orderResponseType: OrderResponseType.Result,
                                  positionSide: PositionSide.Long,
                                  workingType: WorkingType.Mark,
                                  timeInForce: TimeInForce.GoodTillCancel);
            var result = trade.Data;
            return result;
        }

        // Метод для получения временной метки в миллисекундах
        private static long GetTimestamp()
        {
            var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
        }

        // Метод для генерации подписи запроса с использованием секретного ключа
        private static string GetSignature(string apiSecret, RestSharp.ParametersCollection parameters)
        {
            var queryString = string.Join("&", parameters
                .OrderBy(p => p.Name)
                .Select(p => $"{p.Name}={p.Value}"));

            return Helpers.HmacSHA256(apiSecret, queryString);
        }

        // Класс для хэширования HMAC-SHA256
        public static class Helpers
        {
            public static string HmacSHA256(string key, string message)
            {
                var encoding = new System.Text.ASCIIEncoding();
                byte[] keyBytes = encoding.GetBytes(key);
                byte[] messageBytes = encoding.GetBytes(message);
                using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(keyBytes))
                {
                    byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                    return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
                }
            }
        }

        public Trade GetTrade(string symbol, int direction) =>
            _db.Trades.Where(x => x.Symbol == symbol)
            .Where(x => x.Direction == direction)
            .FirstOrDefault();

        public async Task<int> InsertTrade(Signal signal, int signalId)
        {
            var newTrade = new Trade()
            {
                Price = signal.Price,
                SignalID = signalId,
                Symbol = signal.Symbol,
                Direction = signal.Direction,
                SignalType = signal.SignalType
            };
            decimal _myBalance = 0;
            decimal _tokensAmount = 0;

            if (signal.Direction == 0)
                _myBalance = await Balance.GetBalance();
            else
                _tokensAmount = await Balance.GetTokensAmount(signal.Symbol);

            using (var client = new BinanceClient())
            {
                var result = await client.Spot.Order.PlaceOrderAsync(
                    signal.Symbol,
                    (OrderSide)signal.Direction,
                    OrderType.Market,
                    _tokensAmount == 0 ? null : _tokensAmount, // SymbolAmount
                    _myBalance == 0 ? null : _myBalance, // quoteSymbolAmount
                    null, null, null, null, null, null, null, default // Unnecessary parameters
                );

                if (result.Success)
                {
                    foreach (var item in result.Data.Fills)
                    {
                        newTrade.BoughtPrice = item.Price;
                        newTrade.BuyQuoteCommission = item.Commission;
                        newTrade.Amount = item.Quantity;
                    }
                }
                else
                {
                    await TelegramMessenger.SendMessage($"Error");
                }
            }

            await _db.AddAsync(newTrade);
            await _db.SaveChangesAsync();
            int id = newTrade.ID;

            await TelegramMessenger.SendMessage($"Bought: {newTrade.Symbol}, Price: {newTrade.BoughtPrice}");

            return id;
        }

        public Trade UpdateTrade(int ID)
        {
            throw new System.NotImplementedException();
        }
    }
}