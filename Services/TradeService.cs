using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Enums;
using bot_webhooks.Data;
using bot_webhooks.Helpers;
using bot_webhooks.Models;
using RestSharp;

namespace bot_webhooks.Services
{
    public class TradeService : ITradeRepo
    {
        private readonly WebHookContext _db;

        public TradeService(WebHookContext db) => (_db) = (db);

        public Task BinanceAccountInfo()
        {
            // Установить API-ключ и секретный ключ Binance
            string apiKey = "Ваш API-ключ";
            string apiSecret = "Ваш секретный ключ";

            // Создать клиент RestSharp и задать базовый URL API Binance
            var client = new RestClient("https://api.binance.com");

            // Создать запрос для получения информации об аккаунте
            var request = new RestRequest("/api/v3/account", Method.Get);

            // Добавить параметры запроса
            request.AddParameter("timestamp", GetTimestamp());
            request.AddParameter("recvWindow", "5000");
            request.AddParameter("signature", GetSignature(apiSecret, request.Parameters));

            // Добавить заголовок с API-ключом Binance
            request.AddHeader("X-MBX-APIKEY", apiKey);

            // Выполнить запрос и получить ответ
            var response = client.Execute(request);

            // Вывести ответ на консоль
            Console.WriteLine(response.Content);

            return null;
        }

        public Task BinanceFutureOrder()
        {
            // Установить API-ключ и секретный ключ Binance
            string apiKey = "Ваш API-ключ";
            string apiSecret = "Ваш секретный ключ";

            // Создать клиент RestSharp и задать базовый URL API Binance
            var client = new RestClient("https://fapi.binance.com");

            // Создать запрос для размещения ордера на продажу
            var request = new RestRequest("/fapi/v1/order", Method.Post);

            // Добавить параметры запроса для размещения ордера на продажу
            request.AddParameter("symbol", "BTCUSDT"); // Пара торгов
            request.AddParameter("side", "SELL"); // Направление ордера на продажу
            request.AddParameter("type", "LIMIT"); // Тип ордера - лимитный
            request.AddParameter("timeInForce", "GTC"); // Срок действия ордера - действует до отмены
            request.AddParameter("quantity", "0.001"); // Количество криптовалюты для продажи
            request.AddParameter("price", "60000"); // Цена продажи

            // Добавить заголовок с API-ключом Binance
            request.AddHeader("X-MBX-APIKEY", apiKey);

            // Добавить параметры запроса для подписи секретным ключом
            request.AddParameter("timestamp", GetTimestamp());
            request.AddParameter("recvWindow", "5000");
            request.AddParameter("signature", GetSignature(apiSecret, request.Parameters));

            // Выполнить запрос и получить ответ
            var response = client.Execute(request);

            // Вывести ответ на консоль
            Console.WriteLine(response.Content);
            return null;
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