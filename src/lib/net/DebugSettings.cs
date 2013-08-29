namespace bjeb.net
{
	[Serializable(17)]
	public class DebugSettings: Serializable
	{
		public float guiUpdateRate
		{
			get;
			set;
		}

		public float stateUpdateRate
		{
			get;
			set;
		}

		public bool showGui
		{
			get;
			set;
		}

		public bool updateGui
		{
			get;
			set;
		}

		public bool updateWindows
		{
			get;
			set;
		}

		public bool updateState
		{
			get;
			set;
		}

		public bool control
		{
			get;
			set;
		}

		public DebugSettings()
		{
			guiUpdateRate = 60;
			stateUpdateRate = 60;

			showGui = true;
			updateGui = true;
			updateWindows = true;

			updateState = true;
			control = true;
		}

		override protected void doSerialize(Stream stream)
		{
			stream.write(guiUpdateRate);
			stream.write(stateUpdateRate);

			stream.write(showGui);
			stream.write(updateGui);
			stream.write(updateWindows);
			stream.write(updateState);
			stream.write(control);
		}

		override protected void doDeserialize(Stream stream)
		{
			guiUpdateRate = stream.readFloat();
			stateUpdateRate = stream.readFloat();
			
			showGui = stream.readBool();
			updateGui = stream.readBool();
			updateWindows = stream.readBool();
			updateState = stream.readBool();
			control = stream.readBool();
		}
	}
}