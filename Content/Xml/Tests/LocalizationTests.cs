using System.Collections.Generic;
using DeltaEngine.Content.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	class LocalizationTests
	{
		[Test]
		public void GetLocalizedString()
		{
			using (new MockContentLoader(new ContentDataResolver()))
			{
				var localization = ContentLoader.Load<Localization>("Texts");
				localization.TwoLetterLanguageName = "en";
				Assert.AreEqual(localization.GetText("Go"), "Go");
				localization.TwoLetterLanguageName = "de";
				Assert.AreEqual(localization.GetText("Go"), "Los");
				localization.TwoLetterLanguageName = "es";
				Assert.AreEqual(localization.GetText("Go"), "¡vamos!");
				Assert.Throws <KeyNotFoundException>(() => 
					localization.GetText("ThatIsATestExampleToThrowOneException"));
			}
		} 
	}
}