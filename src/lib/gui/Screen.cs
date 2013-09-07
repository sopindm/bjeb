using bjeb.net;

namespace bjeb.gui
{
    [Serializable("screen")]
    public class Screen: Serializable
    {
	public int width
	{
	    get;
	    set;
	}

	public int height
	{
	    get;
	    set;
	}

	public Screen(): this(0, 0)
	{
	}

	public Screen(int width, int height)
	{
	    this.width = width;
	    this.height = height;
	}

	override protected void doSerialize(Stream stream)
	{
	    stream.write(width);
	    stream.write(height);
	}

	override protected void doDeserialize(Stream stream)
	{
	    width = stream.readInt();
	    height = stream.readInt();
	}
    }
}
