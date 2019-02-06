#BtcPayServer-Installer

```
git clone https://github.com/Kukks/btcpayserver-installer
dotnet restore
cd btcpayserver-installer/BTCPayServer.Installer/ClientApp
npm i
cd ../
dotnet run
```

`docker run -d -p 666:80 --name btcpayserver-installer  kukks/btcpayserver-installer:latest`