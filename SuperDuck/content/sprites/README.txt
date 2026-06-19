Drop SuperDuck sprite art here (PNG).

GandalfsStaff now uses custom art: gandalfsstaff.png (11x48), wired up in
GandalfsStaff.cs via:
       graphic = new Sprite(Mod.GetPath<SuperDuckMod>("sprites/gandalfsstaff"));
(note: the .png extension is omitted so it resolves to the preloaded copy.)

To replace it, overwrite gandalfsstaff.png and re-tune center /
collisionOffset / collisionSize in GandalfsStaff.cs to match the new art.
(PNG only; use a real alpha channel, OR a (255,0,255) magenta key with
 <PinkTransparency>true</PinkTransparency> in mod.conf.)

For an animated staff, use a sprite sheet + SpriteMap instead:
       var sm = new SpriteMap(Mod.GetPath<SuperDuckMod>("sprites/gandalfsstaff"), 16, 16);
       sm.AddAnimation("idle", 0.2f, true, 0, 1, 2, 1);
       graphic = sm;

Mod assets are referenced by PATH via GetPath, not by bare atlas name.
Full reference (formats, animation, sound): docs/modding-guide.md §3.2.
