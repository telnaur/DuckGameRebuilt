using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.UFFMod
{
    [BaggedProperty("canSpawn", false)]
    public class Balloon : PhysicsObject, IMountable
    {
        public StateBinding _destroyedStateBinding = new StateBinding("_destroyed");
        public StateBinding _localDestroyedStateBinding = new StateBinding("_localDestroyed");
        public StateBinding _ropeStateBinding = new StateBinding("_rope");
        public StateBinding _beginAttachedStateBinding = new StateBinding("_beginAttached");
        public StateBinding _triedAttachStateBinding = new StateBinding("_triedAttach");
        public StateBinding _animationIndexStateBinding = new StateBinding("netAnimationIndex");
        public StateBinding _frameStateBinding = new StateBinding("spriteFrame");

        public BalloonRope _rope;
        public bool _localDestroyed;
        public bool _beginAttached;
        public bool _triedAttach;

        public Type _attachedType;
        private SpriteMap sprite;
        private Vec2 ropeOffsetTL;

        public Vec2 ropeOffset
        {
            get
            {
                return ropeOffsetTL - center;
            }
        }

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

        public Balloon(float xpos, float ypos, float spe = 0f, float ang = 0f, float launchAngle = 0f, Type attachedType = null) :
            base(xpos, ypos)
        {
            // collision & sprite settings
            _canFlip = false;
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\balloon"), 15, 15);
            graphic = sprite;
            center = new Vec2(7f, 7f);
            collisionSize = new Vec2(8f, 10f);
            collisionOffset = new Vec2(-4f, -5f);
            ropeOffsetTL = new Vec2(7f, 14f);
            thickness = 0.6f;
            depth = -0.5f;
            hSpeed = spe * (float)Math.Cos(ang);
            vSpeed = spe * (float)Math.Sin(ang);
            angle = launchAngle;
            _attachedType = attachedType;
        }

        public override void Terminate()
        {
            if (_rope != null)
                Level.Remove(_rope);

            base.Terminate();
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            _localDestroyed = true;
            SFX.Play(Mod.GetPath<UffMod>("SFX\\balloonpop"));
            Level.Add(new BalloonPop(x, y));
            Level.Remove(_rope);
            Level.Remove(this);
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);
            Destroy(new DTShot(bullet));
            return base.Hit(bullet, hitPos);
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            Destroy();
        }

        public override void Update()
        {
            if (_attachedType != null && !_beginAttached)
                _beginAttached = true;

            if (_rope == null && !(Level.current is Editor))
            {
                _rope = new BalloonRope(Offset(ropeOffset).x, Offset(ropeOffset).y, this, _attachedType);
                if (isServerForObject)
                    Level.Add(_rope);
            }

            if (_localDestroyed && !_destroyed)
            {
                Destroy(new DTImpact(this));
                return;
            }

            if (y < Level.current.topLeft.y - 264f)
            {
                if (_rope != null && _rope._attachedObject != null)
                {
                    _rope._attachedObject.Destroy(new DTImpact(this));
                    Level.Remove(_rope._attachedObject);
                }
                Level.Remove(this);
                return;
            }
            
            float effectiveWeight = weight;
            if (_rope != null && _rope._attachedObject != null)
                effectiveWeight += _rope._attachedObject.weight;
            float riseSpeed = -2f;
            float lerpAmount = (0.16f / (effectiveWeight > 8f ? effectiveWeight / 8f : 1f)) > 0.07f ? (0.16f / (effectiveWeight > 8f ? effectiveWeight / 8f : 1f)) : 0.07f;
            if (vSpeed > riseSpeed)
                vSpeed = MathHelper.Lerp(vSpeed, riseSpeed, lerpAmount);

            angle = MathHelper.Lerp(angle, Maths.DegToRad(Math.Abs(hSpeed * 15f) > 90f ? (hSpeed > 0 ? 90f : -90f) : hSpeed * 15f), 0.09f);

            if (Level.CheckRect<Block>(topLeft - new Vec2(0f, 1f), topRight) != null || Level.CheckRect<Block>(bottomLeft, bottomRight + new Vec2(0f, 1f)) != null)
                Destroy();

            base.Update();

            if (_rope != null)
            {
                _rope.position = Offset(ropeOffset);

                if (_rope._nodesInitialized)
                {
                    PhysicsObject physicsObject = Level.CheckRect<PhysicsObject>(bottomLeft + new Vec2(0f, 1f), bottomRight + new Vec2(0f, 16f), this);
                    if (isServerForObject && !_triedAttach && !_beginAttached && !_rope._hasAttached && physicsObject != null && !(physicsObject is Balloon) && !(physicsObject is Duck))
                        Attach(physicsObject, true);
                    _triedAttach = true;

                    // duck grabs onto rope; doesn't work online
                    /*
                    if (_rope._attachedObject == null)
                    {
                        Duck theDuck = null;
                        float length = 999f;
                        foreach (Duck d in Level.CheckCircleAll<Duck>(_rope.nodes[11].position, 12f))
                            if (d.isServerForObject && d.inputProfile.Pressed("GRAB") && d._timeSinceThrow >= 30 && GetMountDistance(d) < length && Level.CheckCircle<Holdable>(new Vec2(d.x, d.y + 4f), 18f) == null)
                            {
                                bool check = true;
                                foreach (PhysicsObject mountable in Level.CheckCircleAll<PhysicsObject>(d.position, 32f))
                                    if (mountable != this && mountable is IMountable && ((IMountable)mountable).GetMountDistance(d) <= GetMountDistance(d))
                                        check = false;
                                if (check)
                                {
                                    theDuck = d;
                                    length = (d.position - _rope.nodes[11].position).length;
                                }
                            }
                        if (theDuck != null)
                        {
                            theDuck._timeSinceThrow = 0;
                            Attach(theDuck);
                        }
                    }
                    */
                }
            }
        }

        private void Attach(PhysicsObject physicsObject, bool first = false)
        {
            if (first)
            {
                physicsObject.x = x;
                y = physicsObject.top - 18f;
            }
            physicsObject.grounded = false;
            physicsObject.enablePhysics = false;
            _rope._attachedObject = physicsObject;
            _rope._hasAttached = true;
        }

        public float GetMountDistance(Duck d)
        {
            if (_rope != null && _rope.nodes.Count == 12)
                return (d.position - _rope.nodes[11].position).length;
            else
                return 99999f;
        }
    }

    public class BalloonRope : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _angleStateBinding = new CompressedFloatBinding("_angle");
        public StateBinding _balloonStateBinding = new StateBinding("_balloon");
        public StateBinding _attachedObjectStateBinding = new StateBinding("_attachedObject");
        public StateBinding _hasAttachedStateBinding = new StateBinding("_hasAttached");
        public StateBinding _nodesInitializedStateBinding = new StateBinding("_nodesInitialized");
        public StateBinding _travelTimeStateBinding = new StateBinding("_travelTime");

        public Balloon _balloon;
        public PhysicsObject _attachedObject;
        public bool _hasAttached;
        public bool _nodesInitialized;
        public int _travelTime;

        public Type _attachedType;

        internal IList<BalloonNode> nodes = new List<BalloonNode>();

        public BalloonRope(float xpos, float ypos, Balloon balloon, Type attachedType = null) :
            base(xpos, ypos)
        {
            _balloon = balloon;
            _attachedType = attachedType;
        }

        public override void Terminate()
        {
            if (_attachedObject != null)
            {
                Fondle(_attachedObject);
                _attachedObject.sleeping = false;
                _attachedObject.gravMultiplier = 1f;
                _attachedObject.enablePhysics = true;
                _attachedObject.angle = 0f;
                _attachedObject.vSpeed = -0.5f;
                _attachedObject = null;
            }

            foreach (BalloonNode node in nodes)
                Level.Remove(node);

            base.Terminate();
        }

        public override void Update()
        {
            if (_balloon != null)
            {
                angle = _balloon.angle;

                if (nodes.Count == 0)
                {
                    BalloonNode bn;
                    for (int i = 0; i < 12; i++)
                    {
                        bn = new BalloonNode(x - i * (float)Math.Sin(angle), y + i * (float)Math.Cos(angle), i == 11, this);
                        bn.angle = angle;
                        if (isServerForObject)
                            Level.Add(bn);
                        nodes.Add(bn);
                    }
                    _nodesInitialized = true;
                }
            }

            base.Update();

            if (nodes.Count > 0)
            {
                if (isServerForObject)
                {
                    nodes[0].angle = angle;
                    nodes[0].position = position;

                    for (int i = 1; i < 12; i++)
                    {
                        nodes[i].x = MathHelper.Lerp(nodes[i].x, nodes[i - 1].x, 0.03f);
                        nodes[i].y = MathHelper.Lerp(nodes[i].y, nodes[i - 1].y, 0.03f);

                        Vec2 positionDiffNormalized = (nodes[i].position - nodes[i - 1].position).normalized;
                        nodes[i].position = nodes[i - 1].position + positionDiffNormalized;
                        nodes[i].angle = (float)(Math.Atan2(nodes[i].y - nodes[i - 1].y, nodes[i].x - nodes[i - 1].x) - Math.PI / 2);
                    }
                }

                if (_attachedObject == null && _attachedType != null && !_hasAttached)
                {
                    PhysicsObject p = Activator.CreateInstance(_attachedType, Editor.GetConstructorParameters(_attachedType)) as PhysicsObject;
                    p.enablePhysics = false;
                    p.grounded = false;
                    p.position = nodes[11].position + new Vec2((float)Math.Sin(nodes[11].angle) * (p.collisionOffset.y + 1f), (float)-Math.Cos(nodes[11].angle) * (p.collisionOffset.y + 1f));
                    if (p is AerialMine)
                        ((AerialMine)p)._balloon = _balloon;
                    if (isServerForObject)
                        Level.Add(p);
                    _attachedObject = p;
                    _hasAttached = true;
                }

                if (_attachedObject != null)
                {
                    // Duck d = _attachedObject as Duck;
                    if (_attachedObject.owner == null
                        && (Level.CheckRect<Block>(_attachedObject.topLeft + new Vec2(1f, 1f), _attachedObject.bottomRight - new Vec2(1f, 1f)) == null
                        || _travelTime < 15)
                        /* && !(d != null
                        && d._timeSinceThrow >= 30
                        && (d.inputProfile.Down("LEFT")
                        || d.inputProfile.Down("RIGHT")
                        || d.inputProfile.Down("UP")
                        || d.inputProfile.Down("DOWN")
                        || d.inputProfile.Down("JUMP")
                        || d.inputProfile.Down("GRAB")
                        || d.inputProfile.Down("RAGDOLL"))) */)
                    {
                        _attachedObject.gravMultiplier = 0f;
                        _attachedObject.enablePhysics = false;
                        _attachedObject.grounded = false;
                        _attachedObject.position = nodes[11].position + new Vec2((float)Math.Sin(nodes[11].angle) * (_attachedObject.collisionOffset.y + 1f), (float)-Math.Cos(nodes[11].angle) * (_attachedObject.collisionOffset.y + 1f));
                        _attachedObject.angle = (float)(Math.Atan2(_attachedObject.y - nodes[11].y, _attachedObject.x - nodes[11].x) - (float)Math.PI / 2f);
                    }
                    else
                    {
                        if (_attachedObject.owner == null)
                        {
                            Fondle(_attachedObject);
                            _attachedObject.sleeping = false;
                            _attachedObject.gravMultiplier = 1f;
                            _attachedObject.enablePhysics = true;
                            _attachedObject.angle = 0f;
                            _attachedObject.vSpeed = -0.5f;
                        }
                        /* if(d != null)
                            d._timeSinceThrow = 0; */
                        _attachedObject = null;
                    }
                }
            }

            if (_balloon == null || _balloon._localDestroyed || _balloon.destroyed)
                Level.Remove(this);

            if (_travelTime < 15)
                _travelTime++;
        }
    }

    internal class BalloonNode : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _angleStateBinding = new CompressedFloatBinding("_angle");

        private SpriteMap sprite;

        public BalloonNode(float xpos, float ypos, bool end, BalloonRope own)
            : base(xpos, ypos)
        {
            if (end)
            {
                sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\balloonRopeEnd"), 1, 1);
                center = new Vec2(0f, 0f);
            }
            else
            {
                sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\balloonRopePart"), 3, 1);
                center = new Vec2(1f, 0f);
            }
            graphic = sprite;
            owner = own;
            depth = -0.6f;
        }
    }
}