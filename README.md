# Network assignment
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
## **Author:**
Alexander Reuter

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
## **Unity version: 2022.3.41f1**
Additional packages used: 
* Netcode for GameObjects
* Input System
* TextMeshPro

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
## **Game design:**
The game is a very basic top-down 2D shooter.
The map is set in a closed down space at 35x20 world units.
The controls are WASD to move. Mouse position to aim and rotate the player. Space to shoot. Esc to open the menu.
Each time you hit another player with your bullet, you gain one score. 

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
## **Network features:**

### **Player:**
For the player I'm first updating the transform locally on each client. This is to ensure that the movement of the player runs and feels as smooth as possible. 

Downside....


Network features.

Challnges and faced solutions.
Realized you can't call server function as a client in OnClientDisconnect


Reflection.

Update local first:
Movement -> position, rotation

Seperating local and network varibles (not most efficient but much more clear whats going on)
