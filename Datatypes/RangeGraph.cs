namespace DeltaEngine.Datatypes
{
	public class RangeGraph<T> : Range<T>
		where T : Lerp<T>
	{
		public RangeGraph() {}

		public RangeGraph(T minimum, T maximum)
			: base(minimum, maximum) {}
	}
}