# BattleNET-Command-Interface
Allows you to send BattleEye commands to your Arma 3 servers using a database command queue system

WHAT IS IT
--
Basically to issue BattleEye command to an Arma 3 server we are required to manually log in ingame. Or to send commands through the BattleNET.dll. To help create a nice interface between your applications and the BattleNet.dll I have written a BattleNET Command Interface application.


HOW DOES IT WORK
--
All it really does is connect to a database that has a table that contains a list of all the commands you would like to execute on the server/s. The nice thing about executing commands through a server is that almost any application can use a database. Including your SQF code. So infact you could use this system to run BattleNET commands with your ingame scripts which has not previously been possible.


SETUP
--
1. In Main.cs set your database connection data
2. Create a database table called battleyecommandqueue with the import script provided
3. Populate the servers.txt file with your Arma 3 server BattleyEye connection information
4. Build and run, the location isn't important as long as the host can get a connection to the database and the arma server/s

Command Structure
--
- id: Should be set by the database, leave as null
- cid: The number for the battleEye command you want to execute from the Command List Reference below
- command: (Optional) Additional paramiters of the command, see http://www.battleye.com/doc.html for documentation on each - - time: Should be set by the database as current timestamp
- server: The number for which server you want to send the command to, this is the 1st paramiter of the server connection string in servers.txt

Example:
Bans a user with IP address 192.168.0.1 perminantly for reason "writing bad code" on server 1
And then does a write bans command to make it stick. Not sure why you have to do this -.-

INSERT INTO battleyecommandqueue (cid, command, server) VALUES (18, "192.168.0.1 0 writing bad code", 1);
INSERT INTO battleyecommandqueue (cid, command, server) VALUES (20, "", 1);

Command List Reference
--
- 0 Init
- 1 Shutdown
- 2 Reassign
- 3 Restart
- 4 Lock
- 5 Unlock
- 6 Mission
- 7 Missions
- 8 RConPassword
- 9 MaxPing
- 10 Kick
- 11 Players
- 12 Say
- 13 LoadBans
- 14 LoadScripts
- 15 LoadEvents
- 16 Bans
- 17 Ban
- 18 AddBan
- 19 RemoveBan
- 20 WriteBans

SERVERS.TXT EXPLAINED
--
This is the connection information for your arma 3 server BattleEye
If you have multiple servers just add the connection to a new line

The format for the connection is
ID,HOST,PORT,PASSWORD

ID - This is what identifies your server in the command that you insert into the database in the server field.

Example:
- 1, 127.0.0.1, 2302, toast
- 2, www.example.com, 2200, shoopdeloop
- 7, localhost, 2200, mypass

NOTES
--

It's very important that you use the provided BattleNET.dll as it has some changes that I made.
The source code for the DLL is also provided so you can see the changes I have made and make your own additions if required.

Ban list doesn't really work. It's better at this point to read from the ban log file on the server directly rather than going through the dll. Feel free to fix it though :)

The code works farely efficiently but there are some simple improvements that can be made with the database calls.
