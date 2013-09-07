#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
    [Serializable("layout")]
    public class Layout: View
    {
	private ViewContainer _views;

	public ViewContainer views
	{
	    get
	    {
		return _views;
	    }
	}
		
	public bool isVertical
	{
	    get;
	    set;
	}

	public Layout()
	{
	    isVertical = true;
	    _views = new ViewContainer(this);
	}

	override protected Style defaultStyle
	{
	    get
	    {
		return Style.Default;
	    }
	}

	public static Layout makeVertical()
	{
	    return new Layout();
	}

	public static Layout makeHorizontal()
	{
	    Layout ret = new Layout();
	    ret.isVertical = false;

	    return ret;
	}

	public override void draw()
	{
	    if(!isShowing)
		return;

#if UNITY
	    if(isVertical)
	    {
		if(style != Style.Default)
		    GUILayout.BeginVertical(unityStyle(), area.layoutOptions());
		else
		    GUILayout.BeginVertical(area.layoutOptions());
	    }
	    else
	    {
		if(style != Style.Default)
		    GUILayout.BeginHorizontal(unityStyle(), area.layoutOptions());
		else
		    GUILayout.BeginHorizontal(area.layoutOptions());
	    }

	    views.draw();

	    if(isVertical)
		GUILayout.EndVertical();
	    else
		GUILayout.EndHorizontal();
#endif
	}

	override protected void doSerialize(Stream stream)
	{
	    base.doSerialize(stream);
	    stream.write(isVertical);

	    _views.serialize(stream);
	}

	override protected void doSerializeState(Stream stream)
	{
	    _views.serializeState(stream);
	}

	override protected void doDeserialize(Stream stream)
        {
	    base.doDeserialize(stream);

	    isVertical = stream.readBool();
	    _views.deserialize(stream);
	}

	override protected void doDeserializeState(Stream stream)
	{
	    _views.deserializeState(stream);
	}
    }
}