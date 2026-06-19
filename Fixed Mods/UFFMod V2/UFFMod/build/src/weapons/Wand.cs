namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|misc")]
    public class Wand : Gun
    {
        public StateBinding _currentStateStateBinding = new StateBinding("_currentState");
        public StateBinding _animationIndexStateBinding = new StateBinding("netAnimationIndex");
        public StateBinding _frameStateBinding = new StateBinding("spriteFrame");

        public int _currentState;

        protected SpriteMap sprite;
        protected bool skipFinish;

        protected byte netAnimationIndex
        {
            get
            {
                if (sprite == null)
                    return 0;
                return (byte)sprite.animationIndex;
            }
            set
            {
                if (sprite == null || sprite.animationIndex == (int)value)
                    return;
                sprite.animationIndex = (int)value;
            }
        }

        public byte spriteFrame
        {
            get
            {
                if (sprite == null)
                    return 0;
                return (byte)sprite._frame;
            }
            set
            {
                if (sprite == null)
                    return;
                sprite._frame = (int)value;
            }
        }

        public Wand(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\wand"), 18, 25);
            sprite.AddAnimation("SCHWING", 0.6f, false, 2, 3, 4, 0, 0, 0);
            sprite.AddAnimation("held", 1f, false, 1);
            sprite.AddAnimation("unheld", 1f, false, 0);
            graphic = sprite;
            _center = new Vec2(7f, 17f);
            _barrelOffsetTL = new Vec2(14f, 17f);
            _collisionSize = new Vec2(14f, 3f);
            _collisionOffset = new Vec2(-7f, -1f);
            _holdOffset = new Vec2(2f, 2f);

            // weapon settings
            ammo = 5;
            _ammoType = new ATWandStar();
            _fireSound = Mod.GetPath<UffMod>("SFX\\wandFire.wav");
            _kickForce = 0f;
            flammable = 0.4f;
            _hasTrigger = false;
            physicsMaterial = PhysicsMaterial.Wood;

            // defaults
            _currentState = 0;
        }

        public override void Update()
        {
            if (!skipFinish && sprite.finished && _currentState != 0)
                _currentState = 0;

            base.Update();
        }

        public override void Draw()
        {
            if (_currentState == 0)
            {
                if (owner == null)
                    sprite.SetAnimation("unheld");
                else
                    sprite.SetAnimation("held");
            }
            base.Draw();
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!(type is DTIncinerate))
                base.OnDestroy(type);
            Level.Remove(this);
            for (int index = 0; index < 8; ++index)
            {
                Thing t = WoodDebris.New(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                t.hSpeed = ((Rando.Float(1f) > 0.5f ? 1f : -1f) * Rando.Float(3f));
                t.vSpeed = -Rando.Float(1f);
                Level.Add(t);
            }
            return true;
        }

        public override void OnPressAction()
        {
            if (_currentState == 0)
            {
                sprite.SetAnimation("SCHWING");
                if (ammo == 0 && isServerForObject)
                    for (int i = 0; i < Rando.Int(3, 5); i++)
                        Level.Add(new WandPixieDust(Offset(barrelOffset).x + Rando.Float(-3f, 3f), Offset(barrelOffset).y + Rando.Float(-3f, 3f)));
                _currentState = 1;
                base.OnPressAction();
            }
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner == null || _currentState == 0)
                return;
            duck.holdObstructed = false;
        }

        public override void Fire()
        {
            if (!this.loaded)
                return;
            this.firedBullets.Clear();
            if (this.ammo > 0 && (double)this._wait == 0.0)
            {
                this.ApplyKick();
                for (int index = 0; index < this._numBulletsPerFire; ++index)
                {
                    float num = this._ammoType.accuracy;
                    this._ammoType.accuracy *= 1f - this._accuracyLost;
                    this._ammoType.bulletColor = this._bulletColor;
                    float angleDegrees = this.angleDegrees;
                    float angle = (int)this.offDir >= 0 ? angleDegrees + this._ammoType.barrelAngleDegrees : angleDegrees + 180f - this._ammoType.barrelAngleDegrees;
                    if (!this.receivingPress)
                    {
                        Bullet bullet = this._ammoType.FireBullet(this.Offset(this.barrelOffset), this.owner, angle, (Thing)this);
                        if (Network.isActive && this.isServerForObject)
                        {
                            this.firedBullets.Add(bullet);
                            if (this.duck != null && this.duck.profile.connection != null)
                                bullet.connection = this.duck.profile.connection;
                        }
                    }
                    ++this.bulletFireIndex;
                    this._ammoType.accuracy = num;
                }
                this.loaded = false;
                if (!this._manualLoad)
                    this.Reload(true);
                this.firing = true;
                this._wait = this._fireWait;
                this.PlayFireSound();
                if (this.owner == null)
                {
                    Vec2 vec2 = this.barrelVector * Rando.Float(1f, 3f);
                    vec2.y += Rando.Float(2f);
                    this.hSpeed -= (float)(double)vec2.x;
                    this.vSpeed -= (float)(double)vec2.y;
                }
                this._accuracyLost += this.loseAccuracy;
                if ((double)this._accuracyLost <= (double)this.maxAccuracyLost)
                    return;
                this._accuracyLost = this.maxAccuracyLost;
            }
            else
            {
                if (this.ammo > 0 || (double)this._wait != 0.0)
                    return;
                this._wait = this._fireWait;
            }
        }
    }
}
