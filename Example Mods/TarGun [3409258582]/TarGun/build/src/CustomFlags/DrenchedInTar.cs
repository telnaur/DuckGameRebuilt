using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.TarGunMod.CustomFlags
{
    public class DrenchedInTar : Thing
    {
        public StateBinding _isFlaggedBinding = new StateBinding("IsFlagged");
        public StateBinding _flagTimerBinding = new StateBinding("FlagTimer");
        public bool IsFlagged;
        public int FlagTimer;
        public int tarDecalCount;

        private static readonly Dictionary<Duck, DrenchedInTar> _instances = new Dictionary<Duck, DrenchedInTar>();
        private int cachedDecalCount = 0;
        private readonly Duck _duck;
        private DuckBone duckBone;
        private readonly DuckMovementStats duckStats = new DuckMovementStats();

        private readonly SoundWrapper waterSound = new SoundWrapper();
        private readonly TarDecalProperties[] tarDecalData;
        private readonly SpriteMap[] tarStuff;
        private int particleTimer;
        private bool hasSetOffStarParticles;

        public class TarDecalProperties
        {
            public float x; public float y; public float scaleMul; public float rotDeg;
            public TarDecalProperties(float xPos, float yPos, float scaleMultiplier)
            {
                x = xPos;
                y = yPos;
                scaleMul = scaleMultiplier;
            }
        }
        public class DuckMovementStats
        {
            public MovementStatTracker accel; public MovementStatTracker run; public MovementStatTracker jump;
            public DuckMovementStats()
            {
                accel = new MovementStatTracker(0f, 0f);
                run = new MovementStatTracker(0f, 0f);
                jump = new MovementStatTracker(0f, 0f);
            }
        }
        public class MovementStatTracker
        {
            public float original; public float scaled; public float external; public float last;
            public MovementStatTracker(float originalValue, float scaledValue, float externalMultiplier = 1.0f, float lastSeenValue = 1f)
            {
                original = originalValue;
                scaled = scaledValue;
                external = externalMultiplier;
                last = lastSeenValue;
            }
        }
        private void SetDuckStat(ref MovementStatTracker stat, float originalValue, float multiplyBy)
        {
            stat.original = originalValue;
            stat.last = originalValue;
            stat.scaled = originalValue * multiplyBy;
        }

        private DrenchedInTar(Duck duck, int flagTimer, int tarDecalCount)
        {
            _duck = duck;
            FlagTimer = flagTimer;
            IsFlagged = true;
            cachedDecalCount = tarDecalCount;

            SetDuckStat(ref duckStats.accel, _duck.accelerationMultiplier, 0.5f); // vanilla is 1.0f, which becomes 0.5f
            SetDuckStat(ref duckStats.run, _duck.runMax, 0.65f);                  // vanilla is 3.1f, which becomes 2.015f
            SetDuckStat(ref duckStats.jump, _duck.jumpSpeed, 0.8f);               // vanilla is -4.9f, which becomes -3.92f

            tarStuff = new SpriteMap[4];
            tarDecalData = new TarDecalProperties[4];

            const float step = 16f / 4f;
            for (int i = 0; i < tarDecalData.Length; i++)
            {
                tarStuff[i] = new SpriteMap(GetPath("tarBlast"), 16, 16, true) { frame = 15 + i };

                // standing duck spans roughly (-12f, -12f) to (-4f, 4f)
                tarDecalData[i] = new TarDecalProperties(
                    Rando.Float(-12f, -4f),                              // X pos of decal
                    Rando.Float(-12f + step * i, -12f + step * (i + 1)), // Y pos of decal
                    Rando.Float(1f, 1.25f)                               // scale of decal
                );
            }
        }
        public static DrenchedInTar GetInstance(Duck duck, int FlagTimer = 600, int tarDecalCount = 3)
        {
            DrenchedInTar instance;
            if (!_instances.TryGetValue(duck, out instance))
            {
                var newInstance = new DrenchedInTar(duck, 300 + FlagTimer, tarDecalCount);
                _instances[duck] = newInstance;
                Level.Add(newInstance);
                duck.Scream();
                return newInstance;
            }
            if (instance.FlagTimer < 600)
            {
                DevConsole.Log("timer at " + instance.FlagTimer);
                instance.FlagTimer = Math.Min(600, instance.FlagTimer + FlagTimer);
                instance.hasSetOffStarParticles = false;
                instance.IsFlagged = true;
            }

            if (tarDecalCount > instance.cachedDecalCount)
            {
                instance.cachedDecalCount = tarDecalCount;
            }

            return instance;
        }

        public override void Terminate()
        {
            if (waterSound.IsPlaying)
            {
                waterSound.Stop();
                SFX.Play(GetPath("sizzleEnd"), 0.1f);
            }
            base.Terminate();
        }

        public override void Update()
        {
            if (_duck.dead || !IsFlagged)
            {
                Level.Remove(this);
                _instances.Remove(_duck);
                return;
            }

            // timer ticking logic; water sound logic
            if (_duck.doFloat)
            {
                FlagTimer -= 1 + (int)_duck.velocity.length;
                if (!waterSound.IsPlaying) waterSound.Play(GetPath("sizzleLoop"), 0.1f, looped: true);
            }
            else if (waterSound.IsPlaying)
            {
                waterSound.Stop();
                SFX.Play(GetPath("sizzleEnd"), 0.1f);
            }
            if (FlagTimer <= 300 && !_duck.doFloat) FlagTimer--;
            if (FlagTimer < 0) FlagTimer = 0;

            // external stat modification tracker
            if (duckStats.accel.last != _duck.accelerationMultiplier)
                duckStats.accel.external *= _duck.accelerationMultiplier / duckStats.accel.last;
            if (duckStats.run.last != _duck.runMax)
                duckStats.run.external *= _duck.runMax / duckStats.run.last;
            if (duckStats.jump.last != _duck.jumpSpeed)
                duckStats.jump.external *= _duck.jumpSpeed / duckStats.jump.last;


            // gradual stat restoration
            // timer values 0-300 are weighed at 75%, and 301-600 are weighed at 25%
            float timerInterpolation = (((300f - Math.Min(300, FlagTimer)) * 0.75f) + (300f - Math.Max(0, FlagTimer - 300f)) * 0.25f) / 300f;
            // lerp from scaled to original by a factor of timerInterpolation
            _duck.accelerationMultiplier =
                (duckStats.accel.scaled + (duckStats.accel.original - duckStats.accel.scaled) * timerInterpolation)
                * duckStats.accel.external;
            _duck.runMax =
                (duckStats.run.scaled + (duckStats.run.original - duckStats.run.scaled) * timerInterpolation)
                * duckStats.run.external;
            _duck.jumpSpeed =
                (duckStats.jump.scaled + (duckStats.jump.original - duckStats.jump.scaled) * timerInterpolation)
                * duckStats.jump.external;
            // cache stats for future external modification tracking
            duckStats.accel.last = _duck.accelerationMultiplier; duckStats.run.last = _duck.runMax; duckStats.jump.last = _duck.jumpSpeed;


            // occasional goop particles
            this.x = _duck.ragdoll != null ? _duck.ragdoll.x : _duck.x;
            this.y = (_duck.ragdoll != null ? _duck.ragdoll.y : _duck.y);

            float hSpeed = _duck.ragdoll != null ? Math.Abs(_duck.ragdoll.part2.velocity.x) : Math.Abs(_duck.hSpeed);
            float vSpeed = _duck.ragdoll != null ? Math.Abs(_duck.ragdoll.part2.velocity.y) : Math.Abs(_duck.vSpeed);

            if (this.isServerForObject && !_duck.doFloat && FlagTimer > 300)
            {
                particleTimer -= 1 + (int)(hSpeed * hSpeed + Math.Sqrt(vSpeed) * 2);

                if (particleTimer <= 0)
                {
                    Level.Add((Thing)TarParticle.New(this.x + Rando.Float(-3f, 3f), this.y + Rando.Float(-6f, 8f)));
                    particleTimer = (int)(90 * Rando.Float(0.75f, 1.5f));
                }
            }
            // spawn two white stars to let the player know they're fully clean
            if (this.isServerForObject && !hasSetOffStarParticles && FlagTimer <= 300)
            {
                hasSetOffStarParticles = true;
                for (int i = -1; i < 2; i += 2)
                {
                    Level.Add((Thing)new NewDizzyStar(_duck.x + i * 4f, _duck.y, new Vec2(i, -1f), new Color(196, 196, 196)));
                }
            }

            // disengaging
            if (FlagTimer == 0)
            {
                // snap stats back as a safemeasure 
                _duck.accelerationMultiplier = duckStats.accel.original * duckStats.accel.external;
                _duck.runMax = duckStats.run.original * duckStats.run.external;
                _duck.jumpSpeed = duckStats.jump.original * duckStats.jump.external;
                IsFlagged = false;
            }
        }
        public override void Draw()
        {
            // I don't know the exact details of how depth works in duck game, most of this is copied from armor/equipment
            if (this._duck._trapped != null)
                this.depth = this._duck._trapped.depth + 1;
            this.offDir = this._duck.offDir;
            this.depth = this._duck.depth + (this._duck.holdObject != null ? 5 : 12) + 1;

            // reduce alpha when the timer is below 300
            this.alpha = (float)(Math.Min(Math.Sqrt(300f), Math.Sqrt(FlagTimer)) / Math.Sqrt(300f));

            // render each of the tar decals
            for (int i = 0; i < cachedDecalCount; i++)
            {
                if (i == 0) duckBone = this._duck.skeleton.upperTorso;
                else duckBone = this._duck.skeleton.lowerTorso;

                this.angle = this.offDir > (sbyte)0 ? -duckBone.orientation : duckBone.orientation;
                // gradually lower the decals based on alpha
                this.position = duckBone.position + new Vec2(0f, 16f * (1f - this.alpha));

                this.tarStuff[i].flipH = this._duck._sprite.flipH;
                this.tarStuff[i].scale = new Vec2(tarDecalData[i].scaleMul);

                this.Draw((Sprite)this.tarStuff[i], tarDecalData[i].x / tarDecalData[i].scaleMul, tarDecalData[i].y / tarDecalData[i].scaleMul);
            }
        }
    }
}
