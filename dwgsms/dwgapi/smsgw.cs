// 
// dwg.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 9/19/2012, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;

namespace dwgsms.API
{
	public class smsgw
	{
		[DllImport("libdwgsms.pub.so", CharSet = CharSet.Ansi)]
		public static extern int dwg_start_server(int port, IntPtr callbacks);
		//public static extern int dwg_start_server(int port, ref dwg_message_callback_t callbacks);
		
		[DllImport("libdwgsms.pub.so", CharSet = CharSet.Ansi)]
		public static extern int dwg_stop_server();
		
		[DllImport("libdwgsms.pub.so", CharSet = CharSet.Ansi)]
		public static extern int dwg_send_sms(ref str_t destination, ref str_t message, int port);
		
/*		[DllImport("libdwgsms.pub.so")]
		public static extern int dwg_send_sms(ref str_t destination, ref str_t message);
				 */
	}
}

