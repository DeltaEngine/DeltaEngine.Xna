namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Initializes the XnaResolver to get started. To execute the app call Run.
	/// </summary>
	public abstract class App
	{
		protected void Run()
		{
			resolver.Run();
		}

		private readonly XnaResolver resolver = new XnaResolver();

		protected T Resolve<T>() where T : class
		{
			return resolver.Resolve<T>();
		}
	}
}