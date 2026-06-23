Synchronization of online components is something I need to add to the guide because it's not entirely self-explanatory. We'll do our best to try to explain how it works. I think your issue with the detonator though is simple; when you call Level.Add on the server, it adds the object to the server and tells the clients that it was added. When you call Level.Add on the client, it does the same; you'd be adding it twice, in that case. What you need to do is check "Network.isActive" and if it's true, only add the item to the Level if Network.isServer.  

The explosion thing is also related to networking. It's a bit difficult to explain here how that works. If you want, you can use a program like ILSpy, and examine the source to the Grenade class, or the Mine.  

The last issue is something that I implemented last minute. Basically, here's how selecting maps works:  

- if the map contains any modded items
  - is the mod item local-only (that is, not workshop)?
  - is this online mode? if so, remove map from list
  - is the mod item Workshop?
  - does the host have this Workshop item subscribed? if not, remove map from list

If your mod is not pushed to the Workshop, you can't use it online at all. Workshop mods also take priority over your local mods, so once you push to workshop and restart the game it will default to the Workshop version; you can go back to your local version by unsubbing from the Workshop one. Workshop mods are, by default, only visible to yourself, so don't worry about your mod getting released early :)

Last edited by [Paril](https://steamcommunity.com/id/paril); 27 Jul, 2015 @ 1:27pm

[#1](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409715240)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/855630/bf835f556a3ba4f778ffd2550e592de4a0cefe27.png)

[![](https://avatars.fastly.steamstatic.com/7a5b1f531fd14074a75b61462c376718725e5f85.jpg)](https://steamcommunity.com/id/timfalken)

[Timfa](https://steamcommunity.com/id/timfalken)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 1:28pm 

> Originally posted by **[Paril](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409715240)**:
> 
> snip

That first one sounds good, and is also pretty handy to know. I'll try that right now, it should hopefully resolve this.  

I actually copied most of the explosion stuff from the grenade, so I should have most of that already. Might have missed some stuff related to the networking though, as at the time I was specifically looking for stuff that made ducks go "boom".  

I'll check the code again and see what I missed :)  

(Also; Thanks for your quick replies to both my threads :D +10 internet points!)

[#2](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409723938)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/978460/958c368b43281a7c2520d3bd4671e7760ac3e1b6.png)

[![](https://avatars.fastly.steamstatic.com/e50ceb5547cd8bd3304a99f3aaec54c60d7e34b3.jpg)](https://steamcommunity.com/id/paril)

[Paril](https://steamcommunity.com/id/paril)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 1:32pm 

I'll have to ask Landon what the situation is about explosions. Network sync is a really touchy subject with the engine, unfortunately. I still don't entirely understand how it all fits together either, lol.  

EDIT: poopy, I just thought about something. In your case, if the item is created when you grab the object for instance, you need to check for "this.isServerForObject", not the generic Network stuff. When you "Fondle" an object, you become the server for that object - you probably just need to make sure that you are the server for the object before spawning your second item.

Last edited by [Paril](https://steamcommunity.com/id/paril); 27 Jul, 2015 @ 1:33pm

[#3](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409733232)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/855630/bf835f556a3ba4f778ffd2550e592de4a0cefe27.png)

[![](https://avatars.fastly.steamstatic.com/7a5b1f531fd14074a75b61462c376718725e5f85.jpg)](https://steamcommunity.com/id/timfalken)

[Timfa](https://steamcommunity.com/id/timfalken)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 2:11pm 

Checking the server-stuff worked for the detonator-clones! Only one for now. (The pink-isn't-transparent thing popped up again though, but that's unrelated for now I suppose.)  

Explosions don't really appear to work though, even when directly copying the entire MakeExplosion() from the grenade file. (which also doesn't appear to produce shrapnel?)  

Also, I set the ammo to int.maxvalue, but when dropping it, clients see it dissapear in a puff of smoke while it persists on my end.  

EDIT: We have managed to test other people "stealing" detonators and blowing up the suit remotely by catching it in mid-air, so that works. The only thing we've seen is that explosions are only visible for the "server" of the suit.  

Explode() in vest: [public void Explode() { if (time &gt; 30 &amp;&amp; !exploded) - Pastebin.com](http://pastebin.com/Ry6GvU7r)  
Fire() in detonator: [public override void Fire() { //base.Fire(); //causes crash - Pastebin.com](http://pastebin.com/6Q1815wb)  

Bit in detonator that should prevent it dissapearing due to no ammo: [public Detonator(Vest vest, float xp, float yp) : base(xp, yp) { - Pastebin.com](http://pastebin.com/NSTBM6zz) (is there a way to just remove this functionality for my object, instead of working around it?)

Last edited by [Timfa](https://steamcommunity.com/id/timfalken); 27 Jul, 2015 @ 2:26pm

[#4](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409843182)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/978460/958c368b43281a7c2520d3bd4671e7760ac3e1b6.png)

[![](https://avatars.fastly.steamstatic.com/e50ceb5547cd8bd3304a99f3aaec54c60d7e34b3.jpg)](https://steamcommunity.com/id/paril)

[Paril](https://steamcommunity.com/id/paril)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 2:25pm 

I'll ask Landon about the explosions in a bit. If you want, you can go through the list of NM classes ("net message") and see if maybe they will help. The Send class will send stuff to clients.  

As for ammo, it's because ammo is encoded using a signed byte. The max ammo value you'd be able to do over the network at the moment is 127. I will look into how to potentially fix that.

[#5](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409882594)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/855630/bf835f556a3ba4f778ffd2550e592de4a0cefe27.png)

[![](https://avatars.fastly.steamstatic.com/7a5b1f531fd14074a75b61462c376718725e5f85.jpg)](https://steamcommunity.com/id/timfalken)

[Timfa](https://steamcommunity.com/id/timfalken)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 2:27pm 

> Originally posted by **[Paril](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409882594)**:
> 
> I'll ask Landon about the explosions in a bit. If you want, you can go through the list of NM classes ("net message") and see if maybe they will help. The Send class will send stuff to clients.  
> 
> As for ammo, it's because ammo is encoded using a signed byte. The max ammo value you'd be able to do over the network at the moment is 127. I will look into how to potentially fix that.

Oh, that explains that. I'll just lock it at 100 now then, one issue less for me :)

[#6](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409887708)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/855630/bf835f556a3ba4f778ffd2550e592de4a0cefe27.png)

[![](https://avatars.fastly.steamstatic.com/7a5b1f531fd14074a75b61462c376718725e5f85.jpg)](https://steamcommunity.com/id/timfalken)

[Timfa](https://steamcommunity.com/id/timfalken)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 2:33pm 

About the pink bit btw, is it possible that the bool value in  
this.graphic = (Sprite)new SpriteMap(GetPath("detonator"), 16, 16, false); <at the end here  

should be on true to prevent the pink boxes? (though it's weird that it worked for a moment and then stopped, but I was doing all sorts of stuff at the time so I might have bumped something to mess this up)

Last edited by [Timfa](https://steamcommunity.com/id/timfalken); 27 Jul, 2015 @ 2:33pm

[#7](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409905613)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/978460/958c368b43281a7c2520d3bd4671e7760ac3e1b6.png)

[![](https://avatars.fastly.steamstatic.com/e50ceb5547cd8bd3304a99f3aaec54c60d7e34b3.jpg)](https://steamcommunity.com/id/paril)

[Paril](https://steamcommunity.com/id/paril)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 2:43pm 

Shouldn't be, it's a preload thing.

[#8](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409932412)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/855630/bf835f556a3ba4f778ffd2550e592de4a0cefe27.png)

[![](https://avatars.fastly.steamstatic.com/7a5b1f531fd14074a75b61462c376718725e5f85.jpg)](https://steamcommunity.com/id/timfalken)

[Timfa](https://steamcommunity.com/id/timfalken)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 2:55pm 

> Originally posted by **[Paril](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409932412)**:
> 
> Shouldn't be, it's a preload thing.

Spooky! I'll try to figure out what causes this.  

It's been working for the belt item before. I had the belt on (255,0,255) transparency which didn't work for a while but then mysteriously did, and the detonator on the actual transparency that wasn't working right because it wasn't exactly (0,0,0,0), so I made that have (255,0,255) as well, but then both of them got pink boxes around them in-game.

[#9](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409966866)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/978460/958c368b43281a7c2520d3bd4671e7760ac3e1b6.png)

[![](https://avatars.fastly.steamstatic.com/e50ceb5547cd8bd3304a99f3aaec54c60d7e34b3.jpg)](https://steamcommunity.com/id/paril)

[Paril](https://steamcommunity.com/id/paril)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 2:58pm 

We'll fix premultiplied trans for next release. Standing fluid is also being fixed as we speak.

[#10](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409976074)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/855630/bf835f556a3ba4f778ffd2550e592de4a0cefe27.png)

[![](https://avatars.fastly.steamstatic.com/7a5b1f531fd14074a75b61462c376718725e5f85.jpg)](https://steamcommunity.com/id/timfalken)

[Timfa](https://steamcommunity.com/id/timfalken)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 3:05pm 

> Originally posted by **[Paril](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409976074)**:
> 
> We'll fix premultiplied trans for next release. Standing fluid is also being fixed as we speak.

Sounds cool :D I'll patiently await that. Not entirely sure what "Standing fluid" means but it sounds clever!  

Also, I'd again like to thank you for your quick responses. I hope I haven't been taking up too much of your time :)

[#11](https://steamcommunity.com/app/312530/discussions/3/541906989409705452/#c541906989409994543)

![](https://shared.fastly.steamstatic.com/community_assets/images/items/978460/958c368b43281a7c2520d3bd4671e7760ac3e1b6.png)

[![](https://avatars.fastly.steamstatic.com/e50ceb5547cd8bd3304a99f3aaec54c60d7e34b3.jpg)](https://steamcommunity.com/id/paril)

[Paril](https://steamcommunity.com/id/paril)![](https://community.fastly.steamstatic.com/public/images/skin_1/icon_InLibrary.png "has Duck Game")

27 Jul, 2015 @ 3:36pm 

Standing fluid was meant for another thread, haha. Nope!
