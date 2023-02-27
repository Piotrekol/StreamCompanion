# Tournament mode

While in tourney mode, StreamCompanion supports reading and processing data from multiple tournament clients.  

## Enabling tournament mode

* **Step 1:** In `settings.ini` located in StreamCompanion install directory set `TournamentMode` to true
* **Step 2:** Start and stop StreamCompanion for it to create 2 additional settings in `settings.ini`:  
  * `TournamentClientCount` - number of active player clients
  * `TournamentDataClientId` - 0-indexed id of the client used as source of truth for map events

## Usage

Usage of StreamCompanion while in tourney mode doesn't differ from normal mode, with one exception being live token names.  
Because we are now using multiple clients, we need separate tokens to preserve all this data.  
**Live** tokens name format while in tourney mode are as follows: `client_<Id>_<tokenName>`.  
for example: `client_0_combo`, `client_2_ppIfMapEndsNow`, `client_3_unstableRate`

In tournament specific web overlays this can be easily handled by using helper method for retrieving tokens:

@[code js](./codeExample/tournamentTokens.js)
