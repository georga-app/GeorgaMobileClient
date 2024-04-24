# GeorgaMobileClient
a mobile app built using .NET MAUI

## Debugging

Easiest way to get started is by deployment on an Android emulator via the host ip address 10.0.2.2. See appsettings.json for connecting the app to a server.

If you run the GeoRGA Django server in a WSL and want to access it from the (W)LAN, first get the internal WSL ip address:

    wsl hostname -I

Then have the host listening for port 80 and connect it to the WSL adapter:

    netsh interface portproxy add v4tov4 listenport=80 listenaddress=0.0.0.0 connectport=80 connectaddress=<ip from wsl hostname command>

You might want to add an exception to the Windows firewall: Windows Settings -> Network and Internet -> Windows Firewall -> extended Settings -> Connection Rules -> New Rule. In the rule wizard choose user defined -> any ip addresses -> no authification -> protocol TCP (or all, if you need WebSocket) / port 80 -> private, then choose a name and hit ok.
