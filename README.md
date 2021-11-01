# bot-webhooks
This repository contains the code of the web-api restful API service, which provides the possibility of automatic trading, it works in conjunction with Tradingview signals, and automatically deploys to heroku after each push to repo

1) git clone this repo
2) dotnet restore
3) dotnet run
4) send GET requvest via browser https://localhost:5001/webhookreceiver?symbol=CRVUSDT
