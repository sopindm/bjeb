namespace bjeb
{
	abstract public class Computer: System.MarshalByRefObject
	{
		public abstract string name
		{
			get;
		}
	}
}