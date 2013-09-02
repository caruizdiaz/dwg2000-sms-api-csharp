// 
// dwg_message_callback_t.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 9/19/2012, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;

namespace dwgsms.API
{
/*	public delegate void status_changed_event_handler(ref dwg_ports_status status);
	public delegate void sms_reponse_event_handler(ref dwg_sms_res response);
	public delegate void sms_received_event_handler(ref dwg_sms_received rcv);
		 */
	/*public delegate void sms_received_event_handler(ref str_t gw_ip, ref dwg_sms_received rcv);
	public delegate void status_changed_event_handler(ref str_t gw_ip, IntPtr status);
	public delegate void sms_reponse_event_handler(ref str_t gw_ip, ref dwg_sms_res response);
	*/
	
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void sms_received_event_handler(IntPtr gw_ip, ref dwg_sms_received rcv);
	
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void status_changed_event_handler(IntPtr gw_ip, IntPtr status);
    
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void sms_reponse_event_handler(IntPtr gw_ip, ref dwg_sms_res response);
	
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ussd_received_event_handler(IntPtr gw_ip, ref dwg_ussd_received rcv);
	
	
	//public delegate void sms_received_event_handler(IntPtr rcv);
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct dwg_message_callback_t
	{
		public status_changed_event_handler status_callback;
		public sms_reponse_event_handler msg_response_callback;
		public sms_received_event_handler msg_sms_recv_callback;
		public ussd_received_event_handler msg_ussd_recv_callback;
	}
}

