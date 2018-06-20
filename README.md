# NOVAKIN.Mod.Elite

In NOVAKIN, a "mod" is what defines gameplay. A mod contains the logic nessecary to manage the gametype (such as CTF, Rabbit, Arena, etc), as such it is responsible for things such as spawning the players, keeping scores, defining teams, etc. Also included in the mod as definitions for things such as weapons, armors, deployables and other game items.


The goal of NOVAKIN is to include as much as we can about the game into the mod itself, slowly over time, internal engine functions will be migrated into the mod so that modders will have ultimate flexibility. Unfortunatly some things will never be able to be completed in a mod because clientside prediction will break.


Mods are written in C#, and have direct access to internal engine functions. Because NOVAKIN uses Unity, most Unity functions will work. You can of course use NOVAKIN specific functions and classes as well. Because the engine does not know about mods at compiletime, the engine has no clue what classes and such the mod contains. Because of this, there are a number of callbacks that have been created to listen for important ingame events such as map loading, round ending, damage, etc. More callbacks will be added in the future as needed.


If any specifical callbacks are needed to help with your mod, let me (defiance) know and I can add them quickly for you.


I will create a few videos on how to setup your dedicated server and your VS environment in order to mod for NK shortly, but anyone familiar with VS already should figure it out on thier own very easily.

