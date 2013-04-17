Spine Test for XNA and MonoGame
-------------------------------

This is a quick test solution that combines the Spine XNA runtime (https://github.com/EsotericSoftware/spine-runtimes) with a tile engine to create a simple platformer.

It's a VS2012 solution; I used these instructions to get XNA GS4 installed in 2012: http://stackoverflow.com/questions/10881005/how-to-install-xna-game-studio-on-visual-studio-2012

The solution contains a standard XNA project as well as a Monogame Windows (DirectX) port.

I have included the Monogame DLLs that I used, you should be able to resolve missing references with them.

The Spine animations used are the two default SpineBoy anims (Walk, Jump) as well as a quick Crawl anim that I knocked together (I am not an animator!). The jumping anim doesn't look too good, especially when it gets interrupted. Hoping someone might have a stab at fixing that!

I'm also not happy with the collision detection. I've never been too hot with tile-based collisions and there's some rough edges in there. It would be *awesome* if someone could show me how it's done :D

Tweet me @garethiw.

