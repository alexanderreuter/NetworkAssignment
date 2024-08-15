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

Everything else that's network related is initially called only on the server. This is to maintain a server authority structer. By having a set up where the clients sends instructions to the server, the server validates the information, and then updates each client accordingly. This ensures a level playing field by centralizing game mechanics and enforcing consistent rules for all players. For example, the client could try to spawn a bullet in location 1x1, but the server would know that correct position is 2x2 and adjust the position accordingly. 

Here's the bullet spawn function called on the server:
The bulletNetworkObject.Spawn is used to spawn and synchronize the bullet on every client.

    [ServerRpc]
    private void ShootServerRpc(ulong playerID)
    {
        if (bulletPrefab && shootPoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            
            NetworkObject bulletNetworkObject = bullet.GetComponent<NetworkObject>();
            if (bulletNetworkObject != null)
            {
                bulletNetworkObject.Spawn(true); 
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.Initialize(shootPoint.up, playerID);
            }
        }
    }

Lastly we have the UI for the game. It works very similar to the bullet, it's first set up and updated on the server, after that it's synchronized for every client by gathering the data from the server. The only difference is that there's data that needs to be valided for each seperate player, this it to ensure that the score is updated for the correct player. There also needs to be logic to remove the UI whenever a player disconnects.

Here's the UI setup called on the server:
RecieveSetPlayerUIClientRpc(clientId, removeUI, value + 1) is the function that then is ran for each seperate client to receive the data from the serever.

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerUIServerRpc(ulong clientId, bool removeUI, int playerScore)
    {
        if (playerScore == 0 || removeUI)
        {
            RecieveSetPlayerUIClientRpc(clientId, removeUI, playerScore);
        }
        else
        {
            if (playerScoreDictionary.TryGetValue(clientId, out int value))
            {
                playerScoreDictionary[clientId] = value + 1;
                RecieveSetPlayerUIClientRpc(clientId, removeUI, value + 1);
            }
        }
        UpdateScoreClientRpc(player1Text.text, player2Text.text, player3Text.text, player4Text.text);
    }
    
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
## **Challnges and faced solution:**

For me, I think most of the assignment went pretty smooth and I didn't have that manny hurdles. However, there were three things that 


* Realized you can't call server function as a client in OnClientDisconnect
asdsad


Reflection.

Seperating local and network varibles (not most efficient but much more clear whats going on)
