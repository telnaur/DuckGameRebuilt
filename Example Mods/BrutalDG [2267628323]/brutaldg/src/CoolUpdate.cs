using System;
using DuckGame;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace DuckGame.BrutalDG
{
	internal class CoolUpdate : Thing
	{
		public CoolUpdate(float xpos, float ypos) : base(xpos, ypos)
		{
			base.layer = Layer.HUD;
			base.depth = 3f;
		}
		
		public override void Update()
		{
			int num4 = 150;
			if (Graphics.fps > 60)
            {

            }
			else if (Graphics.fps <= 54 && Graphics.fps >= 50)
            {
				num4 = 120;
            }
			else if (Graphics.fps < 50 && Graphics.fps >= 40)
			{
				num4 = 100;
			}
			else
            {
				num4 = 40;
			}
			if (Level.current.things[typeof(KetchupPixelThing)].Count() > num4)
            {
				(Level.current.things[typeof(KetchupPixelThing)].First() as KetchupPixelThing).Wash(false, true);
			}
			if (Level.current.things[typeof(KetchupPixelBlock)].Count() > num4 + 60)
			{
				(Level.current.things[typeof(KetchupPixelBlock)].First() as KetchupPixelBlock).Wash(true);
			}
			if (!BrutalDG._addedTex)
			{
				BrutalDG.LoadTextures();
				BrutalDG._addedTex = true;
			}
			RagdollParts._ragdollLayer.visible = false;
			BrutalDG.bloodblocks = BrutalOptionsData.bloodonthings > 1;
			BrutalDG.bloodguns = BrutalOptionsData.bloodonthings == 1 || BrutalOptionsData.bloodonthings == 3;
			if (BrutalDG.bloodblocks && !(Level.current is Editor))
			{
				foreach (Block block in Level.current.things[typeof(AutoBlock)])
				{
					if (BloodBlocks._blocks.Count > 1300)// || Level.current.level.ToLower() == "random")
					{
						break;
					}
					if (!BloodBlocks._blocks.Contains(block))
					{
						Level.Add(new BloodBlocks(block));
						BloodBlocks._blocks.Add(block);
					}
				}
				foreach (Block block in Level.current.things[typeof(ItemBox)])
				{
					if (!BloodBlocks._blocks.Contains(block))
					{
						Level.Add(new BloodBlocks(block));
						BloodBlocks._blocks.Add(block);
					}
				}
				foreach (BackgroundTile block in Level.current.things[typeof(BackgroundTile)])
				{
					if (BloodBlocks._blocks.Count > 1300)// || Level.current.level.ToLower() == "random")
					{
						break;
					}
					if (!BloodBlocks._blocks.Contains(block))
					{
						Level.Add(new BloodBlocks(block));
						BloodBlocks._blocks.Add(block);
					}
				}
			}
			else
			{
				foreach (BloodBlocks block in Level.current.things[typeof(BloodBlocks)])
				{
					Level.Remove(block);
				}
				BloodBlocks._blocks.Clear();
			}
			if (Level.current.things[typeof(BloodBlocks)].Count() == 0)
			{
				BloodBlocks._blocks.Clear();
			}
			if (!ModLoader.modsEnabled || !(Level.current is TeamSelect2) || Steam.lobby == null || Steam.lobby.id == 0UL)
            {
                this.updateLobby = true;
            }
            else
            {
                string text;
                text = Steam.lobby.GetLobbyData("mods");
                if (Level.current is TeamSelect2 && this.updateLobby && !string.IsNullOrEmpty(text))
                {
                    int num = text.IndexOf(BrutalDG.replaceData);
                    if (num < 0)
                    {
                        this.updateLobby = false;
                    }
                    else
                    {
                        text = text.Remove(num, BrutalDG.replaceData.Length).Trim(new char[]
                        {
                            '|'
                        }).Replace("||", "|");
                        if (Steam.lobby.type == SteamLobbyType.Invisible)
                        {
                            Steam.lobby.SetLobbyData("dev", "true");
                        }
                        Steam.lobby.SetLobbyData("mods", text);
                        this.updateLobby = false;
                    }
                }
            }
			this.savewait++;
			if (this.savewait > 99)
			{
				BrutalDG.SaveSettings();
				this.savewait = 0;
			}
			foreach (Bullet bullet in Level.current.things[typeof(Bullet)])
			{
				if (bullet.travelTime <= 0.02f && BrutalOptionsData.screenshaketype < 2)
				{
					float num = 1f;
					if (bullet.ammo != null && !(bullet.ammo is ATGrenade) && !(bullet.ammo is ATShrapnel))
					{
						num = Math.Abs(bullet.ammo.penetration) * 1.05f;
					}
					if (BrutalOptionsData.screenshaketype == 1)
                    {
						num *= 20 / Vec2.Distance(DuckNetwork.localProfile.duck.position, bullet.position);
					}
					BrutalScreenShake.ScreenShake(Rando.Float(1f, 2f) * num, Rando.Float(0.7f, 2f));
				}
			}
			if (Level.current.things[typeof(BrutalScreenShake)].Count() == 0)
			{
				Level.Add(new BrutalScreenShake(this));
			}
			var events = typeof(RumbleManager).GetField("ListRumbleEvents", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
			if (events != null && events.FieldType == typeof(List<RumbleEvent>) && Network.isActive && BrutalOptionsData.screenshaketype > 0)
            {
				List<RumbleEvent> rumbleEvents = events.GetValue(null) as List<RumbleEvent>;
				if (rumbleEvents.Count > 0)
                {
					foreach (RumbleEvent event1 in rumbleEvents)
                    {
						try
						{
							float num = 1f;
							if (event1.profile == DuckNetwork.localProfile)
							{
								num = Math.Abs(event1.intensityInitial) * 1.05f;
								BrutalScreenShake.ScreenShake(Rando.Float(1f, 2f) * num, Rando.Float(0.7f, 2f));
							}
							else if (BrutalOptionsData.screenshaketype == 1)
							{
								num = Math.Abs(event1.intensityInitial) * 1.05f;
								num *= 15f / Vec2.Distance(DuckNetwork.localProfile.duck.position, event1.position.Value);
								BrutalScreenShake.ScreenShake(Rando.Float(1f, 2f) * num, Rando.Float(0.7f, 2f));
							}
						}
						catch { }
                    }
                }
            }
			if (!BrutalDG.patched)
            {
                BrutalDG.Patch();
                BrutalDG.patched = true;
            }
			if (!BrutalOptionsData.enablegibs)
			{
				foreach (DuckGib gib in Level.current.things[typeof(DuckGib)])
				{
					gib.Remove();
				}
			}
			if (Keyboard.Pressed(Keys.F6, false))
			{
				CoolUpdate.OpenMenu();
			}
			var menu = typeof(DuckNetwork).GetField("_ducknetMenu", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			UIMenu menu1 = (UIMenu)menu;
			if (menu1 != null && !CoolUpdate.added && menu1.open) //&& !menu1.components.Contains(this.brutalsettings))
			{
				try
				{
					menu1.Remove(this.brutalsettings);
					menu1.ResetProperties();
					CoolUpdate.added = true;
					menu1.Insert(this.brutalsettings, 5 + Profiles.active.Count, true);
					menu1.AssignDefaultSelection();
				}
				catch
				{
					
				}
				//menu1.Add(BrutalDG.brutalsetting, true);
			}
			if ((menu1 != null && !menu1.open) || menu1 == null)
			{
				CoolUpdate.added = false;
			}
			foreach (PhysicsObject obj in Level.current.things[typeof(PhysicsObject)])
			{
				if (obj is Duck && !CoolUpdate.volumes.Contains(obj))
				{
					//Level.Add(new BrutalDuckVolume(obj));
					CoolUpdate.volumes.Add(obj);
				}
				else if (obj is RagdollPart && !CoolUpdate.volumes.Contains(obj))
				{
					RagdollPart part = obj as RagdollPart;
					if (part != null)
					{
						Level.Add(new RagdollParts(obj as RagdollPart));
						CoolUpdate.volumes.Add(obj);
					}
					//Level.Add(new BrutalDuckVolume(obj));
				}
				else if (obj is TrappedDuck && !CoolUpdate.volumes.Contains(obj))
				{
					//Level.Add(new BrutalDuckVolume(obj));
					CoolUpdate.volumes.Add(obj);
				}
			}
			base.Update();
			if (this.wait > 0)
			{
				if (this.wait >= 2)
				{
					if (this.d != null && this.bullet != null && this.hitPos != null)
					{
						bool flag = false;
						if (this.d.ragdoll != null && this.d.dead && this.d.ragdoll.active && this.d.ragdoll.visible)
						{
							RagdollParts parts = null;
							RagdollPart part = null;
							if (top)
							{
								part = this.d.ragdoll.part1;
							}
							else
							{
								part = this.d.ragdoll.part3;
							}
							if (part != null)
							{
								parts = RagdollParts.GetRagdollParts(part);
							}
							if (parts != null)
							{
								parts.RemovePart(this.bullet, this.hitPos);
								flag = true;
							}
							if (BrutalOptionsData.enableblood)
							{
								if (!CoolUpdate.streams.Contains(part))
								{
									KetchupStream stream = new KetchupStream(hitPos.x, hitPos.y, bullet.travelDirNormalized + new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)), 1f, default(Vec2));
									stream.anchor = part;
									stream.thing = part;
									Level.Add(stream);
									CoolUpdate.streams.Add(part);
								}
							}
						}
						if (flag || this.wait > 16)
						{
							this.d = null;
							this.hitPos = Vec2.Zero;
							this.bullet = null;
							this.wait = 0;
							return;
						}
					}
					else if (this.d != null)
					{
						bool flag = false;
						if (this.d.ragdoll != null && this.d.dead && this.d.ragdoll.active && this.d.ragdoll.visible)
						{
							RagdollParts parts = null;
							RagdollPart part = this.d.ragdoll.part1;
							if (part != null)
							{
								parts = RagdollParts.GetRagdollParts(part);
							}
							if (parts != null)
							{
								SFX.Play(GetPath("player_killed"), 1f, 0f, 0f, false);
								parts.RemovePart(null, part.position, true);
								flag = true;
							}
							if (this.konami)
							{
								parts = RagdollParts.GetRagdollParts(this.d.ragdoll.part3);
								if (parts != null)
								{
									parts.RemovePart(null, part.position, true);
									flag = true;
								}
							}
							if (BrutalOptionsData.enableblood)
							{
								if (!CoolUpdate.streams.Contains(part))
								{
									KetchupStream stream = new KetchupStream(part.x, part.y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)), 1f, default(Vec2));
									stream.anchor = part;
									stream.thing = part;
									Level.Add(stream);
									CoolUpdate.streams.Add(part);
								}
							}
						}
						if (flag || this.wait > 16)
						{
							this.wait = 0;
							this.d = null;
							this.hitPos = Vec2.Zero;
							this.bullet = null;
							this.konami = false;
							return;
						}
					}
				}
				this.wait++;
			}
		}
		
		public static void OpenMenu()
		{
			UIMenu m = BrutalDG.brutalsettingsmenu;
			var menu = typeof(DuckNetwork).GetField("_ducknetMenu", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			UIMenu menu1 = (UIMenu)menu;
			if (menu1 != null)
			{
				menu1.Close();
			}
			m = new UIMenu("@LWING@BRUTAL DG@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, -1f, "@DPAD@ADJUST @QUACK@BACK", null, false);
			Level.Add(m);
			MonoMain.pauseMenu = m;
			m.Open();
			m.Add(new UIMenuItemToggle("Enable Gibs", null, new FieldBinding(typeof(BrutalOptionsData), "enablegibs", 0f, 1f, 0.1f), Colors.DGOrange, null, null, false, false), true);
			m.Add(new UIMenuItemSlider("Gibs Lifetime", null, new FieldBinding(typeof(BrutalOptionsData), "gibslifetime", 0f, 0.8f, 0.1f), 0.125f, Colors.DGOrange), true);
			List<string> list = new List<string>();
			for (int i = 0; i <= 1000; i++)
			{
				if (i > 0)
				{
					list.Add(i.ToString());
				}
				else
				{
					list.Add("No Limit");
				}
			}
			m.Add(new UIMenuItemNumber("Max Gibs", null, new FieldBinding(typeof(BrutalOptionsData), "gibsamount", 0f, list.Count - 1, 0.1f), 50, Colors.DGOrange, null, null, "", null, list, null), true);
			m.Add(new UIText("", Color.White, UIAlign.Center, 0f, null), true);
			m.Add(new UIMenuItemToggle("Enable Blood", null, new FieldBinding(typeof(BrutalOptionsData), "enableblood", 0f, 1f, 0.1f), Colors.DGOrange, null, null, false, false), true);
			m.Add(new UIMenuItemNumberOld("Blood Color", null, new FieldBinding(typeof(BrutalOptionsData), "bloodcolor", 0f, BrutalDG.colors.Count - 1, 0.1f), 1, default(Color), null, null, "", null, BrutalDG.colors, null), true);
			m.Add(new UIMenuItemSlider("Blood Amount", null, new FieldBinding(typeof(BrutalOptionsData), "bloodamount", 0f, 0.8f, 0.1f), 0.125f, Colors.DGOrange), true);
			m.Add(new UIMenuItemNumber("Sticky Blood", null, new FieldBinding(typeof(BrutalOptionsData), "bloodonthings", 0f, BrutalDG.bloodthings.Count - 1, 0.1f), 1, default(Color), null, null, "", null,  BrutalDG.bloodthings, null), true);
			m.Add(new UIText("", Color.White, UIAlign.Center, 0f, null), true);
			m.Add(new UIMenuItemNumber("ScreenShake", null, new FieldBinding(typeof(BrutalOptionsData), "screenshake", 0f, 400f, 0.1f), 10, Colors.DGOrange, null, null, "%", null, null, null), true);
			//m.Add(new UIMenuItemNumber("Type", null, new FieldBinding(typeof(BrutalOptionsData), "screenshaketype", 0f, BrutalDG.sstype.Count - 1, 0.1f), 1, default(Color), null, null, "", null, BrutalDG.sstype, null), true);
			m.Add(new UIText("", Color.White, UIAlign.Center, 0f, null), true);
			m.Add(new UIMenuItem("BACK", new UIMenuActionCloseMenu(m), UIAlign.Center, default(Color), true), true);
			m.SelectLastMenuItem();
		}
		
		public static List<MaterialThing> volumes = new List<MaterialThing>();
		
		public UIComponent brutalsettings = new UIMenuItem("|DGRED|BRUTAL DUCKGAME", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(CoolUpdate.OpenMenu)), UIAlign.Center, default(Color), false);
		
		public static bool added;
		
		public static List<MaterialThing> streams = new List<MaterialThing>();
		
		public int wait;
		
		public Duck d;
		
		public Bullet bullet;
		
		public Vec2 hitPos;
		
		public new bool top;
		
		public int savewait;
		
		private bool updateLobby;
		
		public bool konami;
	}
}
