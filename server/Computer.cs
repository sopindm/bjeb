namespace bjeb
{
	public class Computer
	{
		public gui.Screen screen
		{
			get;
			private set;
		}

		public Computer()
		{
			screen = new gui.Screen();
		}
	}
}

