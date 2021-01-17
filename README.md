# Unity 2D movement templates

Repo for my Unity 2D movement templates.

## Platformer

### Usage - Movement

1. Create two new `GameObject`s.
2. Name the other Ground and the other Player (as an example).
3. Create and change the `Layer` property of Ground to "Ground".
4. Attach the `PlatformerMovement` script to the Player.
5. Set the `Ground Layer` in `PlatformerMovement` to "Ground".
6. Adjust the `PlatformerMovement` and `Rigidbody2D` settings to your liking.

![Platformer example](https://media.giphy.com/media/zyryVzOY7CaI0vVDyS/giphy.gif)

## Grid

### Usage - Movement

See the example scene in the Grid directory to see how the scripts are set up in detail. Basically, you will need to create your Player
script and make it implement the Movable script. You can then call `this.SetTargetPosition` with the Vector3 representation of direction
you would like the player character to move into. Remember to set the Blocking Layer in Editor to make sure the Movable can collide.

![Grid movement example](https://media.giphy.com/media/YgN5JCouCB19ns9NQd/giphy.gif)

### Usage - Pathfinding

The example scene here is the best way to get acquainted with how things work. However, setting this up should be pretty much as simple as attaching the `Pathfinder` script and then setting the `target` tranform field to some value.

![Grid pathfinding example](https://media.giphy.com/media/gCY9qXPwYJN4V9vz8Z/giphy.gif)