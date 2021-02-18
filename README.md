# Multiplayer Checkers
Authors: Jeffrey Murray Jr, Chris O’Keefe, Sam Krogh

## Project Description
Multiplayer Checkers is an an online, application-based version of American checkers. Players will use a client to connect to a matchmaking server in order to be paired up with other, with the gameplay clients communicating over the server. Players are identified in the game by a username of their choice.

In addition to being able to play a best-of-1 game of checkers against an online opponent (As per the rules here), the application will allow players to rematch their current opponent and track their overall total wins and losses. In-game, players will be able to view the name of their opponent, the current state of the board, as well as whose turn it currently is.

## Game Concept
American style checkers that allows users to play either locally or online versus other players. The game enforces all game rules that a game of checker entails, such as double jumps, move restrictions, kings, and forced moves (white under-glow).

## Controls
Left Click : Interact with buttons & Move pieces <br>
Mouse : Move pieces while holding <br>
Enter : Submit text box inputs <br>
T : Show & Hide Chat <br>

## Graphic Settings
- Resolution: Keep Recommended of 800x600 for the optimal experience

## For Developers
Previously configured to Azure VM, however can be implemented on any **public IP address** server

**To change:**
- Modify Server/server.cpp to correct ip via *ifconfig* or *ip add*
- Modify Checkers/Assets/Scripts/Client.cs serverAddress to corrected IP address
- Recompile & Generate .EXE or .APP on Unity Version **2019.2.10f1**
- Initialize server via ./runServer.sh
- Enjoy!




 
