﻿using Microsoft.Xna.Framework;
using System;

namespace MonoSAMFramework.Portable.Input
{
	public class PointerEventArgs : EventArgs
	{
		public readonly float X;
		public readonly float Y;

		public PointerEventArgs(float px, float py)
		{
			X = px;
			Y = py;
		}

		public PointerEventArgs(Point p)
		{
			X = p.X;
			Y = p.Y;
		}
	}
}
