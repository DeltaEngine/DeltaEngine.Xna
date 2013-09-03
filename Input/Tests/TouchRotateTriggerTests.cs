﻿using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchRotateTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void RotateRedCircle()
		{
			new FontText(FontXml.Default, "Rotate red box with 2 finger touch", Rectangle.One);
			var ellipse = new FilledRect(new Rectangle(0.1f, 0.1f, 0.2f, 0.1f), Color.Red);
			//rotation needs some better input
			new Command(() => ellipse.Center = Point.Half).Add(new TouchRotateTrigger());
		}
	}
}