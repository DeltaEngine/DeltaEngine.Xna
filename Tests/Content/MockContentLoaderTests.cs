using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Mocks;
using DeltaEngine.Datatypes;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Content
{
	public class MockContentLoaderTests
	{
		[SetUp]
		public void CreateContentLoaderInstanceAndTestContent()
		{
			new MockContentLoader(new ContentDataResolver());
			testXmlContent = ContentLoader.Load<MockXmlContent>(TestXmlContentName);
		}

		[TearDown]
		public void DisposeContentLoader()
		{
			if (ContentLoader.current != null)
				ContentLoader.current.Dispose();
		}

		private MockXmlContent testXmlContent;
		private const string TestXmlContentName = "FakeXml";

		[Test]
		public void ContentDataToString()
		{
			string contentDataString = testXmlContent.ToString();
			Assert.IsTrue(contentDataString.Contains(TestXmlContentName), contentDataString);
		}

		[Test]
		public void AllowCreationIfContentNotFound()
		{
			Assert.IsFalse(testXmlContent.InternalAllowCreationIfContentNotFound);
		}

		[Test]
		public void InternalCreateDefault()
		{
			var originalData = testXmlContent.Data;
			testXmlContent.InternalCreateDefault();
			Assert.AreEqual(TestXmlContentName, testXmlContent.Name);
			Assert.AreNotEqual(originalData, testXmlContent.Data);
		}

		[Test]
		public void ThrowExceptionIfContentLoadIsDoneWithoutContentLoaderInstance()
		{
			ContentLoader.current = null;
			Assert.Throws<ContentLoader.NoContentLoaderWasInitialized>(
				() => ContentLoader.Load<MockXmlContent>(TestXmlContentName));
		}

		[Test]
		public void CheckForContentOnStartup()
		{
			Assert.IsTrue(ContentLoader.HasValidContentForStartup());
		}

		[Test]
		public void LoadContent()
		{
			Assert.AreEqual(TestXmlContentName, testXmlContent.Name);
			Assert.IsFalse(testXmlContent.IsDisposed);
			Assert.Greater(testXmlContent.LoadCounter, 0);
		}

		[Test]
		public void LoadGeneratedContent()
		{
			Assert.DoesNotThrow(() => ContentLoader.Load<MockXmlContent>("<GeneratedMockXml>"));
		}

		[Test]
		public void LoadWithExtensionNotAllowed()
		{
			Assert.Throws<ContentLoader.ContentNameShouldNotHaveExtension>(
				() => ContentLoader.Load<MockXmlContent>("Test.xml"));
		}

		[Test]
		public void CheckIfContentExists()
		{
			Assert.IsTrue(ContentLoader.Exists(TestXmlContentName));
			Assert.IsTrue(ContentLoader.Exists(TestXmlContentName, ContentType.Xml));
			Assert.IsFalse(ContentLoader.Exists(TestXmlContentName, ContentType.Camera));
		}

		[Test]
		public void ThrowExceptionIfExistsCheckIsDoneWithoutContentLoaderInstance()
		{
			ContentLoader.current = null;
			Assert.Throws<ContentLoader.NoContentLoaderWasInitialized>(
				() => ContentLoader.Exists(TestXmlContentName));
		}

		[Test]
		public void LoadCachedContent()
		{
			var contentTwo = ContentLoader.Load<MockXmlContent>(TestXmlContentName);
			Assert.IsFalse(contentTwo.IsDisposed);
			Assert.AreEqual(testXmlContent, contentTwo);
		}

		[Test]
		public void RemoveCachedContent()
		{
			Assert.DoesNotThrow(() => ContentLoader.RemoveResource(TestXmlContentName));
		}

		[Test]
		public void ForceReload()
		{
			ContentLoader.ReloadContent(TestXmlContentName);
			Assert.Greater(testXmlContent.LoadCounter, 1);
			Assert.AreEqual(1, testXmlContent.changeCounter);
		}

		[Test]
		public void TwoContentFilesShouldNotReloadEachOther()
		{
			var content2 = ContentLoader.Load<MockXmlContent>(TestXmlContentName + 2);
			ContentLoader.ReloadContent(TestXmlContentName);
			Assert.AreEqual(1, content2.LoadCounter);
			Assert.AreEqual(2, testXmlContent.LoadCounter);
		}

		[Test]
		public void DisposeContent()
		{
			Assert.IsFalse(testXmlContent.IsDisposed);
			testXmlContent.Dispose();
			Assert.IsTrue(testXmlContent.IsDisposed);
		}

		[Test]
		public void CheckContentLocale()
		{
			string contentLocale = ContentLoader.ContentLocale;
			Assert.IsNotEmpty(contentLocale);
			ContentLoader.ContentLocale = null;
			ContentLoader.ContentLocale = contentLocale;
		}

		[Test]
		public void DisposeAndLoadAgainShouldReturnFreshInstance()
		{
			testXmlContent.Dispose();
			var freshContent = ContentLoader.Load<MockXmlContent>(TestXmlContentName);
			Assert.IsFalse(freshContent.IsDisposed);
		}

		[Test]
		public void LoadWithoutContentNameIsNotAllowed()
		{
			Assert.Throws<ContentData.ContentNameMissing>(() => new MockXmlContent(""));
		}

		[Test]
		public void ExceptionOnInstancingFromOutsideContentLoader()
		{
			Assert.Throws<ContentData.MustBeCalledFromContentLoader>(
				() => new MockXmlContent("VectorText"));
		}

		[Test]
		public void ThrowExceptionIfContentNameDoesNotMatchToContentType()
		{
			Assert.Throws<ContentLoader.CachedResourceExistsButIsOfTheWrongType>(
				() => ContentLoader.Load<MockImage>(TestXmlContentName));
		}

		[Test]
		public void ThrowExceptionOnLoadWithWrongMetaData()
		{
			const ContentType WrongMetaDataType = ContentType.Video;
			Assert.AreNotEqual(testXmlContent.MetaData.Type, WrongMetaDataType);
			testXmlContent.MetaData.Type = WrongMetaDataType;
			Func<ContentData, Stream> fakeGetDataStreamMethod = data => Stream.Null;
			Assert.Throws<ContentData.DoesNotMatchMetaDataType>(
				() => testXmlContent.InternalLoad(fakeGetDataStreamMethod));
		}

		[Test]
		public void ThrowExceptionIfCallCreateWithoutContentLoaderInstance()
		{
			ContentLoader.current = null;
			Assert.Throws<ContentLoader.NoContentLoaderWasInitialized>(
				() => ContentLoader.Create<MockXmlContent>(null));
		}

		[Test]
		public void LoadContentViaCreationData()
		{
			var image = ContentLoader.Load<MockImage>("MockImage");
			var animationData = new SpriteSheetAnimationCreationData(image, 1.0f, new Size(1, 1));
			var animation = ContentLoader.Create<SpriteSheetAnimation>(animationData);
			Assert.AreEqual(animationData.Image, animation.Image);
			Assert.AreEqual(animationData.DefaultDuration, animation.DefaultDuration);
			Assert.AreEqual(animationData.SubImageSize, animation.SubImageSize);
		}

		[Test]
		public void ThrowExceptionIfLoadDefaultDataIsNotAllowed()
		{
			Assert.Throws<ContentLoader.ContentNotFound>(
				() => ContentLoader.Load<MockXmlContent>("UnavailableXmlContent"));
		}

		[Test]
		public void LoadDefaultDataIfAllowed()
		{
			Assert.DoesNotThrow(
				() => ContentLoader.Load<DynamicXmlMockContent>("UnavailableDynamicContent"));
		}

		private class DynamicXmlMockContent : ContentData
		{
			public DynamicXmlMockContent(string contentName)
				: base(contentName) {}

			protected override void DisposeData() {}
			protected override void LoadData(Stream fileData) {}
			protected override bool AllowCreationIfContentNotFound
			{
				get { return true; }
			}
		}

		[Test]
		public void ContentLoadWithNullStream()
		{
			ContentLoader.current = new FakeContentLoader();
			Assert.DoesNotThrow(() =>ContentLoader.Load<MockXmlContent>("XmlContentWithNoPath"));
		}

		[Test]
		public void ContentLoadWithWrongFilePath()
		{
			ContentLoader.current = new FakeContentLoader();
			Assert.Throws<ContentLoader.ContentFileDoesNotExist>(
				() => ContentLoader.Load<MockXmlContent>("ContentWithWrongPath"));
		}

		[Test]
		public void ThrowExceptionIfSecondContentLoaderInstanceIsUsed()
		{
			ContentLoader.current = new FakeContentLoader();
			Assert.Throws
				<ContentLoader.ContentLoaderAlreadyExistsItIsOnlyAllowedToSetBeforeTheAppStarts>(
					() => new FakeContentLoader());
		}

		[Test]
		public void ThrowExceptionIfContentLoaderHasNoResolver()
		{
			ContentLoader.current.resolver = null;
			Assert.Throws<ContentLoader.NoContentResolverWasSet>(
				() => ContentLoader.Load<MockXmlContent>(TestXmlContentName));
		}

		private class FakeContentLoader : ContentLoader
		{
			public FakeContentLoader()
				: base("NoPath")
			{
				resolver = new ContentDataResolver();
			}

			protected override ContentMetaData GetMetaData(string contentName,
				Type contentClassType = null)
			{
				HasValidContentAndMakeSureItIsLoaded();
				var metaData = new ContentMetaData { Type = ContentType.Xml };
				if (contentName.Contains("WrongPath"))
					metaData.LocalFilePath = "No.xml";
				return metaData;
			}

			protected override bool HasValidContentAndMakeSureItIsLoaded()
			{
				return ContentMetaDataFilePath == null;
			}
		}
	}
}