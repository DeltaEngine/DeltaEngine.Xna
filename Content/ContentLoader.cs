using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Loads content types like images, sounds, xml files, levels, etc. Returns cached useable
	/// instances and provides quick and easy access to all cached data plus creation of dynamic data.
	/// </summary>
	public abstract class ContentLoader : IDisposable
	{
		protected ContentLoader(string contentPath)
		{
			if (current != null && !current.GetType().Name.StartsWith("Mock"))
				throw new ContentLoaderAlreadyExistsItIsOnlyAllowedToSetBeforeTheAppStarts();
			current = this;
			this.contentPath = contentPath;
		}

		public class ContentLoaderAlreadyExistsItIsOnlyAllowedToSetBeforeTheAppStarts : Exception {}

		/// <summary>
		/// Normally set in Platforms.AppRunner to DeveloperOnlineContentLoader by creating it.
		/// </summary>
		internal static ContentLoader current;
		internal ContentDataResolver resolver;

		protected readonly string contentPath;

		public static Content Load<Content>(string contentName) where Content : ContentData
		{
			if (!IsGeneratedContentName(contentName))
				if (Path.HasExtension(contentName))
					throw new ContentNameShouldNotHaveExtension();
			return Load(typeof(Content), contentName) as Content;
		}

		private static bool IsGeneratedContentName(string contentName)
		{
			return contentName.StartsWith("<Generated");
		}

		public class ContentNameShouldNotHaveExtension : Exception {}

		internal static ContentData Load(Type contentType, string contentName)
		{
			if (current == null)
				throw new NoContentLoaderWasInitialized();
			if (current.resolver == null)
				throw new NoContentResolverWasSet();
			current.resolver.MakeSureResolverIsInitializedAndContentIsReady();
			if (IsGeneratedContentName(contentName))
				return current.resolver.Resolve(contentType, contentName);
			if (!current.resources.ContainsKey(contentName))
				return current.LoadAndCacheContent(contentType, contentName);
			if (!current.resources[contentName].IsDisposed)
				return current.GetCachedResource(contentType, contentName);
			current.resources.Remove(contentName);
			return current.LoadAndCacheContent(contentType, contentName);
		}

		internal class NoContentLoaderWasInitialized : Exception {}
		internal class NoContentResolverWasSet : Exception {}

		public static bool Exists(string contentName)
		{
			return CurrentLoaderGetMetaData(contentName) != null;
		}

		private static ContentMetaData CurrentLoaderGetMetaData(string contentName)
		{
			if (current == null)
				throw new NoContentLoaderWasInitialized();
			current.resolver.MakeSureResolverIsInitializedAndContentIsReady();
			return current.GetMetaData(contentName);
		}

		public static bool Exists(string contentName, ContentType type)
		{
			var metaData = CurrentLoaderGetMetaData(contentName);
			return metaData != null && metaData.Type == type;
		}

		private readonly Dictionary<string, ContentData> resources =
			new Dictionary<string, ContentData>();

		private ContentData LoadAndCacheContent(Type contentType, string contentName)
		{
			var contentData = resolver.Resolve(contentType, contentName);
			LoadMetaDataAndContent(contentData);
			resources.Add(contentName, contentData);
			return contentData;
		}

		protected abstract ContentMetaData GetMetaData(string contentName,
			Type contentClassType = null);

		private void LoadMetaDataAndContent(ContentData contentData)
		{
			contentData.MetaData = GetMetaData(contentData.Name, contentData.GetType());
			if (contentData.MetaData != null)
				contentData.InternalLoad(GetContentDataStream);
			else if (contentData.InternalAllowCreationIfContentNotFound)
				LoadContentDefaultDataWhenNotFound(contentData);
			else
				throw new ContentNotFound(contentData.Name);
		}

		private static void LoadContentDefaultDataWhenNotFound(ContentData contentData)
		{
			if (!current.GetType().Name.StartsWith("Mock"))
				Logger.Warning("Content not found: " + contentData); // ncrunch: no coverage
			contentData.InternalCreateDefault();
		}

		public class ContentNotFound : Exception
		{
			public ContentNotFound(string contentName)
				: base(contentName) {}
		}

		protected virtual Stream GetContentDataStream(ContentData content)
		{
			if (String.IsNullOrEmpty(content.MetaData.LocalFilePath))
				return Stream.Null;
			string filePath = Path.Combine(contentPath, content.MetaData.LocalFilePath);
			try
			{
				return File.OpenRead(filePath);
			}
			catch (Exception ex)
			{
				throw new ContentFileDoesNotExist(filePath, ex);
			}
		}

		public class ContentFileDoesNotExist : Exception
		{
			public ContentFileDoesNotExist(string filePath, Exception innerException)
				: base(filePath, innerException) {}
		}

		private ContentData GetCachedResource(Type contentType, string contentName)
		{
			var cachedResource = resources[contentName];
			if (contentType.IsInstanceOfType(cachedResource))
				return cachedResource;
			throw new CachedResourceExistsButIsOfTheWrongType("Content '" + contentName + "' of type '" +
				contentType + "' requested - but type '" + cachedResource.GetType() + "' found in cache" +
				"\n '" + contentName + "' should not be in meta data files twice with different suffixes!");
		}

		public class CachedResourceExistsButIsOfTheWrongType : Exception
		{
			public CachedResourceExistsButIsOfTheWrongType(string message)
				: base(message) {}
		}

		public static T Create<T>(ContentCreationData creationData) where T : ContentData
		{
			if (current == null)
				throw new NoContentLoaderWasInitialized();
			return current.resolver.Resolve(typeof(T), creationData) as T;
		}

		public static void ReloadContent(string contentName)
		{
			var content = current.resources[contentName];
			current.LoadMetaDataAndContent(content);
			content.FireContentChangedEvent();
		}

		public virtual void Dispose()
		{
			current = null;
		}

		public static string ContentLocale
		{
			get { return current.locale; }
			set
			{
				if (string.IsNullOrEmpty(value))
					return;
				current.locale = value;
			}
		}

		private string locale = "en";

		internal static bool HasValidContentForStartup()
		{
			return current.HasValidContentAndMakeSureItIsLoaded();
		}

		protected abstract bool HasValidContentAndMakeSureItIsLoaded();

		protected string ContentMetaDataFilePath
		{
			get { return Path.Combine(contentPath, "ContentMetaData.xml"); }
		}

		public static void RemoveResource(string key)
		{
			if (current.resources.ContainsKey(key))
				current.resources.Clear();
		}
	}
}