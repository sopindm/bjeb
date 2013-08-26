using System.Collections.Generic;
using bjeb.gui;

namespace bjeb
{
	class ASASModule: Module
	{
		public ASASModule(Computer computer): base(computer)
		{
		}

		override protected void onSetup(Screen screen)
		{
			window.area.set(0, 200, 300, 200);
			content.views.clear();
		}

		override protected void onUpdate()
		{
		}

		override public string name
		{
			get
			{
				return "ASAS";
			}
		}
	}
}