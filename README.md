# bot-webhooks

REST API service for automated execution of trading signals. Consumes webhooks (e.g. TradingView), executes spot and futures logic on Binance and exposes account data for Binance and Bybit. Notifications are sent via Telegram.

**Stack:** .NET 8, ASP.NET Core, Entity Framework Core 8, PostgreSQL, Binance.Net 12, Bybit.Net 6.

---

## Requirements

- .NET 8 SDK  
- PostgreSQL (for signals, trades, users)  
- Binance API keys (spot and USDT-M futures)  
- Optional: Bybit API keys (unified account), Telegram bot token and chat/channel ID  

---

## Configuration

Configuration is read from `appsettings.json` and from environment variables loaded from a `.env` file in the project root (via DotNetEnv). Create `.env` before running.

### Environment variables

| Variable | Required | Description |
|----------|----------|-------------|
| `BINANCE_API_KEY` | Yes | Binance API key (spot and USDT-M) |
| `BINANCE_SECRET` | Yes | Binance API secret |
| `BYBIT_API_KEY` | No | Bybit API key (unified account). Omit to disable Bybit endpoints. |
| `BYBIT_SECRET` | No | Bybit API secret |
| `TELEGRAM_BOT_ID` | No | Telegram bot token for outgoing notifications |
| `TELEGRAM_CHANNEL_ID` | No | Telegram chat/channel ID for notifications |
| `TELEGRAM_API_ID` | No | Telegram app api_id (from https://my.telegram.org/apps) — for receiving channel messages |
| `TELEGRAM_API_HASH` | No | Telegram app api_hash — for receiving channel messages |
| `TELEGRAM_PHONE_NUMBER` | No | Phone number of the account that is a member of the signal channel (e.g. +79...) |
| `TELEGRAM_VERIFICATION_CODE` | No | One-time code from Telegram (first run only); can be set in .env and restart |
| `TELEGRAM_2FA_PASSWORD` | No | 2FA cloud password if the account has it |
| `TELEGRAM_SIGNAL_CHANNEL_USERNAME` | No | Channel @username (without @) to listen for signal messages |
| `TELEGRAM_SIGNAL_CHANNEL_ID` | No | Channel numeric id (alternative to username) to filter messages |

### Database

Set the connection string in `appsettings.json` under `ConnectionStrings:DefaultConnection` (PostgreSQL). Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=127.0.0.1;Port=5432;Database=BotTrader;Username=postgres;Password=your_password;"
}
```

**Telegram channel listener (receiving signals):** The app connects as a **user** (not a bot) using MTProto (WTelegramClient). The account must be a member of the private channel. Get `api_id` and `api_hash` from https://my.telegram.org/apps. On first run, set `TELEGRAM_VERIFICATION_CODE` to the code Telegram sends, then restart; the session is stored and further runs do not need the code. Set either `TELEGRAM_SIGNAL_CHANNEL_USERNAME` (e.g. `my_signals`) or `TELEGRAM_SIGNAL_CHANNEL_ID` so only that channel is processed.

Apply migrations:

```bash
dotnet ef database update
```

---

## Build and run

```bash
git clone <repository-url>
cd bot-webhooks
cp .env.example .env   # if present; otherwise create .env with the variables above
dotnet restore
dotnet run
```

By default the API listens on the URLs configured in `Properties/launchSettings.json` (e.g. `https://localhost:5001`, `http://localhost:5000`). In Development, Swagger UI is available at `/swagger`.

---

## API

Base route for the webhook receiver: `/WebHookReceiver`.

| Method | Path | Description |
|--------|------|-------------|
| GET | `/WebHookReceiver/Info` | Binance USDT-M futures account summary (margins, balances, permissions). |
| GET | `/WebHookReceiver/BybitInfo` | Bybit V5 unified account wallet balance (equity, wallet balance, available balance, margins). Returns 404 if Bybit credentials are not set or the request fails. |
| GET | `/WebHookReceiver/TelegramMessages` | Last messages received from the configured Telegram signal channel. Query: `count` (default 50, max 500). |

Responses are JSON. All relevant endpoints support `CancellationToken` (e.g. for client disconnect and timeouts).

---

## Project layout

- **Controllers** — HTTP endpoints (webhook receiver, Binance/Bybit info).  
- **Services** — Trading and exchange logic: `TradeService`, `BinanceBalanceProvider`, `BybitAccountService`, `SignalService`, `UserService`; `TelegramChannelListenerService` (background, receives messages from Telegram channel via userbot); `TelegramChannelMessageStore` (in-memory buffer of last messages).  
- **Data** — EF Core `WebHookContext`, repository interfaces and entities (signals, trades, users, Binance account snapshot).  
- **Models** — DTOs and entities (e.g. `BinanceFutureAccountInfo`, `BybitAccountInfo`, `Signal`, `Trade`).  
- **Helpers** — Utilities (e.g. `TelegramMessenger`, `Futures` helper).  
- **Migrations** — EF Core migrations for PostgreSQL.  

---

## Deployment

The application is a standard ASP.NET Core 8 web app. Deploy to any host that supports .NET 8 (IIS, Linux with Kestrel, Docker, Heroku with .NET buildpack, etc.). Ensure:

- `ASPNETCORE_ENVIRONMENT` is set appropriately (e.g. `Production`).  
- All required environment variables (Binance keys, DB connection string, optional Bybit/Telegram) are set in the hosting environment; do not rely on a `.env` file in production unless your deployment explicitly loads it.  
- PostgreSQL is reachable and migrations have been applied.  

---

## License

See repository license file.
