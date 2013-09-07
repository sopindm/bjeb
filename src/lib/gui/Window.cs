#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
    [Serializable("window")]
    public class Window: View
    {
	private ViewContainer _views;

	public ViewContainer views
	{
	    get
	    {
		return _views;
	    }
	}

	private static int _nextId = 92142814;
	private static int nextId()
	{
	    _nextId++;
	    return _nextId - 1;
	}

	public int id
	{
	    get;
	    private set;
	}

	override protected Style defaultStyle
	{
	    get
	    {
		return Style.Window;
	    }
	}

	public Window()
	{
	    title = "";

	    id = nextId();
	    draggable = false;

	    _views = new ViewContainer(this);
	}

	public string title
	{
	    get;
	    set;
	}

	public bool draggable
	{
	    get;
	    set;
	}

	public delegate void OnUpdate(Window window);

	public OnUpdate onUpdate
	{
	    get;
	    set;
	}

	public override void draw()
	{
	    if(!isShowing)
		return;
#if UNITY
	    area.rectangle = GUILayout.Window( id, area.rectangle, drawContext, title, unityStyle(), area.layoutOptions());
#endif
	}

	private bool _isUpdated = false;

	public bool isUpdated
	{
		get
		{
			return _isUpdated;
		}
	}

	override protected void doUpdate()
	{
		_isUpdated = true;
	}

	private void drawContext(int id)
	{
#if UNITY
	    _views.draw();

	    if(draggable)
		GUI.DragWindow();

		if(_isUpdated)
		{
			onUpdate(this);
			_isUpdated = false;
		}
#endif
	}

	override protected void doSerialize(Stream stream)
	{
	    base.doSerialize(stream);

	    stream.write(id);
	    stream.write(title);

	    area.serialize(stream);

	    stream.write(draggable);

	    _views.serialize(stream);
	}

	override protected void doSerializeState(Stream stream)
	{
	    area.serialize(stream);
	}

	override protected void doDeserialize(Stream stream)
	{
	    base.doDeserialize(stream);

	    id= stream.readInt();
	    title = stream.readString();

	    area.deserialize(stream);

	    draggable = stream.readBool();

	    _views.deserialize(stream);
	}

	override protected void doDeserializeState(Stream stream)
	{
	    area.deserialize(stream);
	}
    }
}