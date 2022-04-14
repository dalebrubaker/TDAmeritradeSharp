C# data and trading library for TD Ameritrade
=

1. Desktop program to assist setting access token for the https://developer.tdameritrade.com/ APIs.
2. Library for the TD Ameritrade developer APIs.

The Desktop program is .Net 6 Winform
The Library is .net standard 2.0

Credits
=
Big thanks to Nicholas Ventimiglia and his .Net 5 web-based project: https://github.com/NVentimiglia/TDAmeritrade.DotNetCore

Supported APIs
=
Orders
-
* Cancel Order
* Get Order
* Get Orders By Path
* Get Orders By Query
* Place Order
* Replace Order

Saved Orders
-
* Create Saved Order
* Delete Saved Order
* Get Saved Order
* Get Saved Orders by Path
* Replace Saved Orders

Accounts
-
* Get Account
* Get Accounts

Authentication
-
* Post Access Token

Instruments
-

Market Hours
-
* Get Hours for a Single Market

Movers
-

Option Chains
-
* Get Option Chain

Price History
-
* Get Price History

Transaction History
-

User Info & Preferences
-
* Get User Principals

Streaming Data (ClientStream)
-
* Subscribe/Unsubscribe Chart
* Subscribe/Unsubscribe Quote
* Subscribe/Unsubscribe Time & Sales
* Subscribe/Unsubscribe Book
* RequestQOS


Unsupported APIs (Maybe easy to support, but not supported yet)
=


Instruments
- 
* Search Instruments
* Get Instrument

Market Hours
-
* Get Hours for Multiple Markets

Movers
-
* Get Movers

Transaction History
-
* Get Transaction
* Get Transactions

User Info & Preferences
-
* Get Preferences
* Get Streamer Subscription Keys
* Update Preferences

Watchlist
-
* Create Watchlist
* Delete Watchlist
* Get Watchlist
* Get Watchlists for Multiple Accounts
* Get Watchlists for Single Account
* Replace Watchlist
* Update Watchlist

