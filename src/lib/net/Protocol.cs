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

		    //newWindow.onDrawFinished = (w => requestWindowUpdate(w, connection));

		    windows.Add(newWindow);
		}

		return windows;
	    }
/*
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
/*
		case "guiUpdate":
		    return handleGuiUpdate(request, server);
		case "guiWindowUpdate":
		    return handleWindowUpdate(request, server);
		case "update":
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

/*
	    private static Xml handleGuiUpdate(Xml request, IServer server)

		{
		    var nodes = request.root.nodes("window");
		    var windows = server.windows;

		    var nodeIterator = nodes.GetEnumerator();
		    var windowIterator = windows.GetEnumerator();

		    while(nodeIterator.MoveNext() && windowIterator.MoveNext())
		    {
			if(windowIterator.Current.id != nodeIterator.Current.attribute("id").getInt())
			    throw new System.ArgumentException();

			windowIterator.Current.deserializeState(nodeIterator.Current);
		    }

		    return null;
		}

	    private static Xml handleWindowUpdate(Xml request, IServer server)
		{
		    foreach(var window in server.windows)
		    {
			if(window.id != request.root.attribute("id").getInt())
			    continue;

			window.views.deserializeState(request.root);
			return null;
		    }

		    throw new System.ArgumentException();
		}

	    private static Xml handleUpdate(Xml request, IServer server)
		{
		    var vessel = new game.Vessel();
		    vessel.deserialize(request.root.node("vessel"));

		    server.onUpdate(vessel);
			
		    return null;
		}*/
	}
}