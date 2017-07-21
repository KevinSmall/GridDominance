﻿using System;
using System.Diagnostics;
using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.WorldMapScreen;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using GridDominance.Shared.Screens.OverworldScreen;
using Microsoft.Xna.Framework.Media;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Localization;

namespace GridDominance.Shared.Screens
{
	public static class DebugUtils
	{
#if DEBUG
		public static readonly TimingCounter TIMING_KI    = new TimingCounter(32);
		public static readonly TimingCounter TIMING_LASER = new TimingCounter(32);

		public static DebugTextDisplay CreateDisplay(GameScreen scrn)
		{
			var debugDisp = new DebugTextDisplay(scrn.Graphics.GraphicsDevice, Textures.DebugFont);
			var gdg = scrn as GDGameScreen;

			debugDisp.AddLine(() => $"Device = {scrn.Game.Bridge.DeviceName} | Version = {scrn.Game.Bridge.DeviceVersion} | Debugger = {Debugger.IsAttached}");
			debugDisp.AddLine(() => $"FPS={scrn.FPSCounter.AverageAPS:0000.0} (curr={scrn.FPSCounter.CurrentAPS:0000.0} delta={scrn.FPSCounter.AverageDelta * 1000:000.00} min={scrn.FPSCounter.MinimumAPS:0000.0} (d={scrn.FPSCounter.MaximumDelta * 1000:0000.0}) cycletime={scrn.FPSCounter.AverageCycleTime * 1000:000.00} (max={scrn.FPSCounter.MaximumCycleTime * 1000:000.00} curr={scrn.FPSCounter.CurrentCycleTime * 1000:000.00}) total={scrn.FPSCounter.TotalActions:000000})");
			debugDisp.AddLine(() => $"UPS={scrn.UPSCounter.AverageAPS:0000.0} (curr={scrn.UPSCounter.CurrentAPS:0000.0} delta={scrn.UPSCounter.AverageDelta * 1000:000.00} min={scrn.UPSCounter.MinimumAPS:0000.0} (d={scrn.UPSCounter.MaximumDelta * 1000:0000.0}) cycletime={scrn.UPSCounter.AverageCycleTime * 1000:000.00} (max={scrn.UPSCounter.MaximumCycleTime * 1000:000.00} curr={scrn.UPSCounter.CurrentCycleTime * 1000:000.00}) total={scrn.UPSCounter.TotalActions:000000})");
			debugDisp.AddLine(() => $"GC = Time since GC:{scrn.GCMonitor.TimeSinceLastGC:00.00}s ({scrn.GCMonitor.TimeSinceLastGC0:000.00}s | {scrn.GCMonitor.TimeSinceLastGC1:000.00}s | {scrn.GCMonitor.TimeSinceLastGC2:000.00}s) Memory = {scrn.GCMonitor.TotalMemory:000.0}MB Frequency = {scrn.GCMonitor.GCFrequency:0.000}");
			debugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f / Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(scrn.Game.GraphicsDevice):#.00}");
			debugDisp.AddLine(() => $"Entities = {scrn.Entities.Count(),3} | EntityOps = {scrn.Entities.Enumerate().Sum(p => p.ActiveEntityOperations.Count()):00} | Particles = {scrn.Entities.Enumerate().OfType<IParticleOwner>().Sum(p => p.ParticleCount),3} (Visible: {scrn.Entities.Enumerate().Where(p => p.IsInViewport).OfType<IParticleOwner>().Sum(p => p.ParticleCount),3})");
			debugDisp.AddLine(() => $"GamePointer = ({scrn.InputStateMan.GetCurrentState().GamePointerPosition.X:000.0}|{scrn.InputStateMan.GetCurrentState().GamePointerPosition.Y:000.0}) | HUDPointer = ({scrn.InputStateMan.GetCurrentState().HUDPointerPosition.X:000.0}|{scrn.InputStateMan.GetCurrentState().HUDPointerPosition.Y:000.0}) | PointerOnMap = ({scrn.InputStateMan.GetCurrentState().GamePointerPositionOnMap.X:000.0}|{scrn.InputStateMan.GetCurrentState().GamePointerPositionOnMap.Y:000.0})");
			debugDisp.AddLine("DebugGestures", () => $"Pinching = {scrn.InputStateMan.GetCurrentState().IsGesturePinching} & PinchComplete = {scrn.InputStateMan.GetCurrentState().IsGesturePinchComplete} & PinchPower = {scrn.InputStateMan.GetCurrentState().LastPinchPower}");
			debugDisp.AddLine(() => $"OGL Sprites = {scrn.LastReleaseRenderSpriteCount:0000} (+ {scrn.LastDebugRenderSpriteCount:0000}); OGL Text = {scrn.LastReleaseRenderTextCount:0000} (+ {scrn.LastDebugRenderTextCount:0000})");
			debugDisp.AddLine(() => $"Map Offset = {scrn.MapOffset} (Map Center = {scrn.MapViewportCenter})");
			debugDisp.AddLine(() => $"SamSound = [Effects]: {MainGame.Inst.GDSound.GetEffectsStringState()} | [Music]: {MainGame.Inst.GDSound.GetMusicStringState()}");
			debugDisp.AddLine(() => $"Mediaplayer[{MediaPlayer.State}] = (Volume: {MediaPlayer.Volume:0.00}) ({MediaPlayer.PlayPosition.TotalSeconds:0}s) {{{string.Join(",", new[] { MediaPlayer.IsMuted ? "IsMuted" : "", MediaPlayer.GameHasControl ? "GameHasControl" : "", MediaPlayer.IsShuffled ? "IsShuffled" : "", MediaPlayer.IsVisualizationEnabled ? "IsVisualizationEnabled" : "IsRepeating", MediaPlayer.IsRepeating ? "" : "" }.Where(p => !string.IsNullOrWhiteSpace(p)))}}}");
			if (gdg != null) debugDisp.AddLine(() => $"LevelTime = {gdg.LevelTime:000.000} (finished={gdg.HasFinished})");

			if (scrn is GDWorldMapScreen) debugDisp.AddLine(() => $"CurrentLevelNode = {((GDWorldHUD)scrn.HUD).SelectedNode?.Blueprint?.Name ?? "NULL"}; FocusedHUDElement = {scrn.HUD.FocusedElement}; ZoomState = {((GDWorldMapScreen)scrn).ZoomState}");

			debugDisp.AddLine("DebugTimings", () => $"KI: [{TIMING_KI.Format()}] LASER: [{TIMING_LASER.Format()}]");

			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GraphicsDevice.Viewport=[{scrn.Game.GraphicsDevice.Viewport.Width}|{scrn.Game.GraphicsDevice.Viewport.Height}]");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.VirtualGuaranteedSize={scrn.VAdapterGame.VirtualGuaranteedSize} || GameAdapter.VirtualGuaranteedSize={scrn.VAdapterHUD.VirtualGuaranteedSize}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.RealGuaranteedSize={scrn.VAdapterGame.RealGuaranteedSize} || GameAdapter.RealGuaranteedSize={scrn.VAdapterHUD.RealGuaranteedSize}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.VirtualTotalSize={scrn.VAdapterGame.VirtualTotalSize} || GameAdapter.VirtualTotalSize={scrn.VAdapterHUD.VirtualTotalSize}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.RealTotalSize={scrn.VAdapterGame.RealTotalSize} || GameAdapter.RealTotalSize={scrn.VAdapterHUD.RealTotalSize}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.VirtualOffset={scrn.VAdapterGame.VirtualGuaranteedBoundingsOffset} || GameAdapter.VirtualOffset={scrn.VAdapterHUD.VirtualGuaranteedBoundingsOffset}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.RealOffset={scrn.VAdapterGame.RealGuaranteedBoundingsOffset} || GameAdapter.RealOffset={scrn.VAdapterHUD.RealGuaranteedBoundingsOffset}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.Scale={scrn.VAdapterGame.Scale} || GameAdapter.Scale={scrn.VAdapterHUD.Scale}");

			debugDisp.AddLine("ShowOperations", () => string.Join(Environment.NewLine, scrn.Entities.Enumerate().SelectMany(e => e.ActiveEntityOperations).Select(o => o.Name).GroupBy(p=>p).Select(p=>p.Count()==1 ? p.Key : $"{p.Key} (x{p.Count()})")));
			debugDisp.AddLine("ShowOperations", () => string.Join(Environment.NewLine, scrn.HUD.Enumerate().SelectMany(e => e.ActiveHUDOperations).Select(o => o.Name).GroupBy(p => p).Select(p => p.Count() == 1 ? p.Key : $"{p.Key} (x{p.Count()})")));

			debugDisp.AddTabularLine("ShowDebugShortcuts", DebugSettings.GetCategorizedSummaries);

			debugDisp.AddLogLines();

			debugDisp.AddLine("ShowSerializedProfile", () => MainGame.Inst.Profile.SerializeToString(128));

			debugDisp.AddLine("FALSE", () => scrn.InputStateMan.GetCurrentState().GetFullDebugSummary());
			debugDisp.AddLine("FALSE", () => scrn.Game.Bridge.FullDeviceInfoString);

			return debugDisp;
		}

		public static void CreateShortcuts(GameScreen scrn)
		{
#if __DESKTOP__
			DebugSettings.AddSwitch(null, "DBG", scrn, KCL.C(SKeys.D, SKeys.AndroidMenu), true);
#else
			DebugSettings.AddSwitch(null, "DBG", scrn, KCL.C(SKeys.D, SKeys.AndroidMenu), true);
#endif

			DebugSettings.AddTrigger("DBG", "SetQuality_1",         scrn, SKeys.D1, KeyModifier.Control,  x => Textures.ChangeQuality(scrn.Game.Content, TextureQuality.FD));
			DebugSettings.AddTrigger("DBG", "SetQuality_2",         scrn, SKeys.D2, KeyModifier.Control,  x => Textures.ChangeQuality(scrn.Game.Content, TextureQuality.BD));
			DebugSettings.AddTrigger("DBG", "SetQuality_3",         scrn, SKeys.D3, KeyModifier.Control,  x => Textures.ChangeQuality(scrn.Game.Content, TextureQuality.LD));
			DebugSettings.AddTrigger("DBG", "SetQuality_4",         scrn, SKeys.D4, KeyModifier.Control,  x => Textures.ChangeQuality(scrn.Game.Content, TextureQuality.MD));
			DebugSettings.AddTrigger("DBG", "SetQuality_5",         scrn, SKeys.D5, KeyModifier.Control,  x => Textures.ChangeQuality(scrn.Game.Content, TextureQuality.HD));
			DebugSettings.AddTrigger("DBG", "ResetProfile",         scrn, SKeys.R, KeyModifier.Control,   x => MainGame.Inst.ResetProfile());
			DebugSettings.AddTrigger("DBG", "ClearMessages",        scrn, SKeys.C, KeyModifier.None,      x => scrn.DebugDisp.Clear());
			DebugSettings.AddTrigger("DBG", "StartDebugLevel",      scrn, SKeys.D, KeyModifier.ShiftCtrl, x => MainGame.Inst.SetDebugLevelScreen());
			DebugSettings.AddTrigger("DBG", "ShowOverworld",        scrn, SKeys.O, KeyModifier.ShiftCtrl, x => MainGame.Inst.SetOverworldScreen());

			DebugSettings.AddSwitch("DBG", "PhysicsDebugView",      scrn, SKeys.F1,  KeyModifier.None,    false);
			DebugSettings.AddSwitch("DBG", "DebugTextDisplay",      scrn, SKeys.F2,  KeyModifier.None,    true);
			DebugSettings.AddSwitch("DBG", "DebugBackground",       scrn, SKeys.F3,  KeyModifier.None,    false);
			DebugSettings.AddSwitch("DBG", "DebugHUDBorders",       scrn, SKeys.F4,  KeyModifier.None,    false);
			DebugSettings.AddSwitch("DBG", "DebugNetwork",          scrn, SKeys.F5,  KeyModifier.None,    false);
			DebugSettings.AddSwitch("DBG", "ShowMatrixTextInfos",   scrn, SKeys.F6,  KeyModifier.None,    false);
			DebugSettings.AddSwitch("DBG", "ShowDebugMiniMap",      scrn, SKeys.F7,  KeyModifier.None,    false);
			DebugSettings.AddSwitch("DBG", "DebugEntityBoundaries", scrn, SKeys.F8,  KeyModifier.None,    false);
			DebugSettings.AddSwitch("DBG", "DebugEntityMouseAreas", scrn, SKeys.F9,  KeyModifier.None,    false); 
			DebugSettings.AddSwitch("DBG", "ShowOperations",        scrn, SKeys.F10, KeyModifier.None,    true);
			DebugSettings.AddSwitch("DBG", "DebugGestures",         scrn, SKeys.F11, KeyModifier.None,    false);
			DebugSettings.AddSwitch("DBG", "DebugLaserNetwork",     scrn, SKeys.F12, KeyModifier.None,    false);
			DebugSettings.AddSwitch("DBG", "DebugTimings",          scrn, SKeys.F1,  KeyModifier.Control, true); 
			DebugSettings.AddSwitch("DBG", "ContinoousLasers",      scrn, SKeys.F2,  KeyModifier.Control, false);
			DebugSettings.AddSwitch("DBG", "DebugMultiplayer",      scrn, SKeys.F3,  KeyModifier.Control, true);
			DebugSettings.AddSwitch("DBG", "ControlEnemies",        scrn, SKeys.F4,  KeyModifier.Control, false);

			DebugSettings.AddPush("DBG",  "ShowDebugShortcuts",     scrn, SKeys.Tab, KeyModifier.None);
			DebugSettings.AddPush("DBG",  "ShowSerializedProfile",  scrn, SKeys.O,   KeyModifier.None);
			DebugSettings.AddPush("TRUE", "HideHUD",                scrn, SKeys.H,   KeyModifier.None);
			DebugSettings.AddTrigger("DBG", "HideHUD",              scrn, SKeys.L,   KeyModifier.Control, RotateLang);

			if (scrn is GDGameScreen)      DebugSettings.AddSwitch( "DBG",  "ImmortalCannons",  scrn, SKeys.I,         KeyModifier.Control, false);
			if (scrn is GDGameScreen)      DebugSettings.AddPush(   "TRUE", "AssimilateCannon", scrn, SKeys.A,         KeyModifier.None);
			if (scrn is GDGameScreen)      DebugSettings.AddPush(   "TRUE", "LooseCannon",      scrn, SKeys.E,         KeyModifier.None);
			if (scrn is GDGameScreen)      DebugSettings.AddPush(   "TRUE", "AbandonCannon",    scrn, SKeys.S,         KeyModifier.None);

			if (scrn is GDWorldMapScreen)  DebugSettings.AddPush(   "TRUE", "UnlockNode",       scrn, SKeys.A,         KeyModifier.None);
			if (scrn is GDWorldMapScreen)  DebugSettings.AddPush(   "TRUE", "LeaveScreen",      scrn, SKeys.Backspace, KeyModifier.Control);
			if (scrn is GDWorldMapScreen)  DebugSettings.AddTrigger("TRUE", "ZoomOut",          scrn, SKeys.Z,         KeyModifier.None, x => ((GDWorldMapScreen)scrn).ZoomOut());

			if (scrn is GDOverworldScreen) DebugSettings.AddPush(   "TRUE", "UnlockNode",       scrn, SKeys.A,         KeyModifier.None);
			if (scrn is GDOverworldScreen) DebugSettings.AddPush(   "TRUE", "WorldPreview",     scrn, SKeys.P,         KeyModifier.None);
			if (scrn is GDOverworldScreen) DebugSettings.AddTrigger("TRUE", "ReuploadProfile",  scrn, SKeys.O,         KeyModifier.Control, x => MainGame.Inst.Backend.Reupload(MainGame.Inst.Profile).RunAsync() );
		}

		private static void RotateLang(DebugListener obj)
		{
			MainGame.Inst.Profile.Language = (MainGame.Inst.Profile.Language + 1) % L10N.LANG_COUNT;
			MainGame.Inst.SaveProfile();
			L10N.ChangeLanguage(MainGame.Inst.Profile.Language);
		}

#endif
		}
}
