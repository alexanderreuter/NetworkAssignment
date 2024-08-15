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

Everything else that's network related is initially called only on the server. This is to maintain a server authority structure. By having a set up where the clients sends instructions to the server, the server validates the information, and then updates each client accordingly. This ensures a level playing field by centralizing game mechanics and enforcing consistent rules for all players. For example, the client could try to spawn a bullet in location 1x1, but the server would know that correct position is 2x2 and adjust the position accordingly. 

### **Bullet:**
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

### **Ui & Chatsystem:**
The UI for the game works very similar to the bullet, it's first set up and updated on the server, after that it's synchronized for every client by gathering the data from the server. The only difference is that there's data that needs to be valided for each seperate player, this it to ensure that the score is updated for the correct player. There also needs to be logic to remove the UI whenever a player disconnects.

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

The chat system works almost the same. The client first sends the message to the server. The server then validates the message. Then each client retrives the message from the server, and display it locally. 

Client [send message] --> Server [validate and format message] --> Every client [retrive and display message]
    
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
## **Challenges and faced solution:**

For me, I think most of the assignment went pretty smooth and I didn't have that many hurdles. However, there were two things that I had a bit of trouble with.

* Spawning player and the input not working.

One of the things I had trouble with was after the player died, and then respawned, the input system stopped working. The reason for this is because when the player dies, I destory the player object and then respawn a new instance of a player using the player prefab. This happend because the newly spawned network object might not necessarily have the same owner, depending how you handle the spawning. 

The solution for this was two-fold. First when spawning the player, I had the assign the ownership for the corresponding player.  

newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

And when checking for the input, I had to adjust it so it uses the IsOwner check instead of the IsLocal check. This is because when spawning a new player and networkobject you need to connect the clientId with the corresponding player object, and the easiest way to do this was to assign the ownership when spawning a new player object. 

* Realized you can't call server function as a client on the OnClientDisconnect function.

When disconnecting as a client I tried to clean up the player score UI by calling a ServerRpc function. However, it turns out that you're not able to do that. This is because the OnClientDissconnect gets called after the client has disconnected, which means that you're not able to communicate with the server anymore as the disconnecting client. 

The solution for this is pretty simple, instead of calling the function from the disconnecting client you can call it from the server in the same function. This is because the OnClientDissconect runs on both the disconnecting client and the server. 

Here's the solution:

    private void OnClientDisconnect(ulong clientId)
    {
        if (IsServer)
        {
            SetPlayerUIServerRpc(clientId, true, 0);
        }
        
        if (networkManager.LocalClientId == clientId)
        {
            menuCanvas.gameObject.SetActive(true);
            isInMainMenu = true;
            pauseCanvas.gameObject.SetActive(false);
            isInPauseMenu = false;
        }
    }

## **Reflection:**
Overall I'm pretty happy about how the assignment turned out. Although the game is very simple, I was able to implement all the features I wanted to. However, the part I'm the least happy about is the scoring system. This is since the way it's structured right now, it's not modular at all. If in the future, I would like to wanted add or reduce the number of players, it would require quite a lot of work. The reason it's not modular right now, is because I made the mistake of not thinking about the logic before I began, which in turned just led to me going deeper and deeper into the unmodular structure. I decided not to restrucure it because it works as I want it to still, but if I were to do the assignment again, I would definitely use another structure for the scoring system. For example, instead of using a player text varible for each player I would make it into an array, and I would probably also make another dictionary for each player and player score text.

Other than that, I would've prefered if the assignment was done in Unreal's network solution instead of Unity's. This is because I think it's way more likely for us to be working in multiplayer games in Unreal in the future. This would've also made us more used to work with c++ and pointers with replication to synchronize data across the network. Learning Unreal's replication system is something that I wish we would've learned in the networking course since this is very crutial to understand to work with multiplayer games in Unreal.
