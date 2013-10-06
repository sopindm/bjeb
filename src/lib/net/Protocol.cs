using System.Collections.Generic;
using bjeb.gui;
using bjeb.game;

namespace bjeb.net
{
	public interface IServer
	{
	    void onSetup(Screen screen);

	    List<Window> windows
	    {
			get;
	    }

		game.Vessel vessel
		{
			get;
		}

		game.Universe universe
		{
			get;
		}

		DebugSettings settings
		{
			get;
		}

	    void onUpdate();
		void onControl(game.FlightControl control);
	}

	public partial class Protocol
	{
	    public static DebugSettings requestSetup(Connection connection, Screen screen)
	    {
			connection.stream.write("setup");
			screen.serialize(connection.stream);
			connection.flush();

			DebugSettings settings = new DebugSettings();
			settings.deserialize(connection.stream);

			return settings;
	    }

	    public static DebugSettings requestSetup(Connection connection, int width, int height)
	    {
			return requestSetup(connection, new Screen(width, height));
	    }

	    public static List<Window> requestGui(Connection connection, Window.OnUpdate onUpdate)
	    {
			connection.stream.write("gui");
			connection.flush();

			List<Window> windows = new List<gui.Window>();

			int size = connection.stream.readInt();

			for(int i=0;i<size;i++)
			{
				Window newWindow = new Window();
				newWindow.deserialize(connection.stream);

				newWindow.onUpdate = onUpdate;

				windows.Add(newWindow);
			}

			return windows;
	    }

	    public static void requestGuiUpdate(List<Window> windows, Connection connection)
	    {
			connection.stream.write("guiUpdate");
			connection.stream.write(windows.Count);

			foreach(var window in windows)
				window.serializeState(connection.stream);
				
			connection.flush();
	    }

	    public static void requestWindowUpdate(Window window, Connection connection)
	    {
			connection.stream.write("guiWindowUpdate");

			connection.stream.write(window.id);
			window.views.serializeState(connection.stream);

			connection.flush();
	    }

	    public static void requestUpdate(game.Universe universe, game.Vessel vessel, Connection connection)
	    {
			connection.stream.write("update");
			
			universe.serialize(connection.stream);
			vessel.serialize(connection.stream);

			connection.flush();
	    }

		public static void requestControl(game.Vessel vessel, game.FlightControl c, Connection connection)
		{
			connection.stream.write("control");
			vessel.serialize(connection.stream);
			c.serialize(connection.stream);
			connection.flush();

			c.deserialize(connection.stream);
		}
	}
	
	public partial class Protocol
	{
		public static void handle(Connection connection, IServer server)
	    {
			switch(connection.stream.readString())
			{
			case "setup":
				handleSetup(connection, server);
				break;
			case "gui":
				handleGui(connection, server);
				break;
			case "guiWindowUpdate":
				handleWindowUpdate(connection, server);
				break;
			case "guiUpdate":
				handleGuiUpdate(connection, server);
				break;
			case "update":
				handleUpdate(connection, server);
				break;
			case "control":
				handleControl(connection, server);
				break;
			}
	    }

	    private static void handleSetup(Connection connection, IServer server)
	    {
			Screen screen = new Screen();
			screen.deserialize(connection.stream);

			server.onSetup(screen);

			server.settings.serialize(connection.stream);
			connection.flush();
	    }

	    private static void handleGui(Connection connection, IServer server)
	    {
			var windows = server.windows;

			connection.stream.write(windows.Count);

			foreach(var window in server.windows)
				window.serialize(connection.stream);
			connection.flush();
	    }

	    private static void handleGuiUpdate(Connection connection, IServer server)
	    {
			int nodes = connection.stream.readInt();
			var windows = server.windows;

			var windowIterator = windows.GetEnumerator();

			int i =0;
			while(i < nodes && windowIterator.MoveNext())
			{
				windowIterator.Current.deserializeState(connection.stream);
				i++;
			}
	    }

	    private static void handleWindowUpdate(Connection connection, IServer server)
	    {
			int id = connection.stream.readInt();

			foreach(var window in server.windows)
			{
				if(window.id != id)
					continue;

				window.views.deserializeState(connection.stream);
				return;
			}

			throw new System.ArgumentException();
	    }

	    private static void handleUpdate(Connection connection, IServer server)
	    {
			server.universe.deserialize(connection.stream);
			server.vessel.deserialize(connection.stream);
			server.onUpdate();
	    }

		private static void handleControl(Connection connection, IServer server)
		{
			var control = new game.FlightControl();
			server.vessel.deserialize(connection.stream);
			control.deserialize(connection.stream);

			server.onControl(control);

			control.serialize(connection.stream);
			connection.flush();
		}
	}
}