using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Fonts.Tests
{
	internal class FontTextTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			verdana = FontXml.Default;
			tahoma = ContentLoader.Load<FontXml>("Tahoma30");
			CreateBackground();
		}

		private FontXml verdana;
		private FontXml tahoma;

		private static void CreateBackground()
		{
			new FilledRect(Center, Color.DarkGray) { RenderLayer = -2 };
			new FilledRect(CenterDot, Color.Red) { RenderLayer = -1 };
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.2f, 0.2f);
		private static readonly Rectangle CenterDot = Rectangle.FromCenter(0.5f, 0.5f, 0.01f, 0.01f);

		[Test, ApproveFirstFrameScreenshot]
		public void TextShouldSayChangedText()
		{
			new FontText(tahoma, "To be changed", Rectangle.One) { Text = "Changed\nText" };
		}

		[Test, ApproveFirstFrameScreenshot]
		public void TextShouldBeInTheMiddleOfTheScreen()
		{
			new FontText(tahoma, "Middle", Rectangle.Zero) { DrawArea = Rectangle.One };
		}

		[Test, ApproveFirstFrameScreenshot]
		public void FontDefaultsToVerdana12()
		{
			new FontText(FontXml.Default, "Verdana12 font", Center);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void MultiLineTextCentrallyAligned()
		{
			new FontText(verdana, "Text\ncentrally\naligned", Center);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void MultiLineTextLeftAligned()
		{
			CreateBackground();
			new FontText(FontXml.Default, "Text\nleft\naligned", Center)
			{
				HorizontalAlignment = HorizontalAlignment.Left
			};
		}

		[Test, ApproveFirstFrameScreenshot]
		public void MultiLineTextRightAligned()
		{
			new FontText(FontXml.Default, "Text\nright\naligned", Center)
			{
				HorizontalAlignment = HorizontalAlignment.Right
			};
		}

		[Test, ApproveFirstFrameScreenshot]
		public void MultiLineTextTopLeftAligned()
		{
			new FontText(FontXml.Default, "Text\ntop\nleft\naligned", Center)
			{
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Left
			};
		}

		[Test, ApproveFirstFrameScreenshot]
		public void MultiLineTextBottomRightAligned()
		{
			new FontText(FontXml.Default, "Text\nbottom\nright\naligned", Center)
			{
				VerticalAlignment = VerticalAlignment.Bottom,
				HorizontalAlignment = HorizontalAlignment.Right
			};
		}

		[Test, ApproveFirstFrameScreenshot]
		public void AlignToCenterAndEdges()
		{
			new FontText(FontXml.Default, "Center", Rectangle.One);
			new FontText(FontXml.Default, "Top", Top) { VerticalAlignment = VerticalAlignment.Top };
			new FontText(FontXml.Default, "Bottom", Bottom)
			{
				VerticalAlignment = VerticalAlignment.Bottom
			};
			new FontText(FontXml.Default, "Left", Left)
			{
				HorizontalAlignment = HorizontalAlignment.Left
			};
			new FontText(FontXml.Default, "Right", Right)
			{
				HorizontalAlignment = HorizontalAlignment.Right
			};
		}

		private static readonly Rectangle Top = new Rectangle(0.5f, 0.4f, 0.0f, 0.0f);
		private static readonly Rectangle Bottom = new Rectangle(0.5f, 0.6f, 0.0f, 0.0f);
		private static readonly Rectangle Left = new Rectangle(0.4f, 0.5f, 0.0f, 0.0f);
		private static readonly Rectangle Right = new Rectangle(0.6f, 0.5f, 0.0f, 0.0f);

		[Test, ApproveFirstFrameScreenshot]
		public void AlignToCorners()
		{
			new FontText(FontXml.Default, "TL", TopLeft)
			{
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Left
			};
			new FontText(FontXml.Default, "BL", BottomLeft)
			{
				VerticalAlignment = VerticalAlignment.Bottom,
				HorizontalAlignment = HorizontalAlignment.Left
			};
			new FontText(FontXml.Default, "TR", TopRight)
			{
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Right
			};
			new FontText(FontXml.Default, "BR", BottomRight)
			{
				VerticalAlignment = VerticalAlignment.Bottom,
				HorizontalAlignment = HorizontalAlignment.Right
			};
		}

		private static readonly Rectangle TopLeft = new Rectangle(0.4f, 0.4f, 0.0f, 0.0f);
		private static readonly Rectangle TopRight = new Rectangle(0.6f, 0.4f, 0.0f, 0.0f);
		private static readonly Rectangle BottomLeft = new Rectangle(0.4f, 0.6f, 0.0f, 0.0f);
		private static readonly Rectangle BottomRight = new Rectangle(0.6f, 0.6f, 0.0f, 0.0f);

		[Test, CloseAfterFirstFrame]
		public void RenderingHiddenFontTextDoesNotThrowException()
		{
			new FontText(FontXml.Default, "Hi", Rectangle.One) { Visibility = Visibility.Hide };
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}

		[Test, Ignore]
		public void UsingNonExistentFontUsesDefault()
		{
			new FontText(ContentLoader.Load<FontXml>("Missing"), "DefaultFont", Rectangle.One);
		}

		[Test]
		public void CounterWithSpriteFontText()
		{
			var font = ContentLoader.Load<FontXml>("Verdana12");
			var text = new FontText(font, "", Rectangle.FromCenter(0.5f, 0.5f, 0.05f, 0.05f))
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			text.Start<Count>();
		}

		[Test]
		public void FontTextShouldFallBackToUsingVectorText()
		{
			var font = ContentLoader.Load<FontXml>("Verdana12");
			font.WasLoadedOk = false;
			var text = new FontText(font, "", Rectangle.FromCenter(0.5f, 0.5f, 0.05f, 0.05f))
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			text.Start<Count>();
		}

		private class Count : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (FontText text in entities)
					text.Text = "" + count;
				count++;
			}

			private int count;
		}
	}
}