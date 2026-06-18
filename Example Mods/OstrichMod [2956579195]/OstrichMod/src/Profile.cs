using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Aeris")]
    public class Profilex : Gun
     {
    private SinWave _chargeWaver = (SinWave) 0.4f;
    private float _charge;
    private int _chargeLevel;
    private float _chargeFade;

    public Profilex(float xval, float yval)
      : base(xval, yval)
    {
      this._editorName = "Profilex";
      this.ammo = 30;
      this._ammoType = (AmmoType) new ATLaser();
      this._type = "gun";
      this.graphic = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Profile"),18, 10);
      this.center = new Vec2(8f, 4f);
      this.collisionOffset = new Vec2(-8f, -4f);
      this.collisionSize = new Vec2(18f, 10f);
      this._barrelOffsetTL = new Vec2(17, 2f);
      this._fireSound = "laserRifle";
      this._fullAuto = false;
      this._numBulletsPerFire = 3;
      this._fireWait = 0.0f;
      this._kickForce = 0.5f;
      this._holdOffset = new Vec2(0.0f, -1f);
      this._flare = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("ProfileFlare"), 32, 16, false);
      this._flare.center = new Vec2(0.0f, 8f);

    }

    public override void Update()
    {
      if (this.owner == null || this.ammo <= 0)
      {
        this._charge = 0.0f;
        this._chargeLevel = 0;
      }
      this._chargeFade = Lerp.Float(this._chargeFade, (float) this._chargeLevel / 3f, 0.06f);
      base.Update();
    }

    public override void OnPressAction()
    {
    }

    public override void OnHoldAction()
    {
      if (this.ammo <= 0)
        return;
      this._charge += 0.03f;
      if ((double) this._charge > 1.0)
        this._charge = 1f;
      if (this._chargeLevel == 0)
        this._chargeLevel = 1;
      else if ((double) this._charge > 0.400000005960464 && this._chargeLevel == 1)
      {
        this._chargeLevel = 2;
        SFX.Play("phaserCharge02", 0.5f, 0.0f, 0.0f, false);
      }
      else
      {
        if ((double) this._charge <= 0.800000011920929 || this._chargeLevel != 2)
          return;
        this._chargeLevel = 3;
        SFX.Play("phaserCharge03", 0.6f, 0.0f, 0.0f, false);
      }
    }

    public override void OnReleaseAction()
    {
      if (this.ammo <= 0)
        return;
      if (this.owner != null)
      {
        this._ammoType.range = (float) this._chargeLevel * 80f;
        this._ammoType.bulletThickness = (float) (0.200000002980232 + (double) this._charge * 0.400000005960464);
        this._ammoType.penetration = (float) this._chargeLevel;
        this._ammoType.accuracy = 1f + this._charge * 0.35f;
        this._ammoType.bulletSpeed = (float) (8.0 + (double) this._charge * 10.0);
        if (this._chargeLevel == 1)
          this._fireSound = "phaserSmall";
        else if (this._chargeLevel == 2)
          this._fireSound = "phaserMedium";
        else if (this._chargeLevel == 3)
          this._fireSound = "phaserLarge";
        this.Fire();
        this._charge = 0.0f;
        this._chargeLevel = 0;
      }
      base.OnReleaseAction();
    }
  }
}
