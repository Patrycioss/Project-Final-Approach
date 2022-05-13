using System;
using GXPEngine.StageManagement;

namespace GXPEngine.BodyParts;

public class LowerBodyPart : BodyPart
{
    protected float speedMultiplier;
    protected float speed;
    protected float jumpMultiplier;

    public bool disableKeyAndGravityMovement;

    private AnimationSprite animationSprite;

    protected Sound walkSound;
    protected SoundChannel channel;
    
    protected LowerBodyPart(string modelPath, int cols, int rows, int frames, Player player_) : base(modelPath, cols, rows, frames, player_)
    {
        disableKeyAndGravityMovement = false;
        model.SetXY(0,-32);

        channel = new SoundChannel(15);
        channel.Mute = true;
        walkSound?.Play(channelId: 15);

    }

    protected override void Update()
    {

        if (StageLoader.currentStage is {comicActive:true})
        {
            disableKeyAndGravityMovement = true;
        }
        else if (this is SpiderLegs spiderLegs)
        {
            if (spiderLegs.inSpiderForm)
            {
                disableKeyAndGravityMovement = true;
            }
            else disableKeyAndGravityMovement = false;
        }
        else disableKeyAndGravityMovement = false;
    }

    public virtual void HandleMovement()
    {
        if (player.horizontalCollision != null) player.lastHorizontalCollision = player.horizontalCollision;
        if (player.verticalCollision != null) player.lastVerticalCollision = player.verticalCollision;
        
        player.horizontalCollision = player.MoveUntilCollision(player.velocity.x * Time.deltaTime, 0, StageLoader.currentStage?.surfaces.GetChildren()!);
        player.verticalCollision = player.MoveUntilCollision(0, player.velocity.y * Time.deltaTime, StageLoader.currentStage!.surfaces.GetChildren());

        if (!disableKeyAndGravityMovement)
        {
            switch (player.currentState)
            {
                case Player.State.Stand:
                    StandState();
                    channel.Mute = true;
                    break;

                case Player.State.Walk:
                    WalkState();
                    channel.Mute = false;
                    break;

                case Player.State.Jump:
                    JumpState();
                    channel.Mute = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
    }
    
    protected virtual void JumpState()
    {
        player.velocity.y += MyGame.globalGravity * Time.deltaTime;
        CheckIfGrounded();


        if (Input.GetKey(Key.D) && Input.GetKey(Key.A))
        {
            player.velocity.x = 0;
        } 
        if (Input.GetKey(Key.D))
        {
            player.velocity.x = speed;
            player.mirrored = false;
        }
        else if (Input.GetKey(Key.A))
        {
            player.velocity.x = -speed;
            player.mirrored = true;
        }

        if (player.isGrounded && Input.GetKey(Key.A) != Input.GetKey(Key.D))
        {
            player.velocity.y = 0;
            player.currentState = Player.State.Walk;
        }
        else if (player.isGrounded)
        {
            player.velocity.y = 0;
            player.currentState = Player.State.Stand;
        }
        else if (player.verticalCollision is {normal.y: > 0.5f}) player.velocity.y = 0;
    }

    protected virtual void StandState()
    {
        player.velocity.SetXY(0, 0);

        if (!player.isGrounded)
        {
            player.currentState = Player.State.Jump;
            return;
        }

        if (Input.GetKey(Key.A) != Input.GetKey(Key.D))
        {
            player.currentState = Player.State.Walk;
        }
        else if (Input.GetKeyDown(Key.SPACE) || Input.GetKeyDown(Key.W))
        {
            Jump();
            player.currentState = Player.State.Jump;
        }
    }

    protected virtual void WalkState()
    {
        CheckIfGrounded();

        if (Input.GetKey(Key.A) == Input.GetKey(Key.D))
        {
            player.currentState = Player.State.Stand;
            return;
        }
        if (Input.GetKey(Key.A))
        {
            // if (target.horizontalCollision != null) DoSlopedMovementIfPossible();
            // else target.velocity.x = -speed;
            player.velocity.x = -speed;
            player.mirrored = true;
        }
        if (Input.GetKey(Key.D))
        {
            // if (target.horizontalCollision != null) DoSlopedMovementIfPossible();
            // else target.velocity.x = speed;
            player.velocity.x = speed;
            player.mirrored = false;
        }

        if (Input.GetKey(Key.SPACE) || Input.GetKey(Key.W))
        {
            Jump();
            player.currentState = Player.State.Jump;
        }
        else if (!player.isGrounded && !player.wasGrounded)
        {
            player.currentState = Player.State.Jump;
        }
    }

    /// <summary>
    /// Might as well jump
    /// </summary>
    protected virtual void Jump()
    {
        player.velocity.y -= MyGame.globalJumpForce * jumpMultiplier;
    }
    
    /// <summary>
    /// Checks if the player is grounded based on if the collision normal y has a value lower than -0.5f to ensure jumping works on 45 deg slopes
    /// </summary>
    public virtual void CheckIfGrounded()
    {
        if (player.verticalCollision != null)
        {
            // Console.WriteLine($"Vertical Normal {target.verticalCollision.normal}");
            player.isGrounded = player.verticalCollision.normal.y < -0.5f;
        }
        else player.isGrounded = false;
    }

    public override void UpdatePosition()
    {
        SetXY(player.x,player.y + 16);
    }
    
} 
