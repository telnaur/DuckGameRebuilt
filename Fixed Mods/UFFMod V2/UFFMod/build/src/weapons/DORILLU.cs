using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    /*
    DEAR MODDERS:
        please do not try and learn from this code it is horrible and awful and bad and the very first thing i coded for this mod
        thank you
    */
    [EditorGroup("uff|weapons|melee")]
    [BaggedProperty("isSuperWeapon", true)]
    public class DORILLU : Gun
    {
        public StateBinding _hasFiredStateBinding = new StateBinding("_hasFired");
        public StateBinding _launcherStateBinding = new StateFlagBinding(new string[2]
        {
            "_hasLaunched",
            "_goingUp"
        });
        public StateBinding _thrownAwayStateBinding = new StateBinding("_thrownAway");
        public StateBinding _animCycleStateBinding = new StateBinding("_animCycle");
        public StateBinding _crackednessStateBinding = new StateBinding("_crackedness");
        public StateBinding _savedDuckStateBinding = new StateBinding("_savedDuck");
        public StateBinding netSFX_drillklangStateBinding = new NetSoundBinding("netSFX_drillklang");
        public StateBinding netSFX_explodeStateBinding = new NetSoundBinding("netSFX_explode");
        public StateBinding netSFX_missileStateBinding = new NetSoundBinding("netSFX_missile");
        public StateBinding netSFX_clashStateBinding = new NetSoundBinding("netSFX_clash");

        public NetSoundEffect netSFX_drillklang = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\drillklang")
        });
        public NetSoundEffect netSFX_explode = new NetSoundEffect(new string[1]
        {
            "explode"
        })
        {
            volume = 0.2f
        };
        public NetSoundEffect netSFX_missile = new NetSoundEffect(new string[1]
        {
            "missile"
        });
        public NetSoundEffect netSFX_clash = new NetSoundEffect(new string[1]
        {
            "chainsawClash"
        })
        {
            volume = 0.4f
        };

        public bool _hasFired;
        public bool _hasLaunched;
        public bool _goingUp;
        public bool _thrownAway;
        public int _animCycle;
        public int _crackedness;
        public int _cooldown;
        public Duck _savedDuck;

        private List<MaterialThing> thingsHit = new List<MaterialThing>();
        private float minHSpeed;

        private SpriteMap sprite;

        public DORILLU(float x, float y) : base(x, y)
        {
            // editor settings
            _editorName = "Drill";
            _bio = "A manly drill that can pierce the heavens.";

            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\DORILLLLLLLLLLLLLLLLLLLUUU"), 25, 20);
            graphic = sprite;
            center = new Vec2(7f, 10f);
            _collisionSize = new Vec2(14f, 10f);
            _collisionOffset = new Vec2(-7f, -5f);
            _holdOffset = new Vec2(3f, 2f);

            // weapon settings
            ammo = 120;
            weight = 7.5f;
            _hasTrigger = false;
            physicsMaterial = PhysicsMaterial.Default; // magnet gun is a bitch so it can't be metal

            // defaults
            _hasFired = false;
            _hasLaunched = false;
            _goingUp = false;
            _thrownAway = false;
            _animCycle = 0;
            _crackedness = 0;
        }

        public override void Terminate()
        {
            thingsHit.Clear();
            base.Terminate();
        }

        public override void OnPressAction()
        {
            if (!_hasFired)
            {
                if (Network.isActive && isServerForObject)
                    netSFX_drillklang.Play(1f, 0f);
                else
                    SFX.Play(Mod.GetPath<UffMod>("SFX\\drillklang"), 1f, 0f, 0f, false);
                _animCycle = 0;
                _hasFired = true;
                _thrownAway = false;
                base.OnPressAction();
            }
        }

        private void StateChange(int i)
        {
            _crackedness = i;
            _animCycle += 5;
        }

        public override void Thrown()
        {
            // normal throw (disabled)
            /*
            if (hasFired && ammo > 0 && state != 3)
            {
                state = 3;
                ammo = 0;
                if (Network.isActive && isServerForObject)
                    netSFX_explode.Play(1f, 0f);
                else
                    SFX.Play("explode", 1f, 0f, 0f, false);
            }
            */

            if (_hasFired && _crackedness != 3)
                DrillLaunch();
            else
                base.Thrown();
        }

        public override void Update()
        {
            if (duck != null)
                _savedDuck = duck;

            BrokenCheck();

            if (((!_hasFired || owner == null) && _crackedness == 3 && !_hasLaunched))
                _animCycle = 21;
            else if ((!_hasFired || owner == null) && !_hasLaunched)
                _animCycle = 0;

            if (_hasLaunched)
            {
                if (isServerForObject && (x > Level.current.bottomRight.x + 200 || x < Level.current.topLeft.x - 200))
                    Level.Remove(this);
                if (_goingUp)
                {
                    if (_vSpeed < 0.8f)
                        _vSpeed += 0.35f;
                    else
                        _goingUp = false;
                }
                else
                {
                    if (_vSpeed > -0.8f)
                        _vSpeed -= 0.35f;
                    else
                        _goingUp = true;
                }

                if (Math.Abs(_hSpeed) < Math.Abs(minHSpeed))
                    _hSpeed = minHSpeed;
                if (Math.Abs(_hSpeed) < 6.3f)
                    _hSpeed = MathHelper.Lerp(_hSpeed, 6.3f * offDir, 0.16f);
                else
                    _hSpeed = MathHelper.Lerp(_hSpeed, 6.3f * offDir, 0.24f);
                Level.Add(SmallSmoke.New(x, y + Rando.Float(-2f, 2f)));
            }

            if (ammo <= 80 && ammo > 40 && _crackedness != 1)
                StateChange(1);
            else if (ammo <= 40 && ammo > 0 && _crackedness != 2)
                StateChange(2);

            if (owner != null && _animCycle >= 5 && _animCycle < 21)
            {
                _collisionSize = new Vec2(22, 12);
                _collisionOffset = new Vec2(-11, -6);
            }
            else
            {
                _collisionSize = new Vec2(14, 10);
                _collisionOffset = new Vec2(-7, -5);
            }

            if (_hasFired && (duck != null || _hasLaunched))
            {
                if (_crackedness != 3)
                {
                    foreach (BlockGroup blockGroup in Level.CheckCircleAll<BlockGroup>(new Vec2(x + offDir * 8f, y), 16f))
                        GigaDrillBreak(blockGroup);
                    foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(topLeft, bottomRight - new Vec2(0f, _hasLaunched ? 0f : 4f)))
                    {
                        if (thingsHit.Contains(materialThing) || materialThing is DORILLU || materialThing == this || materialThing == duck || (_savedDuck != null && materialThing == _savedDuck) || materialThing is Equipment || (materialThing is Gun && !(materialThing is DORILLU)))
                            continue;
                        /* else if (materialThing is DORILLU)
                            CreateTheHeavens((DORILLU)materialThing); */
                        else if (materialThing is Window || materialThing is Duck)
                            PierceTheHeavens(new DTImpale(this), materialThing);
                        else if (materialThing is Door)
                            PierceTheHeavens(new DTRocketExplosion(this), materialThing);
                        else if (materialThing is PhysicsObject && !(materialThing is Duck) && materialThing != this && materialThing != owner && !(materialThing is Hat) && !(materialThing is Gun))
                            PierceTheHeavens(new DTRocketExplosion(this), materialThing);
                        else if (materialThing is Block)
                            GigaDrillBreak(materialThing);
                    }
                    _animCycle++;
                    if (_crackedness == 0 && _animCycle > 7)
                        _animCycle = 5;
                    else if (_crackedness == 1)
                    {
                        if ((_animCycle >= 5 && _animCycle < 10) || _animCycle > 12)
                            _animCycle = 10;
                    }
                    else if (_crackedness == 2)
                    {
                        if ((_animCycle >= 5 && _animCycle < 15) || _animCycle > 17)
                            _animCycle = 15;
                    }
                    if (owner != null)
                    {
                        if (Math.Abs(owner.hSpeed) < 5.8f)
                            owner.hSpeed = MathHelper.Lerp(owner._hSpeed, owner.offDir * 5.8f, 0.24f);
                        else
                            owner.hSpeed = MathHelper.Lerp(owner._hSpeed, owner.offDir * 5.8f, 0.32f);
                    }
                }
                else
                {
                    _animCycle++;
                    if (_animCycle >= 5)
                        _animCycle = 20;
                }
            }

            sprite.frame = _animCycle;

            base.Update();
        }

        private void DrillLaunch()
        {
            // launch drill
            _hasLaunched = true;
            if (Network.isActive && isServerForObject)
                netSFX_missile.Play(0.8f, 0f);
            else
                SFX.Play("missile", 0.8f, 0f, 0f, false);
            minHSpeed = 1.3f * offDir;
            if (owner != null)
                owner._hSpeed += -8f * offDir;
            canPickUp = false;
        }

        private bool BrokenCheck()
        {
            if (ammo <= 0 && _crackedness == 2)
            {
                Level.Add(new ExplosionPart(x + Rando.Float(-1f, 1f), y + Rando.Float(-1f, 1f), false));
                for (int i = 0; i < 4; i++)
                {
                    Level.Add(new ExplosionPart(x + Rando.Float(-16f, 16f), y + Rando.Float(-16f, 16f), false));
                }
                if (Network.isActive && isServerForObject)
                    netSFX_explode.Play();
                else
                    SFX.Play("explode", 0.2f, 0f, 0f, false);
                _crackedness = 3;
                if (_hasLaunched)
                    Level.Remove(this);
                return true;
            }
            if (ammo <= 0 || _crackedness == 3)
            {
                if (_hasLaunched)
                    Level.Remove(this);
                return true;
            }
            return false;
        }

        /*
        private void CreateTheHeavens(DORILLU otherDrill)
        {
            if (!otherDrill._hasFired || otherDrill._crackedness == 3)
                return;
            Level.Add(new ExplosionPart(x + Rando.Float(-1f, 1f), y + Rando.Float(-1f, 1f), false));
            for(int i = 0 ; i < 16 ; i++)
                Level.Add(new ExplosionPart(x + Rando.Float(-32f, 32f), y + Rando.Float(-32f, 32f), false));
            SFX.Play("explode", 0.2f, 0f, 0f, false);
            SFX.Play("chainsawClash", 0.4f, 0f, 0f, false);
            ammo -= 3;
            otherDrill.ammo -= 3;
            if (otherDrill.x > x)
            {
                if (owner != null)
                    duck._hSpeed = -15f;
                else
                    _hSpeed = -15f;
                if (otherDrill.owner != null)
                    otherDrill.duck._hSpeed = 15f;
                else
                    otherDrill._hSpeed = 15f;
            }
            else
            {
                if (owner != null)
                    duck._hSpeed = 15f;
                else
                    _hSpeed = 15f;
                if (otherDrill.owner != null)
                    otherDrill.duck._hSpeed = -15f;
                else
                    otherDrill._hSpeed = -15f;
            }
        }
        */

        private void PierceTheHeavens(DestroyType destroyType, MaterialThing materialThing)
        {
            thingsHit.Add(materialThing);
            if (materialThing.owner == null)
                Fondle(materialThing);
            if (materialThing is Door)
            {
                Level.Add(new ExplosionPart(materialThing.x + Rando.Float(-1f, 1f), materialThing.y + Rando.Float(-1f, 1f), false));
                SFX.Play("explode", 0.2f, 0f, 0f, false);
                SFX.Play("chainsawClash", 0.4f, 0f, 0f, false);
                ammo -= 7;
                Level.Remove(materialThing);
                Knockback();
            }
            else if (materialThing.Destroy(destroyType))
            {
                Level.Add(new ExplosionPart(materialThing.x + Rando.Float(-1f, 1f), materialThing.y + Rando.Float(-1f, 1f), false));
                SFX.Play("explode", 0.3f, 0f, 0f, false);
                SFX.Play("chainsawClash", 0.6f, 0f, 0f, false);
                ammo -= 7;
                Knockback();
            }
        }

        private void GigaDrillBreak(MaterialThing block)
        {
            if (block is BlockGroup)
            {
                BlockGroup bg = block as BlockGroup;
                bg.Wreck();
            }
            else if (block is AutoBlock)
            {
                thingsHit.Add(block);
                Level.Add(new GlobalExplosion(block.x + Rando.Float(-1f, 1f), block.y + Rando.Float(-1f, 1f)));
                netSFX_explode.Play();
                netSFX_clash.Play();
                ammo -= 8;
                foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(block.position, 16f))
                {
                    if (materialThing is BlockGroup)
                    {
                        BlockGroup b = materialThing as BlockGroup;
                        b.Wreck();
                    }
                    else if (materialThing is PhysicsObject && materialThing != (_savedDuck == null ? null : _savedDuck) && materialThing != this && materialThing != (duck == null ? null : duck))
                    {
                        if (materialThing.isLocal && materialThing.owner == null)
                            Fondle(materialThing);
                        ((PhysicsObject)materialThing).sleeping = false;
                        materialThing.vSpeed = -2f;
                    }
                }
                Knockback();
                HashSet<ushort> blocksToDestroy = new HashSet<ushort>();
                blocksToDestroy.Add((block as AutoBlock).blockIndex);
                ((Block)block).skipWreck = true;
                ((Block)block).shouldWreck = true;
                if (Network.isActive && isLocal)
                    Send.Message(new NMDestroyBlocks(blocksToDestroy));
            }
            else if (_hasLaunched)
            {
                ammo = 0;
                while (_crackedness < 2)
                    StateChange(_crackedness + 1);
                BrokenCheck();
            }
        }

        public void Knockback()
        {
            if (duck == null)
                return;
            if (duck.sliding)
                duck.sliding = false;
            duck.hSpeed = -3.5f * duck.offDir;
            duck.vSpeed = -0.2f;
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner == null)
                return;
            duck.holdObstructed = false;
        }

        public override void Fire()
        {
            /* do nothing */
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (_hasLaunched && impactPowerH >= 1f && with is Duck && with != _savedDuck && ((offDir > 0 && from == ImpactedFrom.Right) || (offDir < 0 && from == ImpactedFrom.Left)))
                ((Duck)with).Kill(new DTImpale(with));
            base.OnImpact(with, from);
        }
    }
}
