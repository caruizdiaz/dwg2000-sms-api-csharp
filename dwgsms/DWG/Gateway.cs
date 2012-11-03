// 
// Gateway.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 9/19/2012, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;

using dwgsms.API;

namespace dwgsms.DWG
{
	public unsafe class Gateway
	{
		public delegate void StatusChangedEventHandler(string gatewayIP, dwg_port_status_value[] portsStatus);
		public delegate void SMSReceivedEventHandler(string gatewayIP, string number, string text, int port, string timestamp);
		public delegate void SentSMSStatusEventHandler(string gatewayIP, string number, int port, dwg_sms_result_code status);
		
		public static event StatusChangedEventHandler StatusChanged;
		public static event SMSReceivedEventHandler SMSReceived;
		public static event SentSMSStatusEventHandler SentSMSStatus;
		
		static Hashtable _gatewayPorts	= new Hashtable();
		
		static bool _isGwConnected;
		public static bool IsGwConnected {
			get {
				return _isGwConnected;
			}
		}		
		
		static string _gwIP;
		public static string GwIP {
			get {
				return _gwIP;
			}
		}
		
		public static void SendMessage(string destination, string message, int port)
		{
			str_t number = new str_t();
			number.s = destination;
			number.len = number.s.Length;
			
			str_t msg = new str_t();
			msg.s = message;
			msg.len = msg.s.Length;
									
			smsgw.dwg_send_sms(ref number, ref msg, port);			
		}
		
		public static void StopListener()
		{
			smsgw.dwg_stop_server();
		}
		
		public static void StartListener(int port)
		{
			dwg_message_callback_t callback = new dwg_message_callback_t();
			IntPtr ptr =  Marshal.AllocHGlobal(Marshal.SizeOf(callback));
			
			callback.status_callback = Gateway.smsgw_status_changed;
			callback.msg_response_callback = Gateway.smsgw_response;
			callback.msg_sms_recv_callback = Gateway.smsgw_sms_received;
			
			Marshal.StructureToPtr(callback, ptr, true);
			
			smsgw.dwg_start_server(port, ptr);			
		}
		
		static void smsgw_sms_received(ref str_t gw_ip, ref dwg_sms_received newsms)
		{	
			string smsContent;
			
			if (newsms.message.len == 0 || newsms.message.s == null)
				smsContent = string.Empty;
			else
				smsContent = newsms.message.s;
			
			if (Gateway.SMSReceived != null)
				Gateway.SMSReceived(gw_ip.s, newsms.str_number.s, smsContent, newsms.port, new string(newsms.timestamp));
		}
 
		static unsafe void smsgw_response(ref str_t gw_ip, ref dwg_sms_res sms_status)
		{				
			//Int64 id = (Int64) Marshal.PtrToStructure(gw_id, typeof(Int64));					
			
			if (Gateway.SentSMSStatus != null)
				Gateway.SentSMSStatus(gw_ip.s, sms_status.str_number.s, sms_status.port, (dwg_sms_result_code) sms_status.result);			
		}
		
		static unsafe void smsgw_status_changed(ref str_t gw_ip, IntPtr statusPtr)
		{
			dwg_ports_status *status = (dwg_ports_status *) statusPtr;	
			dwg_port_status_value[] ports_status	= new dwg_port_status_value[status->size];
			
			_isGwConnected	= true;
			
			for(int i = 0; i < status->size; i++)
				ports_status[i]	= (dwg_port_status_value) status->status_array[i].status;
						
			_gwIP	= gw_ip.s;
			
			if (Gateway.StatusChanged != null)
				Gateway.StatusChanged(gw_ip.s, ports_status);
			
			
			if (_gatewayPorts.ContainsKey(gw_ip.s))
				_gatewayPorts[gw_ip.s] = ports_status;
			else
				_gatewayPorts.Add(gw_ip.s, ports_status);
			
		}
		
		public static bool IsPortReady( int port)
		{
			return IsPortReady(_gwIP, port);
		}
		
		public static bool IsPortReady(string gwIP, int port)
		{
			dwg_port_status_value[] ports_status;
			
			if (!_gatewayPorts.ContainsKey(gwIP))
				return false;
				
			ports_status	= (dwg_port_status_value[]) _gatewayPorts[gwIP];
			
			if (ports_status == null)
				return false;
			
			if (port >= ports_status.Length)
				return false;
			
			return ports_status[port] == dwg_port_status_value.WORKS;
		}
		
		
	}
}

