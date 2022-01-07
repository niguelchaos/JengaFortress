# Jenga Fortress
This is a guide on how to set up and play Jenga Fortress.

Demo Scene: **IntegrateUI**

## Getting started
1. To start playing, tap the <strong>Play</strong> button 
2. Set the stage by tapping on a detected surface, then tap the <strong>Place</strong>. When you see the grid plane, tap the <strong>Confirm</strong> button
3. You will now set the firing position relative to your camera. Use the <strong>slider</strong> to move the gray sphere, which represents the position where your projectiles will be fired from. We recommend you to have the sphere close to your camera, so that the projectiles look like they come straight out of the camera. 
4. Each player will now place their fortresses on the playing field. Start with <strong>Player 1</strong>, simply tap somewhere on the playing field (grid) where you want your fortress to be placed. When you see your fortress on the playing field, tap the <strong>Confirm</strong> button. <strong>Player 2</strong> will now do the same procedure. 
5. Each player will now place their <strong>Core block</strong>, hide this block somewhere in your fortress. Tap on the place you want your core block to be placed. When Player 1 has placed their core block and you can see it, tap the <strong>Confirm</strong> button and pass the phone to Player 2. Player 2 will do the same procedure.

## Playing the game
After getting through the set up process, you find yourself in the game loop! Here's how it goes:
 ### What you can do on your turn
 * <strong>Fire</strong>: You can fire a block by tapping and holding the screen. The longer your hold your finger on the screen, the more force you are applying to your projectile. Release your finger when you are ready to fire and watch a projectile get fired in the direction you are pointing your camera at. You only get <strong>1</strong> block to fire per turn!
 * <strong>Move</strong>: Click on the Move button on the right hand side. In this mode, you can move individual blocks from your own fortress. Use this when you want to reorganize your fortress.
 * <strong>End turn</strong>: After you have fired your block, you need to pass the turn to the other player. Do so by tapping the <strong>End turn</strong> button at the top of the screen. 

### Win condition
You win in Jenga Fortress by making the enemy players core block move away from it's placed position. The core blocks have a spherical boundary where they are initially placed. When a player's core block leaves this boundary (by getting hit from other player or moved) the player loses. 

You can change the winning condition to "hit floor" in the Unity Editor by doing the following:

1. Find GameManager gameobject: Managers > GameManager

2. Choose win condition in Inspector

## Required packages:

- AR Foundation

- ARCore XR Plugin

- XR Plugin Management

- Universal RP