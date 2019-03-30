AGENTS

every Agent, the guys who move around need an Agent script, and a PathMover script. You probably also want to give them some kind of sprite so you can see them. 

I give my guys a circle collider and a ridgidbody so they bump into each other, as well as putting them on a player layer. but that is not really nessosary for the scripts to work. 

The PathMover script on each agent needs to be set up with a movespeed, and a fudge distance.

NODE 
You need a node prefab, which should have a Node script. nothing should be set up here, you can leave everything as default. The node prefab should also have a circle collider, also setup with default values.

PATHFINDER
The next part of the scene needs to be an object called 'Pathfinder'. It needs a Pathfinder script, and a Grid script. You need to set the NodeRad to how big you want your nodes to be, and a grid size x and y, which will determine how many nodes are placed in both directions - depending on the NodeRad.

OBSTRUCTIONS

Any obstruction in the scene needs an Obstruction script. at the moment, this can only use boxcollider2D, so you need one of those as well. 


