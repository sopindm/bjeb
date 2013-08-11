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

            screen.serialize(request.root);

            request.write(connection);

            Xml response = Xml.read(connection);

            Window window = new Window();
            window.deserialize(response.root.node("window"));

            Console.WriteLine("Window ID: " + window.id + " X: " + window.x + "Y: " + window.y + " Width: " + window.width + " Height: " + window.height);
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
