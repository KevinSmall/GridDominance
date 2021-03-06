﻿using System;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.NormalGameScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.Dialogs
{
	class WorldPreviewPanel : HUDRoundedPanel
	{
		public const float WIDTH =  14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT =  6.5f * GDConstants.TILE_WIDTH + 2.5f * GDConstants.TILE_WIDTH;

		public const float INNER_WIDTH  = 0.6f * GDConstants.VIEW_WIDTH;
		public const float INNER_HEIGHT = 0.6f * GDConstants.VIEW_HEIGHT;

		public override int Depth => 0;

		private readonly LevelBlueprint[] _blueprints;
		private readonly Guid _id;
		private readonly string _iabCode;

		private HUDSubScreenProxyRenderer _proxy;
		private HUDTextButton _button;

		private readonly int _worldNumber;
		private readonly int _unlockWorldNumber;

		public WorldPreviewPanel(LevelBlueprint[] bps, Guid unlockID, string iab, int worldnumber)
		{
			_blueprints = bps;
			_id = unlockID;
			_iabCode = iab;
			_worldNumber = worldnumber;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.Asbestos;

			_unlockWorldNumber = 1;

			if (UnlockManager.IsUnlocked(Levels.WORLD_001, false) != WorldUnlockState.OpenAndUnlocked)
			{
				_unlockWorldNumber = 0;
			}
			else
			{
				if (_worldNumber > 2 && UnlockManager.IsUnlocked(Levels.WORLD_002, false) == WorldUnlockState.OpenAndUnlocked) _unlockWorldNumber = 2;
				if (_worldNumber > 3 && UnlockManager.IsUnlocked(Levels.WORLD_003, false) == WorldUnlockState.OpenAndUnlocked) _unlockWorldNumber = 3;
				if (_worldNumber > 4 && UnlockManager.IsUnlocked(Levels.WORLD_004, false) == WorldUnlockState.OpenAndUnlocked) _unlockWorldNumber = 4;
			}
			//MainGame.Inst.GDBridge.IAB.SynchronizePurchases(GDConstants.IABList);
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			var prev = new GDGameScreen_Preview(MainGame.Inst, MainGame.Inst.Graphics, this, _blueprints, 0, _worldNumber);

			AddElement(_proxy = new HUDSubScreenProxyRenderer(prev)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(INNER_WIDTH, INNER_HEIGHT),
			});

			if (MainGame.Flavor == GDFlavor.IAB || MainGame.Flavor == GDFlavor.IAB_NOMP)
			{
				AddElement(_button = new HUDTextButton
				{
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),
					Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

					L10NText = L10NImpl.STR_PREV_BUYNOW,
					TextColor = Color.White,
					Font = Textures.HUDFontBold,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,

					BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.PeterRiver, 16),
					BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.BelizeHole, 16),

					Click = OnClickBuy,
				});

				AddElement(new HUDLabel
				{
					Alignment = HUDAlignment.BOTTOMCENTER,
					RelativePosition = new FPoint(0, 0.75f * GDConstants.TILE_WIDTH),
					Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.5f * GDConstants.TILE_WIDTH),

					L10NText = L10NImpl.STR_PREV_OR,
					TextColor = Color.White,
					Font = Textures.HUDFontBold,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
				});
			}

			AddElement(new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

				Text = L10N.TF(L10NImpl.STR_PREV_FINISHWORLD, _unlockWorldNumber),
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Turquoise, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.GreenSea, 16),

				Click = OnClickFinishPrev,
			});

			AddElement(new HUDImage
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 0.125f * GDConstants.TILE_WIDTH),
				Size = new FSize(0.75f * GDConstants.TILE_WIDTH, 0.75f * GDConstants.TILE_WIDTH),

				Image = Textures.TexIconScore,
			});

			AddElement(new HUDLabel
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(1.5f * GDConstants.TILE_WIDTH, 0.125f * GDConstants.TILE_WIDTH),
				Size = new FSize(4.25f * GDConstants.TILE_WIDTH, 0.75f * GDConstants.TILE_WIDTH),

				Text = $"{MainGame.Inst.Profile.TotalPoints} / {UnlockManager.PointsForUnlock(_id)}",
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 50,
				TextAlignment = HUDAlignment.CENTERLEFT,
			});
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			base.DoUpdate(gameTime, istate);

			if (_button !=null) _button.BackgroundNormal = _button.BackgroundNormal.WithColor(ColorMath.Blend(FlatColors.BelizeHole, FlatColors.WetAsphalt, FloatMath.PercSin(gameTime.TotalElapsedSeconds * 5)));

			if (MainGame.Inst.Profile.PurchasedWorlds.Contains(_id))
			{
				HUD.ShowToast("WPP::SUCC1", L10N.T(L10NImpl.STR_IAB_BUYSUCESS), 40, FlatColors.Emerald, FlatColors.Foreground, 2.5f);
				MainGame.Inst.SetOverworldScreenCopy(HUD.Screen as GDOverworldScreen);
			}

			if (MainGame.Inst.GDBridge.IAB.IsPurchased(_iabCode) == PurchaseQueryResult.Purchased)
			{
				MainGame.Inst.Profile.PurchasedWorlds.Add(_id);
				MainGame.Inst.SaveProfile();

				HUD.ShowToast("WPP::SUCC2", L10N.T(L10NImpl.STR_IAB_BUYSUCESS), 40, FlatColors.Emerald, FlatColors.Foreground, 2.5f);
				MainGame.Inst.SetOverworldScreenCopy(HUD.Screen as GDOverworldScreen);
			}
		}

		public void SetNextScreen(int idx)
		{
			var prev = new GDGameScreen_Preview(MainGame.Inst, MainGame.Inst.Graphics, this, _blueprints, idx, _worldNumber);

			_proxy.ChangeScreen(prev);
		}

		private void OnClickBuy(HUDTextButton sender, HUDButtonEventArgs args)
		{
			try
			{
				var r = MainGame.Inst.GDBridge.IAB.StartPurchase(_iabCode);
				switch (r)
				{
					case PurchaseResult.ProductNotFound:
						SAMLog.Error("IAB-PNF", "Product not found", "_iabCode -> " + _iabCode);
						Owner.HUD.ShowToast("WPP::ERR1", L10N.T(L10NImpl.STR_IAB_BUYERR), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.NotConnected:
						Owner.HUD.ShowToast("WPP::ERR2", L10N.T(L10NImpl.STR_IAB_BUYNOCONN), 40, FlatColors.Orange, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.CurrentlyInitializing:
						Owner.HUD.ShowToast("WPP::ERR3", L10N.T(L10NImpl.STR_IAB_BUYNOTREADY), 40, FlatColors.Orange, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.PurchaseStarted:
						SAMLog.Info("IAB-BUY", "PurchaseStarted");
						break;
					default:
						SAMLog.Error("WPP::EnumSwitch_OCB", "OnClickBuy()", "r -> " + r);
						break;
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("WPP::IAB_CALL", e);
				
			}
		}

		private void OnClickFinishPrev(HUDTextButton sender, HUDButtonEventArgs args)
		{
			if (_unlockWorldNumber == 0)
			{
				MainGame.Inst.SetTutorialLevelScreen();
				return;
			}

			int missPoints = UnlockManager.PointsForUnlock(_id) - MainGame.Inst.Profile.TotalPoints;
			
			HUD.ShowToast("WPP::HINT", L10N.TF(L10NImpl.STR_PREV_MISS_TOAST, missPoints, _worldNumber), 32, FlatColors.Orange, FlatColors.Foreground, 4f);

			MainGame.Inst.SetWorldMapScreenZoomedOut(Levels.WORLDS_BY_NUMBER[_unlockWorldNumber]);
		}
	}
}