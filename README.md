# Just Survive Somehow

#### GAME DESIGN SPEC

JSS is a turn-based, survival, dungeon-crawler game. On each level, you start at the bottom-left corner of the board and have to find your way to the exit at the top-left corner of the board after killing all the enemies on the board. Along the way, you will find food or soda, that increases your health. You will also encounter zombie enemies that will attack you, and you can attack back too. Finally, you can also break walls down.

There are two types of enemies. Easy enemies are not quite aware of their surroundings, and hence only attack the player when you come within two tiles of them. Hard enemies will attempt to find you from anywhere on the board.


This project has two separate AIs for the Easy enemies and the hard enemies.

Once you have Unity installed, you should be able to play the game and move around and complete levels etc with the AIs.

**To summarize:**

 1. AI for the Hard Enemy to always move towards the player each turn, and attack him if possible. The hard enemy will follow the shortest path. If not possible, will search for next one. If next one is also blocked then it will check for first path again just in case if the hurdles were broken by the player.
 2. AI for the Easy Enemy to only start following the player if the player comes within two tiles of the Easy Enemy and then attack him whenever possible.
 3. Feature to make game more competitive - Game will go forward only if all the enemies, which are present on the current day, are killed.
 4. Removed bug -  
 	* Collisions would take place rarely on a same block between player and enemy, due to coroutines stacking up.
	* The enemy would stay at the same closest tile and not move. 

#### Unity Installation

I've used Unity version 5.4.0f3, so you will need either that version, or a newer version to run this game.

You can find all the versions of Unity here - [https://unity3d.com/get-unity/download/archive](https://unity3d.com/get-unity/download/archive).

