//In this file are all classes for the different body parts

using System;
using System.Drawing;
using GXPEngine.BreakableStuffs;
using GXPEngine.Core;
using GXPEngine.StageManagement;

namespace GXPEngine.BodyParts
{
    
    
    //Red
    public class JumpingLegs : LowerBodyPart {
        public JumpingLegs(Player player_) : base("bodyParts/jumper-small.png", 10, 2, 19, player_)
        {
            model.SetXY(0,-16);
            model.SetCycle(0,1);
            
            jumpMultiplier = 1;
            speedMultiplier = 1;

            speed = speedMultiplier * MyGame.globalSpeed;
        }

        protected override void Update()
        {
            base.Update();
            model.Mirror(!player.mirrored,false);
            model.Animate(Time.deltaTime);

            if (Input.GetKey(Key.D) || Input.GetKey(Key.A))
            {
                model.SetCycle(0, 19, 15);
            }
            else model.SetCycle(5,1);

        }
    }

    public class GrapplingHook : UpperBodyPart
    {
        private readonly float grapplePower;
        public Hook? hook;
        public bool pulling;
        private float pullPower;
        private readonly float pullPowerIncrement;
        private readonly float maxPullPower;

        private Sound shootingSound;
        

        public GrapplingHook(Player player_) : base("bodyParts/monkey-small.png", 1, 1, 1, player_)
        {
            model.SetXY(0,-18);
            SetAbilityModel("bodyParts/monkey_arm-small.png",1,1,1, addCollider_:true);
            abilityModel.SetOrigin(1,15);
            abilityModel.x = 15;
            abilityModel.y = 5;

            shootingSound = new Sound("sounds/shoot.wav");
            
            
            
            grapplePower = 1.5f;
            pullPower = 0.05f;

            pullPowerIncrement = 0.01f;
            maxPullPower = 1;
            
            
            
            hook = null;
            pulling = false;
        }

        protected override void UseAbility()
        {
            if (StageLoader.currentStage is {comicActive: true}) return;
            if (abilityModel == null) return;

            if (hook != null && !pulling && hook.hasHit)
            {
                pulling = true;
                if (player.lowerBodyPart != null) player.lowerBodyPart.disableKeyAndGravityMovement = true;
            }
            else if (hook == null)
            {
                shootingSound.Play(volume: 0.1f);
                pulling = false;

                Vec2 newPosition = new Vec2(player.x + abilityModel.x,player.y + abilityModel.y);
                
              
                hook = new Hook(newPosition.x,newPosition.y, abilityDirection.SetMagnitude(grapplePower * MyGame.globalSpeed))
                {
                    rotation = abilityModel.rotation,
                };
            }
        }

        protected override void CancelAbility()
        {
            pulling = false;
            hook?.LateDestroy();
            hook = null;
            player.lowerBodyPart?.CheckIfGrounded();

            if (!player.isGrounded) player.currentState = Player.State.Jump;
            
            player.lowerBodyPart!.disableKeyAndGravityMovement = false;
            
            StageLoader.currentStage?.drawing.ClearTransparent();
        }

        protected override void Update()
        {
            abilityModel?.Animate(Time.deltaTime);
            // abilityModel.Mirror(abilityModel.rotation > 0,false);

            
            model.Mirror(!player.mirrored,false);
            model.Animate(Time.deltaTime);

            if (abilityModel != null)
            {
                abilityDirection = MyGame.mousePos - new Vec2(x + MyGame.playerBaseSize.x / 2, y + MyGame.partBaseSize.y);
                RotateToMouse();

          
            
                if (Input.GetMouseButtonDown(0))
                {
                    UseAbility();
                    abilityUsable = false;
                    timeOfUsage = Time.now;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    CancelAbility();
                }
                else if (hook == null)
                {
                    RotateToMouse();
                }
            }
            
            if (hook != null)
            {
                Vec2 direction = new Vec2(hook.x,hook.y) - new Vec2(player.x + MyGame.partBaseSize.x / 2.0f,
                    player.y + MyGame.partBaseSize.y);

                // Vec2 direction = hook.velocity;
                
                
                
                
                StageLoader.currentStage?.drawing.ClearTransparent();
                StageLoader.currentStage?.drawing.Stroke(Color.Brown);
                
                if (abilityModel != null)
                {

                    StageLoader.currentStage?.drawing.Line(x + abilityModel.x, y + abilityModel.y, hook.x, hook.y);
                    
                    if (!hook.HitTest(player) && !hook.HitTest(this))
                    {
                        abilityModel.rotation = direction.GetAngleDegrees();
                    }
                    
                }


                if (!hook.hasHit)
                {
                    if (direction.Magnitude() > 450) CancelAbility();
                }


                if (pulling)
                {
                    if (direction.Magnitude() < 20 || direction.y > 0)
                    {
                        CancelAbility();
                        return;
                    }

                    player.velocity = direction.SetMagnitude(pullPower);

                    if (pullPower < maxPullPower)
                    {
                        pullPower += pullPowerIncrement * Time.deltaTime;
                    }

                    if (player.horizontalCollision != null || player.verticalCollision != null)
                    {
                        CancelAbility();
                    }
                }
            }
        }


        protected override void RotateToMouse()
        {
            if (hook == null) base.RotateToMouse();
        }
    }

    //Blue
    public class ExtendyLegs : LowerBodyPart
    {
        private readonly int extendSpeed;
        private readonly float crouchIntensity;
        private readonly int maxExtendiness;

        private AnimationSprite tracks;

        private float extendedPart;

        private Sound extending;
        private float playedSound;
        private SoundChannel soundChannel;
        private IntPtr soundId;

        private SoloudSoundSystem soundSystem;

        public ExtendyLegs(Player player_) : base("bodyParts/extend_part-small.png", 1, 1,1, player_)
        {
            tracks = new AnimationSprite("bodyParts/extender-small.png", 10, 2, 19);
            AddChild(tracks);

            extending = new Sound("sounds/extendo.wav", looping: false);


            soundChannel = new SoundChannel(30);
        
            
            
            playedSound = 0;
            extendedPart = 0;
            
            Console.WriteLine(model.height);
            
            // model.SetScaleXY(0.5f);
            model.SetXY(0,-16);
            tracks.SetXY(0,model.y);
            
            tracks.x = (player_.mirrored ? -2 : 2);

            model.x = (player_.mirrored ? -2 : 2);
            
            jumpMultiplier = 0;
            speedMultiplier = 1;
            speed = MyGame.globalSpeed * speedMultiplier;
            extendSpeed = 1;
            crouchIntensity = 0.02f;

            maxExtendiness = (int) MyGame.playerBaseSize.y * 6;
        }

        public override void HandleMovement()
        {
            base.HandleMovement();

            model.Mirror(!player.mirrored,false);
            tracks.Mirror(!player.mirrored,false);
            
            model.x = (player.mirrored ? -2 : 2);
            tracks.x = (player.mirrored ? -2 : 2);

            Collision? boundaryCollision;

            Console.WriteLine(player.height);

            if (Input.GetKey(Key.A) || Input.GetKey(Key.D))
            {
                tracks.SetCycle(0, 19,20);
            }

            if (Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.S))
            {
                extending.Play(volume: 0.1f, channelId: 30);
            }

            
            
            
            
            if (Input.GetKey(Key.W) && player.height < maxExtendiness)
            {
                // if (!soundChannel.IsPlaying)
                // {
                //     extending.Play(volume: 0.1f, channelId:30);
                //     playedSound = Time.now;
                // }
                

                
                boundaryCollision = player.MoveUntilCollision(0, -extendSpeed, StageLoader.currentStage?.surfaces.GetChildren()!);
                if (boundaryCollision == null)
                {
                    player.height += extendSpeed;
                    model.height += extendSpeed;
                    
                    tracks.y += extendSpeed;
                    extendedPart += extendSpeed;
                }
            }
            else if (Input.GetKey(Key.S))
            {

                boundaryCollision = player.MoveUntilCollision(0,extendSpeed, StageLoader.currentStage?.surfaces.GetChildren()!);
                if (boundaryCollision is {normal: {y: < -0.5f}} && player.height > 21)
                {
                    player.height -= extendSpeed;
                    model.height -= extendSpeed;

                    // if (player.height < 32) model.visible = false;
                    // else model.visible = true;

                    if (extendedPart >= extendSpeed)
                    {
                        tracks.y -= extendSpeed;
                        extendedPart -= extendSpeed;
                    }
                    else
                    {
                        tracks.y -= extendSpeed;
                    }
                }
            }
            else soundChannel.Stop();
        }
    }

    public class StrongArm : UpperBodyPart
    {
        public StrongArm(Player player_) : base("bodyParts/puncher.png", 5, 8, 40, player_)
        {
            model.SetScaleXY(0.5f);
            model.SetXY(0,-16);
            model.Mirror(!player.mirrored,false);
            SetAbilityModel("bodyParts/test/blue/ability.png",1,1,1, true,player.mirrored?180:360);
            model.SetCycle(0, 1, 30);

            abilityModel.x = player.mirrored ? 20 : -20;
            abilityModel.visible = false;
        }
        protected override void UseAbility()
        {
            model.SetCycle(0, 40, 30);
        }

        protected override void Update()
        {
            base.Update();
            
            model.Animate(Time.deltaTime);
            
            model.Mirror(!player.mirrored,false);

            if (player.mirrored)
            {
                model.x = -26;
                abilityModel.x = 0;
                abilityModel.y = 10;

            }
            else
            {
                model.x = -7;
                abilityModel.x = 20;
                abilityModel.y = -10;
            }


            Console.WriteLine(model.currentFrame);
            
            if (model.currentFrame == 39)
            {
                foreach (Breakable breakableBlock in StageLoader.currentStage.breakableBlocks.GetChildren())
                {
                    if (abilityModel.HitTest(breakableBlock)) breakableBlock.Break(); 
                }
                model.SetCycle(0,1);
            }


            
            // abilityModel.visible = false;
            abilityModel.rotation = player.mirrored ? 180 : 360;
        }
    }

    //Green
    public class SpiderLegs : LowerBodyPart
    {
        public bool inSpiderForm;
        private float climbSpeed;

        private bool wasInSpiderForm;

        public SpiderLegs(Player player_) : base("bodyParts/spider-small.png", 10, 4, 39, player_)
        {
            model.SetCycle(0,1,20);
            model.SetXY(0,-16);
            jumpMultiplier = 0;
            inSpiderForm = false;
            speedMultiplier = 1;

            speed = MyGame.globalSpeed * speedMultiplier;

            const float climbMultiplier = 0.3f;
            climbSpeed = climbMultiplier * speed;

        }

        protected override void Update()
        {
            base.Update();
            
            model.Mirror(!player.mirrored,false);
            model.Animate(Time.deltaTime);
        }

        public override void HandleMovement()
        {

            inSpiderForm = false;

            foreach (Hitbox hitbox in StageLoader.currentStage.climbableSurfaces.GetChildren())
            {
                if (hitbox.climbable)
                {
                    if (player.HitTest(hitbox))
                    {
                        inSpiderForm = true;
                        if (player.upperBodyPart is GrapplingHook grapplingHook)
                        {
                            grapplingHook.hook?.Kill();
                        }
                    }
                }
            }

            if (Input.GetKey(Key.W) || Input.GetKey(Key.A) || Input.GetKey(Key.D) || Input.GetKey(Key.S))
            {
                model.SetCycle(inSpiderForm ? 0 : 20, 19, 20);
            }
            else model.SetCycle(inSpiderForm? 0 : 20,1);

            // if (inSpiderForm == false && climbKeyPressed)
            // {
            //     climbKeyPressed = false;
            //     CheckIfGrounded();
            // }
            
            // if (!climbKeyPressed) base.HandleMovement();

            if (!inSpiderForm) base.HandleMovement();

            // if (inSpiderForm && climbKeyPressed)
            if (inSpiderForm)
            {
                
                
                if (Input.GetKey(Key.W))
                {
                    player.MoveUntilCollision(0, -climbSpeed * Time.deltaTime, StageLoader.currentStage.surfaces.GetChildren());
                }

                if (Input.GetKey(Key.A))
                {
                    player.MoveUntilCollision(-climbSpeed * Time.deltaTime, 0, StageLoader.currentStage.surfaces.GetChildren());
                    player.mirrored = true;
                }

                if (Input.GetKey(Key.D))
                {
                    player.MoveUntilCollision(climbSpeed * Time.deltaTime, 0, StageLoader.currentStage.surfaces.GetChildren());
                    player.mirrored = false;
                }

                if (Input.GetKey(Key.S))
                {
                    player.MoveUntilCollision(0, climbSpeed * Time.deltaTime, StageLoader.currentStage.surfaces.GetChildren());
                }
            }
            
        }
    }

    public class GreenUpperBodyPart : UpperBodyPart
    {
        public GreenUpperBodyPart(Player player_) : base("bodyParts/test/green/upper.png", 1, 1, 1, player_)
        {
            SetAbilityModel("bodyParts/test/green/ability.png",1,1,1);
            
            
            
            
        }
        
        protected override void UseAbility()
        {
        }
    }
    
    //PlaceHolder
    public class PlaceHolderLowerBodyPart : LowerBodyPart {public PlaceHolderLowerBodyPart(Player player_) : base("bodyParts/placeHolder/lower.png", 1, 1, 1, player_){}}

    public class PlaceHolderUpperBodyPart : UpperBodyPart
    {
        public PlaceHolderUpperBodyPart(Player player_) : base("bodyParts/placeHolder/upper.png", 1, 1, 1, player_){}
        protected override void UseAbility()
        {
        }
    }
}
