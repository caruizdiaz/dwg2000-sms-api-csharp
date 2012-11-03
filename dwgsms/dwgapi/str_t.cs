// 
// str_t.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 9/19/2012, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;

namespace dwgsms.API
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct str_t
	{
		[MarshalAs(UnmanagedType.LPTStr)]
		public string s;
		public int len;
	}
}

