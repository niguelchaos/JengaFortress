# Jenga Fortress

## Controls: ##
* SPACE to fire
* To change winning condition to "hit floor":
    1. Find GameManager gameobject: Managers > GameManager
    2. Choose win condition in Inspector
    3. Manually drag core block to ground in Scene view 
    4. Observe the game ending when it hits the floor
## Introduction ##
In this game, players take turns to shoot projectiles at each others' fortresses. The fortresses are made out of jenga blocks (mostly). We intended from
the start to have the entire fortress made out of jenga blocks, but that proved to be computationally prohibitive. Therefore we compromised by creating larger blocks for some fortress elements instead of several smaller ones, reducing the amount of colliders. Other optimizations included reducing fortress complexity and modifying Unity physics' settings. This boosts the performance by a large margin since it's not simulating physics for tons of smaller blocks. 

### Block Types ###
To switch it up from regular jenga blocks that are usually made out of wood, we created new blocks made out of ice and rubber. We created new physics material for these blocks and adjusted the parameters according to https://medium.com/sun-dog-studios/rapid-unity-tutorials-1-physics-materials-68758351fd8a in order to try matching the real world ice and rubber behavior. The rubber block has more bounciness while the ice block has much less static and dynamic friction compared to the wooden block. 

### Springs-Connected Blocks ###

To further push the boundaries of the physical behavior between the blocks, we attached spring joints to them in different ways. For example in the demo, one of the walls has springs attached its initial world space position which makes the wall surprisingly stronger. We also wanted to have the walls act like some sort of a trampoline, which bounces the projectiles back at the player but we weren't able to implement such a functionality for the time being. The 7 walls between the fortresses are supposed to demonstrate the different behaviors of the materials and springs. When firing projectiles, the game will fire a projectile towards each wall so you can compare the behavior side by side. 

### Asset Context ###
In terms of gameplay, each player has a "core block" which is hidden from the other player. 
There are currently three methods of losing: The core block hitting the floor, the block falling outside of a spherical boundary, or both combined. 

There is a cube behind the enemy fortress which changes colors based on the game state. These are the following game states:

* White: Main Menu, Starting
* Yellow: Player one's turn 
* Blue: Player two's turn
* Red: Game over

##