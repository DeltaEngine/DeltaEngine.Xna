using System;
using DeltaEngine.Content;

namespace DeltaEngine.Platforms
{
	internal class AutofacContentDataResolver : ContentDataResolver
	{
		public AutofacContentDataResolver(Resolver resolver)
		{
			this.resolver = resolver;
		}

		private readonly Resolver resolver;

		public override ContentData Resolve(Type contentType, string contentName)
		{
			return resolver.Resolve(contentType, contentName) as ContentData;
		}

		public override ContentData Resolve(Type contentType, ContentCreationData data)
		{
			return resolver.Resolve(contentType, data) as ContentData;
		}

		public override void MakeSureResolverIsInitializedAndContentIsReady()
		{
			resolver.MakeSureContentManagerIsReady();
		}
	}
}
