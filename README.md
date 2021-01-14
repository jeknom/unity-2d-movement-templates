# Unity 2D movement templates

Repo for my Unity 2D movement templates.

## Usage - Platformer Movement

1. Create two new `GameObject`s.
2. Name the other Ground and the other Player (as an example).
3. Create and change the `Layer` property of Ground to "Ground".
4. Attach the `PlatformerMovement` script to the Player.
5. Set the `Ground Layer` in `PlatformerMovement` to "Ground".
6. Adjust the `PlatformerMovement` and `Rigidbody2D` settings to your liking.

![Platformer example](https://media.giphy.com/media/zyryVzOY7CaI0vVDyS/giphy.gif)

## Usage - Grid Movement

See the example scene in the Grid directory to see how the scripts are set up in detail. Basically, you will need to create your Player
script and make it implement the Movable script. You can then call `this.SetTargetPosition` with the Vector3 representation of direction
you would like the player character to move into. Remember to set the Blocking Layer in Editor to make sure the Movable can collide.

![Grid example](https://media.giphy.com/media/YgN5JCouCB19ns9NQd/giphy.gif)