using System;
using System.Drawing;
using GXPEngine.BodyParts;
using GXPEngine.Core;
using GXPEngine.StageManagement;

namespace GXPEngine;

public class Player : Sprite
{
	public Vec2 velocity;
	public State currentState;
	public bool isGrounded;
	public bool wasGrounded;

	public Collision? horizontalCollision;
	public Collision? verticalCollision;
	public Collision? lastHorizontalCollision;
	public Collision? lastVerticalCollision;

	public LowerBodyPart? lowerBodyPart;
	public UpperBodyPart? upperBodyPart;

	public bool mirrored;


	public Vec2 position => new Vec2(x, y);
	public Vec2 centerPosition => new Vec2(x + width / 2.0f, y + height / 2.0f);
	
	public Player(float pX, float pY) : base("hitboxes/player.png")
	{
		SetXY(pX,pY);

		velocity = new Vec2(0, 0);

		horizontalCollision = null;
		verticalCollision = null;
		lastHorizontalCollision = null;
		lastVerticalCollision = null;

		lowerBodyPart = null;
		upperBodyPart = null;

		SetUpperBodyPart(new GrapplingHook(this));
		SetLowerBodyPart(new JumpingLegs(this));

		mirrored = false;
	}

	public void SetUpperBodyPart(UpperBodyPart? newBodyPart)
	{
		// if (upperBodyPart?.model.height > MyGame.partBaseSize.y)
		// {
		// 	int temp = upperBodyPart.model.height - (int) MyGame.partBaseSize.y;
		// 	height -= temp;
		// 	if (lowerBodyPart != null) lowerBodyPart.y -= temp;
		// }
		
		if (upperBodyPart is GrapplingHook grapplingHook)
		{
			grapplingHook.hook?.Kill();
			StageLoader.currentStage?.drawing.ClearTransparent();
		}


		upperBodyPart?.Destroy();
		upperBodyPart = newBodyPart;
		upperBodyPart?.SetXY(x, y);
	}

	public void SetLowerBodyPart(LowerBodyPart? newBodyPart)
	{
		// if (lowerBodyPart?.model.height > MyGame.partBaseSize.y || lowerBodyPart?.model.height < MyGame.partBaseSize.y)
		// {
		// 	int temp = lowerBodyPart.model.height - (int)MyGame.partBaseSize.y;
		// 	height -= temp;
		// 	if (lowerBodyPart != null) y += temp;
		// }

		Console.WriteLine(lowerBodyPart?.model.height);

		if (lowerBodyPart is ExtendyLegs && lowerBodyPart.model.height != 32)
		{
			int temp = height - 32;
			height -= temp;
			if (lowerBodyPart != null) y += temp;
		}


		lowerBodyPart?.Destroy();
		lowerBodyPart = newBodyPart;
		lowerBodyPart?.SetXY(x,y + 32);
		
		
		if (lowerBodyPart != null && lastVerticalCollision != null && HitTest(lastVerticalCollision.other)) y -= 32;
		
		lowerBodyPart?.CheckIfGrounded();
	}
	

	private void Update()
	{
		if (StageLoader.currentStage is {comicActive: true})
		{
			visible = false;
			if (lowerBodyPart != null) lowerBodyPart.visible = false;
			if (upperBodyPart != null) upperBodyPart.visible = false;
			return;
		}
		else
		{
			visible = true;
			if (lowerBodyPart != null) lowerBodyPart.visible = true;
			if (upperBodyPart != null) upperBodyPart.visible = true;
		}
		
		
		wasGrounded = isGrounded;
		
		lowerBodyPart?.HandleMovement();
		
		lowerBodyPart?.UpdatePosition();

		upperBodyPart?.UpdatePosition();

		if (lowerBodyPart.Index < 0)
		{
			StageLoader.currentStage.SetChildIndex(lowerBodyPart,0);
		}
		

		if (lowerBodyPart != null && upperBodyPart != null)
		{
			StageLoader.currentStage.SetChildIndex(upperBodyPart,lowerBodyPart.Index + 1);
		}
	}

	/// <summary>
	/// The player has three states: Stand, Walk and Jump, these states are used in movement and
	/// to determine when gravity should be applied and when certain animations
	/// should be played.
	/// </summary>
	public enum State
	{
		Stand,
		Walk,
		Jump
	}

}