using System;
using System.Collections.Generic;
using System.Drawing;

namespace GXPEngine.Comics;
public class EpilogueManager : Pivot
{
	private EasyDraw background;
	private List<Sprite> pages;

	public bool isActive;


	public EpilogueManager()
	{
		background = new EasyDraw(4800, game.height);
		background.Clear(Color.Black);
		AddChild(background);

		isActive = false;

		pages = new List<Sprite>();
		
		for (int i = 0; i < 7; i++)
		{
			pages.Add(new Sprite($"comics/epilogue/page1/{7-i}small.png"));
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