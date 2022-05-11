﻿using System;

namespace GXPEngine;

public class Animation : AnimationSprite
{
	private bool playOnce;

	private bool destroyNextFrame;

	public Animation(string path, int cols, int rows, int frames, bool playOnce_ = false, byte animationDelay = 255) : base(path, cols, rows, frames, addCollider:false)
	{
		playOnce = playOnce_;
		_animationDelay = animationDelay;
		SetCycle(0,4);
	}

	public void Update()
	{
		Animate(Time.deltaTime);

		if (destroyNextFrame) Destroy();

		
		if (playOnce)
		{
			Console.WriteLine(currentFrame);
			if (currentFrame == frameCount-1)
			{
				destroyNextFrame = true;
			}
		}
	}
}