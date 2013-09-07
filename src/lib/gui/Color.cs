using bjeb.net;

namespace bjeb.gui
{
    [Serializable("color")]
    public class Color: Serializable
    {
	public float a
	{
	    get;
	    private set;
	}

	public float r
	{
	    get;
	    private set;
	}

	public float g
	{
	    get;
	    private set;
	}

	public float b
	{
	    get;
	    private set;
	}

	public Color(): this(0, 0, 0, 0)
	{
	}

	public Color(float r, float g, float b): this(1, r, g, b)
	{
	}

	public Color(float a, float r, float g, float b)
	{
	    this.a = a;
	    this.r = r;
	    this.g = g;
	    this.b = g;
	}

	public Color(double r, double g, double b): this(1, r, g, b)
	{
	}

	public Color(double a, double r, double g, double b): this((float)a, (float)r, (float)g, (float)b)
	{
	}
	
	override protected void doSerialize(Stream stream)
	{
	    stream.write(a);
	    stream.write(r);
	    stream.write(g);
	    stream.write(b);
	}

	override protected void doDeserialize(Stream stream)
	{
	    a = stream.readFloat();
	    r = stream.readFloat();
	    g = stream.readFloat();
	    b = stream.readFloat();
	}

	public static new Color tryCreate(Stream stream)
	{
	    return (Color)Serializable.tryCreate(stream);
	}

	public static Color black
	{
	    get
	    {
		return new Color(1, 0, 0, 0);
	    }
	}

	public static Color blue
	{
	    get
	    {
		return new Color(1, 0, 0, 1);
	    }
	}

	public static Color clear
	{
	    get
	    {
		return new Color(0, 0, 0, 0);
	    }
	}

	public static Color cyan
	{
	    get
	    {
		return new Color(1, 0, 1, 1);
	    }
	}

	public static Color gray
	{
	    get
	    {
		return new Color(1, 0.5, 0.5, 0.5);
	    }
	}

	public static Color green
	{
	    get
	    {
		return new Color(1, 0, 1, 0);
	    }
	}

	public static Color magenta
	{
	    get
	    {
		return new Color(1, 1, 0, 1);
	    }
	}

	public static Color red
	{
	    get
	    {
		return new Color(1, 1, 0, 0);
	    }
	}

	public static Color white
	{
	    get
	    {
		return new Color(1, 1, 1, 1);
	    }
	}

	public static Color yellow
	{
	    get
	    {
		return new Color(1, 1, 0.92, 0.016);
	    }
	}

#if UNITY
	public UnityEngine.Color unity
	{
	    get
	    {
		return new UnityEngine.Color(r, g, b, a);
	    }
	}
#endif
    }
}