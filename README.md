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
The map is set in a closed down space at approximately 35x20 world units.
The controls are WASD to move. Mouse position to aim and rotate the player. Space to shoot. Esc to open the menu.
Each time you hit another player with your bullet, you gain one score. 

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
## **Network features:**

### **Player:**
For the player I'm seperating the movement for the local client and the rest of the clients. I'm using one varible to update the movement locally and a network varible to synchronize it across every client. This is to ensure that the movement of the player runs and feels as smooth as possible.  

    private Vector2 localMoveInput;
    private NetworkVariable<Vector2> serverMoveInput;
    private NetworkVariable<Quaternion> serverRotation;

    void Update()
    {
        if (IsOwner) // Update local stuff
        {
            transform.position += (Vector3)localMoveInput * speed * Time.deltaTime;
            UpdateRotation();
        }
        else // Update from server as client
        {
            transform.position += (Vector3)serverMoveInput.Value * speed * Time.deltaTime;
            transform.rotation = serverRotation.Value;
        }
    }




Network features.

Challnges and faced solutions.
Realized you can't call server function as a client in OnClientDisconnect


Reflection.

Update local first:
Movement -> position, rotation

Seperating local and network varibles (not most efficient but much more clear whats going on)
