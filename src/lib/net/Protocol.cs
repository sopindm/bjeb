using System.Collections.Generic;
using bjeb.gui;

namespace bjeb.net
{
	public interface IServer
	{
		void onSetup(Screen screen);
	}

	public class Protocol
	{
		public static void requestSetup(Connection connection, Screen screen)
		{
            Xml request = Xml.createMessage("setup");
            screen.serialize(request.root);
            request.write(connection);
		}

		public static void requestSetup(Connection connection, int width, int height)
		{
			requestSetup(connection, new Screen(width, height));
		}

		public static List<Window> requestGui(Connection connection)
		{
			Xml request = Xml.createMessage("gui");
			request.write(connection);

			Xml response = Xml.read(connection);
				
			List<Window> windows = new List<gui.Window>();

			foreach(var node in response.root.nodes("window"))
			{
				Window newWindow = new Window();
				newWindow.deserialize(node);

				newWindow.onDrawFinished = (w => requestWindowUpdate(w, connection));

				windows.Add(newWindow);
			}

			return windows;
		}

		public static void requestGuiUpdate(IEnumerable<Window> windows, Connection connection)
		{
			Xml request = Xml.createMessage("guiUpdate");

			foreach(var window in windows)
				window.serializeState(request.root);

			request.tryWrite(connection);
		}

		public static void requestWindowUpdate(Window window, Connection connection)
		{
			Xml request = Xml.createMessage("guiWindowUpdate");

			request.root.attribute("id").set(window.id);
			window.views.serializeState(request.root);

			request.tryWrite(connection);
		}

		public static void handle(Connection connection, IServer server)
		{
			Xml request = Xml.read(connection);

			switch(request.root.attribute("type").getString())
			{
			case "setup":
				handleSetup(request, connection, server);
				break;
			}
				/*
			case "gui":
				return handleGui(request, connection, server);
			case "guiUpdate":
				return handleGuiUpdate(request, connection, server);
			case "guiWindowUpdate":
				return handleWindowUpdate(request, connection, server);
				}*/
		}

		private static void handleSetup(Xml request, Connection connection, IServer server)
		{
			Screen screen = new Screen();
			screen.deserialize(request.root.node("screen"));

			server.onSetup(screen);
		}

		//private static void handleGui(Xml request, Connection connection, IServer server);
	}
}