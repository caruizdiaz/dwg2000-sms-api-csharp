// 
// dwg.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 2/21/2013, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;

namespace dwgsms.API
{	
	[StructLayout(LayoutKind.Sequential)]
	public struct dwg_ussd_received
	{
		public int port;
		public int type;
		public int encoding;	
		public str_t message;
	}
}