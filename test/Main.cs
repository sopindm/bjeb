using System;
using bjeb.net;
using bjeb.gui;

namespace bjeb.test
{
    class MainClass
    {
		public static void update(Connection connection)
        {
            Screen screen = new Screen();

            screen.width = 1000;
            screen.height = 500;

            Xml request = new Xml("msg");

			request.root.attribute("type").set("gui");

            screen.serialize(request.root);

            request.write(connection);

            Xml response = Xml.read(connection);

            Window window = new Window();
            window.deserialize(response.root.node("window"));

			window.x--;
			window.y++;
			window.width++;
			window.height--;

            Console.WriteLine("Window ID: " + window.id + " X: " + window.x + "Y: " + window.y + " Width: " + window.width + " Height: " + window.height);
			Console.WriteLine("Button: " + window.button.text);
			Console.WriteLine();

			request = new Xml("msg");

			request.root.attribute("type").set("guiUpdate");

			window.serializeState(request.root);
			request.write(connection);
			
			request = new net.Xml("msg");
			request.root.attribute("type").set("guiWindowUpdate");
			request.root.attribute("id").set(window.id);
			window.button.serializeState(request.root);

			request.write(connection);
		}

        public static void Main(string[] args)
        {
			Client client = new Client("127.0.0.1", 4400);

            while (true)
            {
                if(!client.execute(() => 
						{
							update(client.connection);
							System.Threading.Thread.Sleep(1000);
						}))
				{
					Console.WriteLine("No connection");
					System.Threading.Thread.Sleep(1000);
				}
			}
		}
    }
}
