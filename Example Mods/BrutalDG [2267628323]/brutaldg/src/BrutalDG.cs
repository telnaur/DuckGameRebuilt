using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using System.Threading;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("Brutal DuckGame")]

// The author of the mod
[assembly: AssemblyCompany("BananaBomb")]

// The description of the mod
[assembly: AssemblyDescription("Just ketchup.")]

// The mod's version
[assembly: AssemblyVersion("1.0.5.1")]

namespace DuckGame.BrutalDG
{
	internal static class SawsTouch
	{
		private static void Prefix(Saws __instance, MaterialThing with)
		{
			if (with != null && with is RagdollPart)
			{
				RagdollParts parts = RagdollParts.GetRagdollParts(with as RagdollPart);
				if (parts != null && parts._swordtimer <= 0f && parts.ragdoll != null)
				{
					parts.RemovePart(new Bullet(parts.ragdoll.x, parts.ragdoll.y, new ATTracer(), Maths.DegToRad(Rando.Float(360f)), null, false, 0f, true, true), parts.ragdoll.position, false);
					parts._swordtimer = 3f;
				}
			}
		}
	}
	internal static class DuckKill
	{
		private static void Prefix(Duck __instance, DestroyType type)
		{
			float num = 1f;
			if (BrutalOptionsData.screenshaketype == 1 && Network.isActive)
            {
				if (__instance != DuckNetwork.localProfile.duck)
					num = 0f;
			}
			//BrutalScreenShake.ScreenShake(Rando.Float(1f, 2f) * num, Rando.Float(0.7f, 2f));
			if (type is DTCrush)
			{
				Duck duck = __instance;
				CoolUpdate upd = Level.current.FirstOfType<CoolUpdate>();
				if (upd != null)
				{
					upd.wait = 1;
					upd.d = duck;
				}
			}
			else if (type is DTFall && __instance.inputProfile != null && __instance.inputProfile.CheckCode(Input.konamiCode))
			{
				Duck duck = __instance;
				CoolUpdate upd = Level.current.FirstOfType<CoolUpdate>();
				if (upd != null)
				{
					upd.wait = 1;
					upd.d = duck;
					upd.konami = true;
				}
			}
		}
		
		private static void Konami(InputProfile __instance, InputCode c)
		{
			if (c == Input.konamiCode)
			{
				CoolUpdate upd = Level.current.FirstOfType<CoolUpdate>();
				if (upd != null)
				{
					foreach (Duck duck in Level.current.things[typeof(Duck)])
					{
						if (duck.inputProfile == __instance)
						{
							upd.wait = 1;
							upd.d = duck;
						}
					}
				}
			}
		}
	}
	
	internal static class Init
	{
        private static void Prefix(Level __instance, Thing t)
		{
			if (t is QuadLaserBullet || t is DeathBeam || t is Net || t is CookedDuck || t is TrappedDuck)
			{
				float amount = 0.2f;
				if (t is QuadLaserBullet)
				{
					amount = 1.2f;
				}
				else if (t is DeathBeam)
				{
					amount = 2f;
				}
				else if (t is Net)
				{
					if ((BrutalOptionsData.screenshake * 0.004f) > 0.1f)
					{
						amount = 0.6f;
					}
					else
					{
						amount = 0f;
					}
				}
				else if (t is CookedDuck || t is TrappedDuck)
				{
					amount = 0.8f;
				}
				if (BrutalOptionsData.screenshaketype > 0)
				{
					amount = 0f;
				}
				BrutalScreenShake shake = Level.current.FirstOfType<BrutalScreenShake>();
				if (shake != null)
				{
					float num = BrutalOptionsData.screenshake * 0.004f;
					if (num > 1.58f)
						num = 20f;
					shake.amount = Rando.Float(1f, 2f) * num * 1.2f * amount;
					shake.time = Rando.Float(0.7f, 2f);
				}
			}
		}
	}
	
	internal static class Fire
	{
		private static void ShakeScreen()
		{
			BrutalScreenShake.ScreenShake(Rando.Float(1f, 2f), Rando.Float(0.7f, 2f));
		}
	}
	
	internal static class OnHit
	{
		private static void Duck(Duck __instance, Bullet bullet, Vec2 hitPos)
		{
			Duck duck = __instance;
			CoolUpdate upd = Level.current.FirstOfType<CoolUpdate>();
			if (upd != null)
			{
				upd.top = (hitPos.y < duck.top + 10f);
				upd.wait = 1;
				upd.d = duck;
				upd.bullet = bullet;
				upd.hitPos = hitPos;
			}
		}
		private static void Ragdoll(RagdollPart __instance, Bullet bullet, Vec2 hitPos)
		{
			CoolUpdate upd = Level.current.FirstOfType<CoolUpdate>();
			if (upd.wait == 0 || upd == null)
			{
				RagdollPart duck = __instance;
				if (BrutalOptionsData.enablegibs)
				{
					RagdollParts parts = RagdollParts.GetRagdollParts(duck);
					if (parts != null)
					{
						parts.RemovePart(bullet, hitPos);
					}
				}
				if (BrutalOptionsData.enableblood)
				{
					if (!CoolUpdate.streams.Contains(duck))
					{
						KetchupStream stream = new KetchupStream(hitPos.x, hitPos.y, bullet.travelDirNormalized + new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)), 1f, default(Vec2));
						stream.anchor = duck;
						stream.thing = duck;
						Level.Add(stream);
						CoolUpdate.streams.Add(duck);
					}
				}
			}
		}
		private static void Trapped(TrappedDuck __instance, Bullet bullet, Vec2 hitPos)
		{
			TrappedDuck duck = __instance;
			if (BrutalOptionsData.enableblood)
			{
				if (!CoolUpdate.streams.Contains(duck))
				{
					KetchupStream stream = new KetchupStream(hitPos.x, hitPos.y, bullet.travelDirNormalized + new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)), 1f, default(Vec2));
					stream.anchor = duck;
					stream.thing = duck;
					Level.Add(stream);
					CoolUpdate.streams.Add(duck);
				}
			}
		}
	}
	
	public class BrutalDG : DisabledMod
	{
		public static ModConfiguration config;
		
		private static PropertyInfo steamIdField = typeof(ModConfiguration).GetProperty("workshopID", BindingFlags.Instance | BindingFlags.NonPublic);
		
		private static PropertyInfo disabledField = typeof(ModConfiguration).GetProperty("disabled", BindingFlags.Instance | BindingFlags.NonPublic);

		public static bool disabled
		{
			get
			{
				return (bool)disabledField.GetValue(config, new object[0]);
			}
			set
			{
				disabledField.SetValue(config, value, new object[0]);
			}
		}

		public static string replaceData
		{
			get
			{
				return config.isWorkshop ? steamIdField.GetValue(config, new object[0]).ToString() : "LOCAL";
			}
		}
		
		public override Priority priority
		{
			get { return base.priority; }
		}

		protected override void OnPreInitialize()
		{
			config = base.configuration;
			base.OnPreInitialize();
		}

		protected override void OnPostInitialize()
		{
			Form form = (Form)Control.FromHandle(MonoMain.instance.Window.Handle);
			form.FormClosing += new FormClosingEventHandler(BrutalDG.FormClosed);
			BrutalDG.LoadSettings();
			Thread thread = new Thread(threadB);
			thread.Start();
		}

		public static void LoadTextures()
		{
			foreach (DuckPersona persona in Persona.all)
			{
				Tex2D part1 = Graphics.Recolor(Content.Load<Tex2D>(BrutalDG.GetPath<BrutalDG>("ragdollparts1")), persona.color);
				BrutalDG._parts1Sprite.Add(Persona.Number(persona), part1);
				Tex2D part2 = Graphics.Recolor(Content.Load<Tex2D>(BrutalDG.GetPath<BrutalDG>("ragdollparts2")), persona.color);
				BrutalDG._parts2Sprite.Add(Persona.Number(persona), part2);
				Tex2D over = Graphics.Recolor(Content.Load<Tex2D>(BrutalDG.GetPath<BrutalDG>("ragdollparts1overlay")), persona.color);
				BrutalDG._partsoverlaySprite.Add(Persona.Number(persona), over);
				Tex2D gibs = Graphics.Recolor(Content.Load<Tex2D>(BrutalDG.GetPath<BrutalDG>("duckketchupducks1")), persona.color);
				BrutalDG._gibsSprite.Add(persona, gibs);
			}
		}

		public static void FormClosed(object sender, EventArgs e)
		{
			bool flag = !Program.commandLine.Contains("-download");
			if (!flag)
			{
				disabled = false;
				typeof(ModLoader).GetMethod("DisabledModsChanged", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[0]);
			}
		}
		
		public void threadB()
		{
			while (Level.current == null || !(Level.current.ToString() == "DuckGame.TitleScreen") && !(Level.current.ToString() == "DuckGame.TeamSelect2"))
				Thread.Sleep(200);
			if (!upd)
			{
				autoupd = new BrutalUpdate();
				upd = true;
			}
		}
		
		public static void Patch()
		{
			bool flag = false;
			Assembly Harmony = null;
			Dictionary<Assembly, Mod> _modAssemblies = (typeof(ModLoader).GetField("_modAssemblies", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as Dictionary<Assembly, Mod>);
			foreach (Assembly a in _modAssemblies.Keys)
			{
				if (a.GetType("HarmonyLoader.Loader") != null)
				{
					Harmony = a;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Harmony = Assembly.Load(File.ReadAllBytes(GetPath<BrutalDG>("HarmonyLoader") + ".dll"));
				(typeof(ModLoader).GetField("_modAssemblies", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as Dictionary<Assembly, Mod>).Add(Harmony, new DisabledMod());
			}
			if (Harmony != null)
			{
				string SpecialCode = "Unknown error";
				string dummystring = "d";
				try
				{
					Type t = Harmony.GetType("HarmonyLoader.Loader");
					MethodInfo Patch = t.GetMethod("Patch", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					Patch.Invoke(null, new object[] { SGMI(typeof(Duck), "Hit", out SpecialCode), SGMI(typeof(OnHit), "Duck", out dummystring), null, null });
					Patch.Invoke(null, new object[] { SGMI(typeof(Duck), "Kill", out SpecialCode), SGMI(typeof(DuckKill), "Prefix", out dummystring), null, null });
					Patch.Invoke(null, new object[] { SGMI(typeof(Level), "AddThing", out SpecialCode), SGMI(typeof(Init), "Prefix", out dummystring), null, null });
					Patch.Invoke(null, new object[] { SGMI(typeof(RagdollPart), "Hit", out SpecialCode), SGMI(typeof(OnHit), "Ragdoll", out dummystring), null, null });
					Patch.Invoke(null, new object[] { SGMI(typeof(Saws), "Touch", out SpecialCode), SGMI(typeof(SawsTouch), "Prefix", out dummystring), null, null });
				}
				catch
				{
					
				}
			}
			else
			{
				DevConsole.Log("No harmony", Color.Red, 2f, -1);
			}
		}
		public static MethodInfo SGMI(Type type, string Methodname, out string Log)
		{
			MethodInfo method = type.GetMethod(Methodname, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			Log = "Tried to patch " + Methodname;
			return method;
		}
		
		public static void SaveSettings()
		{
			string path = BrutalDG.GetPath<BrutalDG>("settings.txt");
			try
			{
				File.WriteAllText(path, string.Empty);
			}
			catch
			{
				
			}
			if (!File.Exists(path) || File.ReadLines(path).Count() <= 1)
			{
				using (StreamWriter streamWriter = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
				{
					streamWriter.WriteLine("don't edit");
				}
			}
			using (StreamWriter streamWriter = new StreamWriter(new FileStream(path, FileMode.Open, FileAccess.Write)))
			{
				streamWriter.BaseStream.Seek(0L, SeekOrigin.End);
				streamWriter.WriteLine("enablegibs = " + BrutalOptionsData.enablegibs.ToString().ToLower());
				streamWriter.WriteLine("gibslifetime = " + BrutalOptionsData.gibslifetime.ToString());
				streamWriter.WriteLine("gibsamount = " + BrutalOptionsData.gibsamount.ToString());
				streamWriter.WriteLine("enableblood = " + BrutalOptionsData.enableblood.ToString().ToLower());
				streamWriter.WriteLine("bloodamount = " + BrutalOptionsData.bloodamount.ToString());
				streamWriter.WriteLine("bloodcolor = " + BrutalOptionsData.bloodcolor.ToString());
				streamWriter.WriteLine("screenshake = " + BrutalOptionsData.screenshake.ToString());
				streamWriter.WriteLine("bloodonthings = " + BrutalOptionsData.bloodonthings.ToString().ToLower());
				streamWriter.WriteLine("screenshaketype = " + BrutalOptionsData.screenshaketype.ToString());
			}
		}
		
		public static void LoadSettings()
		{
			string path = BrutalDG.GetPath<BrutalDG>("settings.txt");
			if (!File.Exists(path) || File.ReadLines(path).Count() <= 1)
			{
				BrutalOptionsData.enableblood = true;
				BrutalOptionsData.gibslifetime = 0.8f;
				BrutalOptionsData.gibsamount = 500;
				BrutalOptionsData.enablegibs = true;
				BrutalOptionsData.bloodamount = 0.25f;
				BrutalOptionsData.screenshake = 80;
				BrutalOptionsData.bloodonthings = 3;
				BrutalOptionsData.screenshaketype = 0;
			}
			else
			{
				IEnumerable<string> lines = File.ReadLines(path);
				foreach (string line in lines)
				{
					string text = "";
					foreach(char ch in line.ToCharArray())
					{
						if((Char.IsNumber(ch)) || (Char.IsPunctuation(ch)))
						{
							text += ch.ToString();
						}
					}
					if (line.Contains("enablegibs"))
					{
						BrutalOptionsData.enablegibs = line.Contains("true");
					}
					else if (line.Contains("gibslifetime"))
					{
						BrutalOptionsData.gibslifetime = float.Parse(text);
					}
					else if (line.Contains("gibsamount"))
					{
						BrutalOptionsData.gibsamount = int.Parse(text);
					}
					else if (line.Contains("enableblood"))
					{
						BrutalOptionsData.enableblood = line.Contains("true");
					}
					else if (line.Contains("bloodamount"))
					{
						BrutalOptionsData.bloodamount = float.Parse(text);
					}
					else if (line.Contains("bloodcolor"))
					{
						BrutalOptionsData.bloodcolor = int.Parse(text);
					}
					else if (line.Contains("screenshake") && !line.Contains("screenshaketype"))
					{
						BrutalOptionsData.screenshake = int.Parse(text);
					}
					else if (line.Contains("bloodonthings"))
					{
						BrutalOptionsData.bloodonthings = int.Parse(text);
					}
					else if (line.Contains("screenshaketype"))
					{
						BrutalOptionsData.screenshaketype = int.Parse(text);
					}
				}
			}
		}
		
		public static void MakeDecals(Thing t)
		{
			if (!BrutalDG.drawing)
			{
				BrutalDG.drawing = true;
				if (BloodThings.GetThing(t) != null && BloodThings.GetThing(t).drawing)
				{
					BloodThings thing = BloodThings.GetThing(t);
					if (thing != null)
					{
						if (thing._amount > thing._amountdid)
						{
							if (t.graphic != null && t.graphic.texture != null)
							{
								System.IO.MemoryStream stream = new System.IO.MemoryStream();
								Texture2D tex = t.graphic.texture;
								tex.SaveAsPng(stream, tex.Width, tex.Height);
								Bitmap bmp = new Bitmap(stream);
								stream.Close();
								stream.Dispose();
								if (bmp != null)
								{
									using (var bitmap = new Bitmap(bmp))
									{
										List<Vec2> _firstpos = new List<Vec2>();
										List<Vec2> _pos = new List<Vec2>();
										List<System.Drawing.Color> _colors = new List<System.Drawing.Color>();
										int x5 = t.graphic.texture.width / t.graphic.width;
										int y5 = t.graphic.texture.height / t.graphic.height;
										for (int x1 = 0; x1 < t.graphic.width * x5; x1++)
										{
											for (int y1 = 0; y1 < t.graphic.height * y5; y1++)
											{
												System.Drawing.Color pixel = bitmap.GetPixel(x1, y1);
												if (pixel.A > 100 && pixel.GetBrightness() > 0.1f && !thing._blood.Contains(new Vec2(x1, y1)))
												{
													if (x1 < t.graphic.width && y1 < t.graphic.height)
													{
														_firstpos.Add(new Vec2(x1, y1));
													}
													_colors.Add(pixel);
													_pos.Add(new Vec2(x1, y1));
												}
											}
										}
										if (_firstpos.Count < 5)
										{
											BrutalDG.drawing = false;
											return;
										}
										int num4 = Rando.Int(_firstpos.Count - 1);
										int frame = 0;
										for (int y3 = 0; y3 < t.graphic.texture.height / t.graphic.height; y3++)
										{
											for (int x3 = 0; x3 < t.graphic.texture.width / t.graphic.width; x3++)
											{
												int x = (int)_firstpos[num4].x + t.graphic.width * x3;
												int y = (int)_firstpos[num4].y + t.graphic.height * y3;
												System.Drawing.Color c = bitmap.GetPixel(x, y);
												if (c.A > 100 && c.GetBrightness() > 0.1f && !thing._blood.Contains(new Vec2(x, y)) && _colors.Count > 7)
												{
													Color color = new Color(240, 48, 48);
													if (BrutalOptionsData.bloodcolor < BrutalDG.blood.Count)
													{
														color = BrutalDG.blood[BrutalOptionsData.bloodcolor];
													}
													if (BrutalOptionsData.bloodcolor == BrutalDG.blood.Count)
													{
														color = BrutalDG.blood[Rando.Int(BrutalDG.blood.Count - 1)];
													}
													float r = color.r;
													float g = color.g;
													float b = color.b;
													if (BrutalOptionsData.bloodcolor != BrutalDG.blood.Count)
													{
														r *= c.GetBrightness();
														g *= c.GetBrightness();
														b *= c.GetBrightness();
													}
													color.r = (byte)r;
													color.g = (byte)g;
													color.b = (byte)b;
													KetchupPixelThing pixel = new KetchupPixelThing(t, new Vec2((int)_firstpos[num4].x, (int)_firstpos[num4].y), color, thing);
													if (!thing._ketchup.ContainsKey(pixel))
													{
														thing._ketchup.Add(pixel, frame + x3);
													}
													//System.Drawing.Color color1 = System.Drawing.Color.FromArgb(color.a, color.r, color.g, color.b);
													//bitmap.SetPixel(x, y, color1);
													thing._blood.Add(new Vec2(x, y));
												}
											}
											frame += t.graphic.texture.height / t.graphic.height;
										}
										//BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
										//bool process = Rando.Int(3) > 1;
										//int w = 0;
										//int num = bitmapData.Width * bitmapData.Height;
										//int* ptr = (int*)((void*)bitmapData.Scan0);
										//while (w < num)
										//{
										//	if (process && *ptr == -65281)
										//	{
										//		*ptr = 0;
										//	}
										//	else
										//	{
										//		byte* ptr2 = (byte*)ptr;
										//		byte b = *ptr2;
										//		*ptr2 = ptr2[2];
										//		ptr2[2] = b;
										//		float num2 = (float)ptr2[3] / 255f;
										//		for (int j = 0; j < 3; j++)
										//		{
										//			ptr2[j] = (byte)((float)ptr2[j] * num2);
										//		}
										//	}
										//	w++;
										//	ptr++;
										//}
										//int[] array = new int[bitmapData.Width * bitmapData.Height];
										//Marshal.Copy(bitmapData.Scan0, array, 0, array.Length);
										//Texture2D texture2D = new Texture2D(Graphics.device, bitmapData.Width, bitmapData.Height);
										//texture2D.SetData<int>(array);
										//t.graphic.texture = texture2D;
										////t.graphic.texture.SetData<int>(array);
										//bitmap.UnlockBits(bitmapData);
										thing._amountdid++;
										if (thing._amountdid > thing._amount)
											thing._amountdid = thing._amount;
										
										
									}
								}
								
							}
						}
						
					}
				}
			}
			BrutalDG.drawing = false;
		}
		
		static BrutalUpdate autoupd;
		
		public static UIMenu brutalsettingsmenu;
		
		public static List<string> colors = new List<string>
		{
			"|DGRED|RED",
			"|DGGREEN|GREEN",
			"|DGBLUE|BLUE",
			"|YELLOW|YELLOW",
			"|DGPURPLE|PURPLE",
			"|RED|R|ORANGE|A|YELLOW|I|GREEN|N|DGBLUE|B|BLUE|O|PURPLE|W"
		};
		
		public static List<Color> blood = new List<Color>
		{
			new Color(240, 48, 48),
			new Color(50, 240, 56),
			new Color(50, 220, 240),
			new Color(255, 225, 35),
			new Color(130, 50, 240)
		};
		
		public static bool upd;
		
		public static bool patched;
		
		private static bool drawing;
		
		public static bool bloodblocks;
		
		public static bool bloodguns;
		
		public static List<string> bloodthings = new List<string>
		{
			"OFF",
			"GUNS",
			"BLOCKS",
			"ALL"
		};

		public static List<string> sstype = new List<string>
		{
			"REGULAR",
			"RADIUS",
			"PLAYER"
		};

		public static Dictionary<int, Tex2D> _parts1Sprite = new Dictionary<int, Tex2D>();

		public static Dictionary<int, Tex2D> _parts2Sprite = new Dictionary<int, Tex2D>();

		public static Dictionary<int, Tex2D> _partsoverlaySprite = new Dictionary<int, Tex2D>();

		public static Dictionary<DuckPersona, Tex2D> _gibsSprite = new Dictionary<DuckPersona, Tex2D>();

		public static Tex2D _blank = new Tex2D(32, 32);

		public static bool _addedTex;
	}
}
