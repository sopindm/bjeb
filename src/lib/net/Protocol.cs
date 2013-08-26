using System.Collections.Generic;
using bjeb.gui;
//using bjeb.game;

namespace bjeb.net
{
	public interface IServer
	{
	    void onSetup(Screen screen);

	    List<Window> windows
	    {
		get;
	    }

//	    void onUpdate(game.Vessel vessel);
	}

	public class Protocol
	{
	    public static void requestSetup(Connection connection, Screen screen)
	    {
		connection.stream.write("setup");
		screen.serialize(connection.stream);
	    }

	    public static void requestSetup(Connection connection, int width, int height)
	    {
		requestSetup(connection, new Screen(width, height));
	    }

	    public static List<Window> requestGui(Connection connection)
	    {
		connection.stream.write("gui");

		List<Window> windows = new List<gui.Window>();

		int size = connection.stream.readInt();

		for(int i=0;i<size;i++)
		{
		    Window newWindow = new Window();
		    newWindow.deserialize(connection.stream);

		    newWindow.onDrawFinished = (w => requestWindowUpdate(w, connection));

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
	    }

	    public static void requestWindowUpdate(Window window, Connection connection)
	    {
		connection.stream.write("guiWindowUpdate");

		connection.stream.write(window.id);
		window.views.serializeState(connection.stream);
	    }

/*
	    public static void requestUpdate(game.Vessel vessel, Connection connection)
		{
		    Xml request = Xml.createMessage("update");
		    vessel.serialize(request.root);
		    request.tryWrite(connection);
		}*/
		
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
/*		case "update":
		    return handleUpdate(request, server);*/
		}
	    }

	    private static void handleSetup(Connection connection, IServer server)
	    {
		Screen screen = new Screen();
		screen.deserialize(connection.stream);

		server.onSetup(screen);
	    }

	    private static void handleGui(Connection connection, IServer server)
	    {
		var windows = server.windows;

		connection.stream.write(windows.Count);

		foreach(var window in server.windows)
		    window.serialize(connection.stream);
	    }

	    private static Xml handleGuiUpdate(Connection connection, IServer server)
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

		return null;
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
/*
	    private static Xml handleUpdate(Xml request, IServer server)
		{
		    var vessel = new game.Vessel();
		    vessel.deserialize(request.root.node("vessel"));

		    server.onUpdate(vessel);
			
		    return null;
		}*/
	}
}