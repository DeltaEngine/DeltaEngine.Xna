using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Content.Disk.Tests
{
	class MetaDataCreatorTests
	{
		[Test, Ignore]
		public void TryCreatingAnimationFromFiles()
		{
			File.Delete(Path.Combine("Content", "ContentMetaData.xml"));
			ContentLoader.current = new DiskContentLoader();
			ContentLoader.current.resolver = new ContentDataResolver();
			Assert.IsTrue(ContentLoader.Exists("ImageAnimation", ContentType.ImageAnimation));
		}
	}
}
