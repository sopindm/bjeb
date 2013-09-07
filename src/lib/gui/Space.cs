#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[Serializable("space")]
	public class Space: LayoutView
	{
		private float? _size;

		public Space()
		{
			_size = null;
		}

		public Space(float size)
		{
			_size = size;
		}

		override protected Style defaultStyle
		{
			get
			{
				return Style.Default;
			}
		}

		override protected void drawLayout()
		{
#if UNITY
			if(_size == null)
				GUILayout.FlexibleSpace();
			else
				GUILayout.Space(_size.Value);
#endif
		}

		override protected void drawFixed()
		{
		}

		override protected void doSerialize(Stream stream)
		{
			base.doSerialize(stream);
			stream.write(_size);
		}

		override protected void doDeserialize(Stream stream)
		{
			base.doDeserialize(stream);
			_size = stream.tryReadFloat();
		}
	}
}