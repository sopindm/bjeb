namespace bjeb
{
	public class Computer
	{
		public Gui gui
		{
			get;
			private set;
		}

		public Computer()
		{
			gui = new Gui();
		}
	}
}

