namespace System
{
	public class EventArgs<T> : EventArgs
	{
		public T Argument { get; set; }

		public EventArgs()
		{
		}

		public EventArgs(T arg)
		{
			this.Argument = arg;
		}
	}
}
