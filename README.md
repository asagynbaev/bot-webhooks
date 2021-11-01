# bot-webhooks
This repository contains the code of the web-api restful API service, which provides the possibility of automatic trading, it works in conjunction with Tradingview signals, and automatically deploys to heroku after each push to repo.

1) git clone this repo
2) add .env file (TELEGRAM_BOT_ID, TELEGRAM_CHANNEL_ID)
3) dotnet restore
4) dotnet run
5) send GET requvest via browser https://localhost:5001/webhookreceiver?symbol=CRVUSDT
