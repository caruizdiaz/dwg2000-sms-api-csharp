// 
// dwg_port_status.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 9/19/2012, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;

namespace dwgsms.API
{
	[StructLayout(LayoutKind.Sequential)]		
	public unsafe struct dwg_port_status
	{
		public int port;
		public int status;
	}
}

