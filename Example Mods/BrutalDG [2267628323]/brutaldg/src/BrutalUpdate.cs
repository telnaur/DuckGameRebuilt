using System;
using DuckGame;
using System.Linq;

namespace DuckGame.BrutalDG
{
	internal class BrutalUpdate : Thing, IAutoUpdate
	{
		public BrutalUpdate() : base()
		{
			AutoUpdatables.Add(this);
		}
		
		public override void Update()
		{
			if (Level.current != null && Level.current.things[typeof(CoolUpdate)].Count() == 0)
			{
				Level.Add(new CoolUpdate(0f, 0f));
			}
			else if (Level.current.things[typeof(CoolUpdate)].Count() > 1)
			{
				foreach (CoolUpdate upd in Level.current.things[typeof(CoolUpdate)])
				{
					Level.Remove(upd);
					if (Level.current.things[typeof(CoolUpdate)].Count() == 0)
					{
						break;
					}
				}
			}
			base.Update();
		}
	}
}
