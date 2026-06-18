using System;
using DuckGame;

namespace DuckGame.BrutalDG
{
	internal class BrutalScreenShake : Thing
	{
		public BrutalScreenShake(CoolUpdate u) : base()
		{
			this.upd = u;
		}
		
		public override void Update()
		{
			if (this.time > 0 && Level.current != null && Level.current.camera != null)
			{
				Camera cam = Level.current.camera;
				cam.position -= this.move;
				this.move = new Vec2(Rando.Float(-this.amount, this.amount), Rando.Float(-this.amount, this.amount));
				cam.position += this.move;
				this.time -= 0.1f;
			}
			if (this.time <= 0)
			{
				Level.current.camera.position -= this.move;
				this.move = Vec2.Zero;
			}
			if (this.upd == null)
			{
				Level.Remove(this);
			}
			base.Update();
		}
		
		public static void ScreenShake(float amount, float time)
		{
			BrutalScreenShake shake = Level.current.FirstOfType<BrutalScreenShake>();
			if (shake != null)
			{
				float num = BrutalOptionsData.screenshake * 0.004f;
				if (num > 1.58f)
					num = 20f;
				shake.amount = amount * num * 1.2f;
				shake.time = time;
			}
		}
		
		private CoolUpdate upd;
		
		public float amount;
		
		public float time;
		
		private Vec2 move;
	}
}
