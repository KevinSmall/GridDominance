﻿using System;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;

namespace GridDominance.Shared
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MainGame : MonoSAMGame
	{
		protected override void OnInitialize()
		{
#if __DESKTOP__
			IsMouseVisible = true;
			Graphics.IsFullScreen = false;

			Graphics.PreferredBackBufferWidth = 1200;
			Graphics.PreferredBackBufferHeight = 900;
			Window.AllowUserResizing = true;

#if DEBUG
			Graphics.SynchronizeWithVerticalRetrace = false;
			IsFixedTimeStep = false;
			TargetElapsedTime = TimeSpan.FromMilliseconds(1);
#endif

			Graphics.ApplyChanges();
			Window.Position = new Point((1920 - Graphics.PreferredBackBufferWidth) / 2, (1080 - Graphics.PreferredBackBufferHeight) / 2);

#else
			Graphics.IsFullScreen = true;
			Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
			Graphics.ApplyChanges();
#endif
		}

		protected override void OnAfterInitialize()
		{
			SetCurrentScreen(new GDGameScreen(this, Graphics, Levels.LEVEL_003));
		}

		protected override void LoadContent()
		{
			Textures.Initialize(Content, GraphicsDevice);
			Levels.LoadContent(Content);
		}

		protected override void UnloadContent()
		{
			// NOP
		}
	}
}

