Creating a mod for Duck Game will require experience with basic math and a lot of fiddling. Documentation is fairly limited at the moment, but hopefully as things move forward we will have more samples and documentation to provide.  

The base class for everything in Duck Game is "Thing" - if you wish to add a new object to the game, that is the class you must inherit.  

Adding an EditorGroup attribute to your custom class will allow it to appear in the Editor. The format for this attribute is a token followed by |, for instance:  

[EditorGroup("guns|explosives")]

will place your Thing in guns -> explosives.  

The static function Thing::GetPath should be used when you wish to query for content in your mod. Thing::GetPath(string), the non-static function, will give you a path directly to your mod (more specifically, to the path of the mod that owns the Thing - which in this case is yours). Thing::GetPath<TMod>(string), the static version, will allow you to call it from anywhere. Pass it your Mod type, or the Mod type you wish you get a path to, and it will give you it.  

The **Content** class is used to load and fetch loaded sounds, textures and songs.  
The **Sprite** and **SpriteMap** classes are used for creating and maintaining a sprite.  
The **Graphics** class provides direct access to the drawing procedures, if you need more advanced drawing access.  

Every Type instance can be given a **property bag** - a list of keys and values that can be modified during initialization. Inside your Mod, you can check the protected "_properties" member, and you may modify your Mod's property bag during initialization **but not at run-time**. Things can be given simple properties via the BaggedProperty attribute, for instance:  

[BaggedProperty("isGrenade", false)]

You can fetch a property bag for a Type via the ContentProperties.GetBag functions. These are read-only. You can use these as a way to communicate special constant values between mods.

Important types

The following types will be loaded from your mod and registered for use in the game:  

- **Thing** - the base class for anything that can appear in the game.  

- **AmmoType** - a special class used for specifying ammo fired from guns. This allows you to specify special data to be passed along with your ammo, and is an important piece of how bullets are transferred over the network.  

- **DeathCrateSetting** - a class for specifying a potential outcome for a Death Crate. Not currently synced in multiplayer at the time of writing this guide.  

- **DestroyType** - a class for specifying the type of destruction against a prop. These are mostly used as tags and will likely change in the future.

The following are built-in bagged properties for Things that you may modify on your Things:  

- **canSpawn** - boolean, default true. Whether or not this Thing can be spawned from a random box. Note that this only applies to Things that inherit from **PhysicsObject**.  

- **isOnlineCapable** - boolean, default true. Whether or not this Thing will be spawnable in online play.  

- **isFatal** - boolean, default true. Whether or not this Thing can **implicitly** kill other ducks (for instance, the Net Gun does not **implicitly** kill other ducks, whereas a pistol does). This is used as a hint for the random level generator.  

- **isSuperWeapon** - boolean, default false. Whether or not this Thing is classed as a 'super weapon' and will have lower chances of spawning in randomly generated maps.

Uploading and updating

When you're ready to upload, you can perform a Workshop upload in-game via the Manage Mods menu in Settings. The initial upload is quick and automated. If you wish to have a small preview image in the Manage Mods menu, add a texture named "preview.png" inside your content/ folder, and it will be used by the Menu to provide a preview.  

To update a mod, select the UPDATE option in Manage Mods. You'll be prompted to add a few lines for update notes; when ready, click Update and it will submit the content through Steam Workshop.
