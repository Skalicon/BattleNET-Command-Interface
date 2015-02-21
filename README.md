# BattleNET-Command-Interface
Allows you to send BattleEye commands to your Arma 3 servers using a database command queue system

-- WHAT IS IT --

Basically to issue BattleEye command to an Arma 3 server we are required to manually log in ingame. Or to send commands through the BattleNET.dll. To help create a nice interface between your applications and the BattleNet.dll I have written a BattleNET Command Interface application.


-- HOW DOES IT WORK --

All it really does is connect to a database that has a table that contains a list of all the commands you would like to execute on the server/s. The nice thing about executing commands through a server is that almost any application can use a database. Including your SQF code. So infact you could use this system to run BattleNET commands with your ingame scripts which has not previously been possible.


-- SETUP --

1. In Main.cs set your database connection data
2. Create a database table called battleyecommandqueue with the import script provided
3. Populate the servers.txt file with your Arma 3 server BattleyEye connection information
4. Build and run


-- NOTES --

It's very important that you use the provided BattleNET.dll as it has some changes that I made.
The source code for the DLL is also provided so you can see the changes I have made and make your own additions if required.

Ban list doesn't really work. It's better at this point to read from the ban log file on the server directly rather than going through the dll. Feel free to fix it though :)

The code works farely efficiently but there are some simple improvements that can be made with the database calls.
