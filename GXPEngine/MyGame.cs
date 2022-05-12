using System;
using GXPEngine.BodyParts;
using GXPEngine.StageManagement;

namespace GXPEngine;

public class MyGame : Game
{
    public Player player;

    public static Vec2 partBaseSize = new(32,32);
    public static Vec2 playerBaseSize = new(32, 32);
    public static Vec2 initialPlayerPosition = new Vec2(0, 0);
    public static float globalGravity = 0.01f;
    public static float globalSpeed = 0.5f;
    public static float globalJumpForce = 1.5f;
    
    //From what x does the screen start scrolling
    private readonly int scrollX;
    
    
    private MyGame() : base(1200, 720, false)
    {
        scrollX = width / 2;
        
        targetFps = 60;

        StageLoader.LoadStage(Stages.Test);
    }

    private void Update()
    {
        if (Input.GetKeyDown(Key.C))
        {
            if (StageLoader.currentStage != null)
            {
                StageLoader.currentStage.background.Destroy();
                StageLoader.currentStage.backgroundSprites.Destroy();
                StageLoader.currentStage.breakableBlocks.Destroy();
                StageLoader.currentStage.spriteBatch.Destroy();
                StageLoader.currentStage.epilogueManager.isActive = true;
            }
        }
        
        
        if (StageLoader.currentStage is {comicActive: true}) return;

        Scroll();


        //Red
        if (Input.GetKeyDown(Key.NUMPAD_1) || Input.GetKeyDown(Key.Q))
            player.SetUpperBodyPart(new GrapplingHook(player));
        if (Input.GetKeyDown(Key.NUMPAD_2) || Input.GetKeyDown(49)) player.SetLowerBodyPart(new JumpingLegs(player));

        //Blue
        if (Input.GetKeyDown(Key.NUMPAD_3) || Input.GetKeyDown(Key.E)) player.SetUpperBodyPart(new StrongArm(player));
        if (Input.GetKeyDown(Key.NUMPAD_4) || Input.GetKeyDown(50)) player.SetLowerBodyPart(new ExtendyLegs(player));

        //Green
        // if (Input.GetKeyDown(Key.NUMPAD_5) || Input.GetKeyDown(53)) player.SetUpperBodyPart(new GreenUpperBodyPart(player));
        if (Input.GetKeyDown(Key.NUMPAD_6) || Input.GetKeyDown(51)) player.SetLowerBodyPart(new SpiderLegs(player));

        if (Input.GetKeyDown(Key.R))
        {
            player.SetUpperBodyPart(new GrapplingHook(player));
            player.SetLowerBodyPart(new JumpingLegs(player));
            player.SetXY(initialPlayerPosition.x, initialPlayerPosition.y);
        }


        void Scroll()
        {
            if (StageLoader.currentStage == null || player == null) return;

            //If the player is to the left of the center of the screen it will move to the left with the player until it hits the start of the stage
            if (player.x + StageLoader.currentStage.x < scrollX)
            {
                StageLoader.currentStage.x = scrollX - player.x;
            }
            else if (player.x + StageLoader.currentStage.x > width - scrollX)
            {
                StageLoader.currentStage.x = width - scrollX - player.x;
            }

            //If the player is to the right of the center of the screen it will move to the right with the player until it hits the end of the stage
            if (StageLoader.currentStage.x > 0)
            {
                StageLoader.currentStage.x = 0;
            }
            else if (StageLoader.currentStage.x < -StageLoader.currentStage.stageWidth + game.width)
            {
                StageLoader.currentStage.x = -StageLoader.currentStage.stageWidth + game.width;
            }
        }
    }

    public static Vec2 mousePos => StageLoader.currentStage != null ? new Vec2(Input.mouseX - StageLoader.currentStage.x, Input.mouseY) : new Vec2(Input.mouseX, Input.mouseY);

    private static void Main()
    {
        new MyGame().Start();
    }
}