namespace bjeb
{
	abstract class Computer
	{
		public abstract string name
		{
			get;
		}
	}

	class Computer1: Computer
	{
		public override string name
		{
			get
			{
				return "Computer1";
			}
		}
	}

	class Computer2: Computer
	{
		public override string name
		{
			get
			{
				return "Computer2";
			}
		}
	}
}