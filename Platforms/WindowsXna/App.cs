using DeltaEngine.Core;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Initializes the XnaResolver to get started. To execute the app call Run.
	/// </summary>
	public abstract class App
	{
		protected App() { }

		protected App(Window windowToRegister)
		{
			resolver.RegisterInstance(windowToRegister);
		}

		private readonly XnaResolver resolver = new XnaResolver();

		protected void Run()
		{
			resolver.Run();
		}

		protected T Resolve<T>() where T : class
		{
			return resolver.Resolve<T>();
		}
	}
}