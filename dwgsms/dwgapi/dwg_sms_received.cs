// 
// dwg_sms_received.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 9/19/2012, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;

namespace dwgsms.API
{
	[StructLayout(LayoutKind.Sequential)]
	public struct dwg_sms_received
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
		public char[] number;		
		public str_t str_number;
		public int type;
		public int port;
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I1, SizeConst = 16)]
		public char[] timestamp;
		public int timezone;
		public int encoding;	
		public str_t message;
	}
}