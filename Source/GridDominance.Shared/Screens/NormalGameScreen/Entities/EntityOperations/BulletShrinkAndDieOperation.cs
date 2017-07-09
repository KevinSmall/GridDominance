﻿using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.NormalGameScreen.EntityOperations
{
	class BulletShrinkAndDieOperation : GameEntityOperation<BaseBullet>
	{
		public const string NAME = "BulletShrinkAndDie";

		private readonly bool _realKill;

		public BulletShrinkAndDieOperation(float time, bool kill = true) : base(NAME, time)
		{
			_realKill = kill;
		}

		protected override void OnStart(BaseBullet entity)
		{
			entity.BulletExtraScale = 1;
			entity.IsDying = true;
		}

		protected override void OnProgress(BaseBullet entity, float progress, SAMTime gameTime, InputState istate)
		{
			entity.BulletExtraScale = 1 - progress;
		}

		protected override void OnEnd(BaseBullet entity)
		{
			entity.BulletExtraScale = 0;
			entity.BulletAlpha = 0;
			if (_realKill) entity.Alive = false;
		}

		protected override void OnAbort(BaseBullet entity)
		{
			//
		}
	}
}
