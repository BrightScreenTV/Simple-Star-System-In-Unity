# SimpleStarSystemUnity


_Deprecated:_ Please see the newest method of generating this effect [here](https://github.com/BrightScreenTV/Star-System-Sphere-Faker)

This is my way of generating an infinte star system for the backdrop to a game. I didn't want to have 100's of even small particles all needing to be controlled as the player moved to produce a starry backdrop for a space game, so I used the idea of a relatively small number of star 'plates', each quite large, arranged in a grid pattern and projected behind the player. Each plate is just a load of random, white dots on a blank texture.

The main camera is a child of the player's sprite (which in this instance can rotate left and right with the arrow keys, accelerate with 'Q', decelerate with 'A', and come to a complete stop with 'S') so that the player is always locked to the centre of the screen. There is also a plane just behind the player which is the 'star system' and as this is a child of player then it effectively acts as a backdrop. The 'star system' is captured by a camera (the Star System Camera) which is rendered to this plane.

The plates of stars are arranged in a grid which is initially centred around the Star System camera. As the camera moves and heads towards the edge of this grid, the plates which are behind the movement are transformed and placed ahead of it, so provided an infinte cascade of stars.

The Star System camera mimics the movement of the player as it turns and accelerates, but with one difference: it's position never wraps, i.e. you can set the player to remain within the confines of a boundary (enter values in the players script on the editor), but this camera will continue to move in whichever direction the player is going despite the player hitting the edge and being repositioned at the opposite edge. This enables the player to be controlled within a certain area of the world but the stars continue seamlessly even if the player's position changes abruptly when it reaches the extent of the playable area.

You can set the look of the star system with the grid size and number of layers (to give it a feeling of depth the first layer is positioned closest to the camera, and subsequent layers further away). I'd recommend a minimum of 3 for the grid size and 3 for the layers to give it a nice, deep space feel.
