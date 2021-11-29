#Jenga fortress

In this game, players take turns to shoot projectiles at eachothers fortresses. The fortresses are made out of jenga blocks (mostly). We intended from
the start to have all of the fortress to be made out of jenga blocks, but that proved to be computationaly intensive. Therefore we comprimised by creating large blocks for some fortress elements instead of several smaller ones. This boosts the performance by a large margin since it's not simulating physics for tons of smaller blocks. 

To switch it up from regular jenga blocks that are usually made out of wood, we created new blocks made out of ice and rubber. We created new physics material for these blocks and adjusted the parameters according to https://medium.com/sun-dog-studios/rapid-unity-tutorials-1-physics-materials-68758351fd8a in order to try matching the real world ice and rubber behavior. The rubber block has more bounciness while the ice block has much less static and dynamic friction compared to the wooden block. 

To further push the boundaries of the physical behavior of the jenga blocks, we attached invisible springs to them. For example in the demo, one of the walls has springs attached to it which will keep the wall better intact when firing at it. To fire projectiles at a wall press SPACE. We also wanted to have the walls act like some sort of a trampoline, which bounces the projectiles back at the player but we weren't able to implement such a functionality for the time being. The 7 walls between the fortresses are supposed to demonstrate the different behaviors of the materials and springs. When firing projectiles, the game will fire a projectile towards each wall so you can compare the behavior side by side. 

In terms of gameplay, each player has a "core block". When this blocks falls outside of a spherical boundary (which is visible in the editor), the player loses. 

Before starting the game, you should find a GameManager gameobject. In the GameManager, choose the win condition, for example "hit floor". In this gamemode, the game is over when one of the core blocks hits the floor. To demonstrate this, you would need to manually raise the core block of your fortress and observe the game ending when it hits the floor. 

There is a cube behind the enemy fortress which changes colors based on the game state. These are the following game states

White: Inside the editor
Yellow: Player one's turn 
Blue: Player two's turn
Red: Game over

##