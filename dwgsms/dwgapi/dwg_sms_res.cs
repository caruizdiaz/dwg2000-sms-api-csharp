// 
// dwg_sms_res.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 9/19/2012, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;

namespace dwgsms.API
{
	[StructLayout(LayoutKind.Sequential)]
	public struct dwg_sms_res
	{
		public int count_of_number;
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I1, SizeConst = 24)]
		public char[] number;
		public str_t str_number;
		public int port;
		public dwg_sms_result_code result;
		public int count_of_slice;
		public int succeded_slices;
	}
}

