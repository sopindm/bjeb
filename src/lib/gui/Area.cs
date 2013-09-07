using System.Collections.Generic;
#if UNITY
using UnityEngine;
#endif

namespace bjeb.gui
{
    [net.Serializable("area")]
    public class Area: net.Serializable
    {
	public float? x
	{
	    get;
	    set;
	}

	public float? y
	{
	    get;
	    set;
	}

	public float? width
	{
	    get;
	    set;
	}

	public float? height
	{
	    get;
	    set;
	}

	public float? minWidth
	{
	    get;
	    set;
	}

	public float? minHeight
	{
	    get;
	    set;
	}

	public float? maxWidth
	{
	    get;
	    set;
	}

	public float? maxHeight
	{
	    get;
	    set;
	}

	public bool? widthExpandable
	{
	    get;
	    set;
	}

	public bool? heightExpandable
	{
	    get;
	    set;
	}

	public Area()
	{
	}

	public Area(float x, float y, float width, float height)
	{
	    set(x, y, width, height);
	}

	public void set(float x, float y, float width, float height)
	{
	    this.x = x;
	    this.y = y;
	    this.width = width;
	    this.height = height;
	}

	public bool isSet()
	{
	    return x != null && y != null && width != null && height != null;
	}

#if UNITY
	public Rect rectangle
	{
	    get
	    {
		return new Rect(x.Value, y.Value, width.Value, height.Value);
	    }
	    set
	    {
		x = value.x;
		y = value.y;
		width = value.width;
		height = value.height;
	    }
	}

	public GUILayoutOption[] layoutOptions()
	{
	    List<GUILayoutOption> options = new List<GUILayoutOption>();

	    if(width != null)
		options.Add(GUILayout.Width(width.Value));

	    if(height != null)
		options.Add(GUILayout.Height(height.Value));
			
	    if(minWidth != null)
		options.Add(GUILayout.MinWidth(minWidth.Value));

	    if(minHeight != null)
		options.Add(GUILayout.MinHeight(minHeight.Value));
			
	    if(maxWidth != null)
		options.Add(GUILayout.MaxWidth(maxWidth.Value));

	    if(maxHeight != null)
		options.Add(GUILayout.MaxHeight(maxHeight.Value));

	    if(widthExpandable != null)
		options.Add(GUILayout.ExpandWidth(widthExpandable.Value));
			
	    if(heightExpandable != null)
		options.Add(GUILayout.ExpandHeight(heightExpandable.Value));
			
	    return options.ToArray();
	}
#endif	

	override protected void doSerialize(net.Stream stream)
	{
	    stream.write(x);
	    stream.write(y);

	    stream.write(width);
	    stream.write(height);

	    stream.write(minWidth);
	    stream.write(minHeight);

	    stream.write(maxWidth);
	    stream.write(maxHeight);

	    stream.write(widthExpandable);
	    stream.write(heightExpandable);
	}

	override protected void doDeserialize(net.Stream stream)
	{
	    x = stream.tryReadFloat();
	    y = stream.tryReadFloat();

	    width = stream.tryReadFloat();
	    height = stream.tryReadFloat();
	    
	    minWidth = stream.tryReadFloat();
	    minHeight = stream.tryReadFloat();

	    maxWidth = stream.tryReadFloat();
	    maxHeight = stream.tryReadFloat();

	    widthExpandable = stream.tryReadBool();
	    heightExpandable = stream.tryReadBool();
	}
    }
}