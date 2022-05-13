using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace GXPEngine.Comics;
public class EpilogueManager : Pivot
{
	private EasyDraw background;
	private List<Sprite> pages;

	public bool isActive;

	private Sound snap;


	public EpilogueManager()
	{
		background = new EasyDraw(4800, game.height);
		background.Clear(Color.Black);
		AddChild(background);

		snap = new Sound("sounds/snap.wav");

		isActive = false;

		pages = new List<Sprite>();
		
		for (int i = 0; i < 5; i++)
		{
			pages.Add(new Sprite($"comics/epilogue/page1/{5-i}small.png"));
		}
		
		for (int i = 0; i < pages.Count; i++)
		{
			pages[i].SetXY(345.5f,0);
			AddChild(pages[i]);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && isActive)
		{
			if (pages.Count != 1)
			{
				if (pages.Count == 2)
				{
					snap.Play(volume: 0.1f);
				}
				
				RemoveChild(pages[pages.Count-1]); 
				Console.WriteLine(pages[pages.Count-1]);
				pages[pages.Count-1].Destroy();
				pages.RemoveAt(pages.Count-1);
			}
			else
			{
				game.Destroy();
			}
		}
	}
}