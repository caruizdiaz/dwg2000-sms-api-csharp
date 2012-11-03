// 
// dwg_ports_status.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 9/19/2012, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;

namespace dwgsms.API
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct dwg_ports_status
	{
		public int size;			
//		[MarshalAs(UnmanagedType.SafeArray)]
		public dwg_port_status* status_array;		
	}	
}

