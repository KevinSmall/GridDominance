﻿using System;
using GridDominance.Levelformat.Parser;
using GridDominance.Levelformat.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace GridDominance.Content.Pipeline.GDLevel
{
	[ContentTypeWriter]
	public class GDLevelWriter : ContentTypeWriter<LevelFile>
	{
		protected override void Write(ContentWriter output, LevelFile value)
		{
			var start = output.BaseStream.Position;
			value.BinarySerialize(output);
			var length = output.BaseStream.Position - start;

			Console.WriteLine("Writing " + length + " byte long serialized file");

		}
		
		public override string GetRuntimeType(TargetPlatform targetPlatform)
		{
			return typeof(LevelFile).AssemblyQualifiedName;
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof (GDLevelReader).AssemblyQualifiedName;
		}
	}
}