===== Build & Demo =====

See here: https://drive.google.com/drive/folders/1fP1oFSv07bMlxB3OwR7rVNpBPCLfCYT1?usp=sharing

===== Concept =====

I wanted to try and put an artistic twist on the cellular automata theme. This project
started as a concept to create a world where different colours had advantages over other
ones (almost like the Fire Emblem weapon triangle). This actually worked, but the
results were so ridiculously chaotic that there wasn't anything "beautiful" about it.

This submitted version is loosely based on the 4-5 rule concept used to generate natural
caves. There are some adjusted rules to create a bit more stability and some "wooshing"
visuals when certain colour combos quickly sweep across a painting before disappearing.
See below on behaviours for that.

The tick rate in-game is slow purely for aesthetics - feel free to try speeding it up
in-editor if you'd like (see below about scriptable objects).

===== Platform =====

This project was created in Unity 2018.2.6f1
To Build: Nothing special, just use the Unity build dialogue box. Everything is set up.

===== Controls =====

WASD		Move Camera
Mouse Wheel	Zoom
F		Change Colour
Left Click	Place / Change Tile
Space		Pause / Resume
Backspace	Clear All (No Confirmation)
Escape		Quit (No Confirmation)

===== Behaviours =====

"Neighbours" refers to all directly adjacent and diagonal tiles.

Space
	- Doesn't directly do anything.
	- The world is mapped so that you can paint tiles anywhere.

Primary Colours (Red, Green, Blue):
	- Will become a space tile if there are 5 of more neighbouring space tiles.
	- Will mix into a secondary colour upon contact with each other.

Secondary Colours (Cyan, Magenta, Yellow):
	- Will expand rapidly into primary colours they contact.
	- Will become a space tile if two secondaries contact.
	- Will become a space tile if there are 2 or more neighbouring space tiles.

===== Source =====

All non-Unity code in this project was created from scratch.

Here's the core layout of the system:

	- Scriptable objects are used as a data system to keep components decoupled from
	one another while being very Unity Editor friendly. You can find those in the
	ScriptableObjects folder. This system functions very similarly to dependency
	injection both functionally and in terms of pros/cons, but the editor is
	responsible for hooking up and showing links instead of an injector class.

	- Neighbouring tiles, how memory is managed, colours, settings etc. are all
	defined in scriptable objects. You can find these in the GameData folder. Memory
	allocation/deallocation was not a concern since this is just a small-scale fun
	thing to play around with. Memory could be managed into buffers to improve
	performance a fair bit.

	- TileKernels.cs is where the actual core "game logic" happens. These can be
	swapped out on the WorldSimulator game object within the scene if you're in the
	editor. There's no in-build UI for that. I'd probably swap this to using
	NativeArrays and ParallelJobs for scalability if I were to develop further.

	- Rendering is done within WorldRenderer via Graphics.DrawMeshInstancedIndirect
	and tile visuals are all handled in the singular shader in the Shaders folder.

	- Input handling code is a bit rough around the edges. I would much rather pull
	out each of those functions within the InputEventGenerator and create a list
	of generic input types to process.

	- The "core classes" are WorldSimulator, World, WorldChunk, WorldRenderer,
	TileKernels.

===== Licenses =====

Music is by Kevin MacLeod at incompetech.com

Background image is a random Google Image that I found many of the same copy of, I'm
unsure of the original source.