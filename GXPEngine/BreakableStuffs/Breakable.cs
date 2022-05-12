using System;
using GXPEngine.StageManagement;
using GXPEngine.Visual;

namespace GXPEngine.BreakableStuffs;

public class Breakable : Sprite
{
	private Hitbox hitbox;

	protected (string path, int cols, int rows, int frames, float animationDelay) breakAnimationInfo;


	private Sound breakSound;
	
	public Breakable(int pX, int pY, string filePath = "placeholders/colors.png") : base(filePath)
	{
		SetXY(pX,pY);

		breakAnimationInfo = ("", -1, -1, -1, -1);

		breakSound = new Sound("sounds/break.wav");
		
		// Console.WriteLine($"{x}, {y}");
		hitbox = new Hitbox();		
		hitbox.SetXY(x,y);
		hitbox.width = width;
		hitbox.height = height;
		StageLoader.currentStage?.surfaces.AddChild(hitbox);

		
		collider.isTrigger = true;
		
		
		
	}

	public void Break()
	{
		breakSound.Play(volume:0.9f);
		
		MakeAnimation();
		
		StageLoader.currentStage?.breakableBlocks.RemoveChild(hitbox);
		hitbox.Destroy();
		LateDestroy();
	}

	protected virtual void MakeAnimation()
	{
		Animation animation = new ("placeholders/placeHolders.png", 4, 1, 4, true, 255);
		animation.SetXY(x,y);
		StageLoader.currentStage?.animations.AddChild(animation);
	}
}