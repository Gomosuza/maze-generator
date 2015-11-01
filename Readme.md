A 3D maze generator implemented using Monogame (using Visual Studio 2015/C# 6.0).

## Compile ##

* Restore nuget packages for solution
* then either run "RELEASE VERSION.cmd" to get all the necessary files in "!Releases" output dir
* or compile solution and run in Visual Studio 

**Note that while monogame was used it is embedded directly into the exe using Fody.Costura**

## Sample maze ##

![Maze](/maze.png?raw=true)

## Known issues ##

* DirectX implementation of Monogame does not support AA which makes the maze look really bad at distance (especially with textured walls).
* Sometimes the mouse left/right movement seems sluggish, this is a [known issue with Monogame on Windows](https://github.com/mono/MonoGame/issues/2464)
	* The cause is the mouse actually moving outside the window (even though it should be reset each frame). To fix it alt tabbing out of the game and back in will help
* The player may get stuck in some of the walls due to a poor collision implementation
* When running into a wall while sprinting the player can actually see through the wall