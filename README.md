# CSS432 [Project Proposal](https://docs.google.com/document/d/105CCw2I-2erSvRIkoGeBQuCgsMpgwbm8mEWOLpMxZOA/edit?usp=sharing)

to be updated...

## General Guidelines
- Unity Version: 2019.2.10f1
- always branch from master
- assuming some basic knowledge of unity & github

## Schedule
| Date | Goals  
|---   |---     
| 1/26 | Refine project idea, design architecture  
| 2/2  | Finalize Idea, begin prototyping  
| 2/9  | Work on basic game rules and network communication  
| 2/16 | Finish game rules and win/loss condition
| 2/23 | Work on the online multiplayer
| 3/1  | Finish online multiplayer, add additional features
| 3/8  | Finish additional features
| 3/15 | Final polish
---

## Server
Currently configured to Azure VM
To change:
- Modify Server/server.cpp to correct ip via ifconfig
- Modify Checkers/Assets/Scripts/Client.cs serverAddress to corrected IP address
- Initialize server via ./runServer.sh
- Enjoy!




 
