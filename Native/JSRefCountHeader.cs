using System.Runtime.InteropServices;

namespace QuickJs
{
	[StructLayout(LayoutKind.Sequential)]
	public struct JSRefCountHeader
	{
		public int ref_count;
	}


}
