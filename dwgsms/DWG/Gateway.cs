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
		public delegate void USSDReceivedEventHandler(string gatewayIP, int port, dwg_ussd_result_code status, string message);
		
		private static readonly status_changed_event_handler statusChanged = Gateway.smsgw_status_changed;
		private static readonly sms_received_event_handler smsRecv = Gateway.smsgw_sms_received;
		private static readonly sms_reponse_event_handler sendSmsSts = Gateway.smsgw_response;
		private static readonly ussd_received_event_handler ussdRcv = Gateway.smsgw_ussd_received;
		
		/// <summary>
		/// Occurs when gateway's status changed
		/// </summary>
		public static event StatusChangedEventHandler StatusChanged;
		
		/// <summary>
		/// Occurs when a SMS is received.
		/// </summary>
		public static event SMSReceivedEventHandler SMSReceived;
		
		/// <summary>
		/// Occurs when an USSD received.
		/// </summary>
		public static event USSDReceivedEventHandler USSDReceived;
		
		/// <summary>
		/// Occurs when a sent SMS is received by the gateway
		/// </summary>
		public static event SentSMSStatusEventHandler SentSMSStatus;
		
		public class Port
		{
			public Port()
			{
				NegativeHits = 0;
				Banned = false;			
			}
			
			public dwg_port_status_value Status = dwg_port_status_value.UNKNOWN;
			public int NegativeHits;
			public bool Banned;
			
			/// <summary>
			/// Put negatives hits counter to zero.
			/// </summary>
			public void ResetCounter()
			{
				NegativeHits = 0;
			}
			
			/// <summary>
			/// Ban this port from sending message. When banned, it cannot be used to send any sms/ussd
			/// </summary>
			public void Ban()
			{
				TimedBan(System.Threading.Timeout.Infinite);
			}
			
			/// <summary>
			/// Ban this port for a specific amount of time
			/// </summary>
			/// <param name='timeout'>
			/// the banning time
			/// </param>
			public void TimedBan(int timeout)
			{
				Banned	= true;				
				
				new System.Threading.Thread(delegate() 
				{
					if (timeout == System.Threading.Timeout.Infinite)
						return;
					
					System.Threading.Thread.Sleep(timeout * 1000);
					Console.WriteLine("==> unbanning port <==");
					
					ResetCounter();
					UnBan();
					
				}).Start();
			}
			
			public void UnBan()
			{
				Banned	= false;
			}
		}
		
		static Hashtable _gatewayPorts	= new Hashtable();
		static int _roundRobinPortIndex	= 0;
		
		static bool _isGwConnected;
		/// <summary>
		/// Checks if the gateway is connected or not to this server
		/// </summary>
		/// <value>
		/// <c>true</c> if it is connected; otherwise, <c>false</c>.
		/// </value>
		public static bool IsGwConnected {
			get {
				return _isGwConnected;
			}
		}		
		
		static string _gwIP;
		/// <summary>
		/// Gets the gateway's IP.
		/// </summary>
		/// <value>
		/// The IP.
		/// </value>
		public static string GwIP {
			get {
				return _gwIP;
			}
		}
		
		static int _numberOfPorts	= -1;
		/// <summary>
		/// Gets the number of ports.
		/// </summary>
		/// <value>
		/// The number of ports.
		/// </value>
		public static int NumberOfPorts
		{
			get {
				return _numberOfPorts;				
			}
		}
		
		/// <summary>
		/// Gets the number of elegible ports. This is the number of ports that are not banned and with a registerd SIMCARD 
		/// </summary>
		/// <value>
		/// The number of elegible ports.
		/// </value>
		public static int NumberOfElegiblePorts
		{
			get 
			{		
				if (_ports == null)
					return 0;
				
				int count = 0;
				
				foreach(Port port in _ports)
					if (!port.Banned && port.Status == dwg_port_status_value.WORKS)
						count++;
				
				return count;			
			}
		}
		
		static Port[] _ports;
		/// <summary>
		/// Sends a SMS.
		/// </summary>
		/// <returns>
		/// True if the message was successfully queued.
		/// </returns>
		/// <param name='destination'>
		/// Destination number
		/// </param>
		/// <param name='message'>
		/// SMS content
		/// </param>
		/// <param name='selectedPort'>
		/// Index of the port used to send the message
		/// </param>
		public static bool SendMessage(string destination, string message, ref int selectedPort)
		{
			int current = _roundRobinPortIndex;			
			selectedPort = -1;	
			
			for(int index = _roundRobinPortIndex + 1; index != current; index++)
			{				
				if (index > _numberOfPorts - 1)
					index = 0;
				
				if (IsPortReady(index) && !IsPortBanned(index))
				{
					selectedPort = index;
					break;
				}				
			}
			
			if (selectedPort == -1 && (IsPortReady(current) && !IsPortBanned(current)))
				selectedPort = current;
			
			if (selectedPort == -1)
				return false;
			
			_roundRobinPortIndex	= selectedPort;		
			
			SendMessage(destination, message, selectedPort);
			
			return true;
		}
		
		/// <summary>
		/// Sends a USSD request.
		/// </summary>
		/// <param name='requestCode'>
		/// USSD request code
		/// </param>
		/// <param name='port'>
		/// Gateway's port to use
		/// </param>
		/// <exception cref='Exception'>
		/// If the gateway is not connected, an exception will be thrown
		/// </exception>
		public static void SendUSSDMessage(string requestCode, int port)
		{
			if (!Gateway.IsGwConnected)
				throw new Exception("Gateway is not connected");
			
			str_t number = new str_t();
			number.s =  Marshal.StringToHGlobalAnsi(requestCode);
			number.len = requestCode.Length;
					
			smsgw.dwg_send_ussd(ref number, port);
		}
		
		/// <summary>
		/// Sends a SMS.
		/// </summary>
		/// <returns>
		/// True if the message was successfully queued.
		/// </returns>
		/// <param name='destination'>
		/// Destination number
		/// </param>
		/// <param name='message'>
		/// SMS content
		/// </param>
		/// <param name='selectedPort'>
		/// Index of the port from which the message will be sent
		/// </param>
		public static void SendMessage(string destination, string message, int port)
		{
			if (!Gateway.IsGwConnected)
				throw new Exception("Gateway is not connected");
							
			str_t number = new str_t();
			number.s =  Marshal.StringToHGlobalAnsi(destination);
			number.len = destination.Length;
			
			str_t msg = new str_t();
			msg.s = Marshal.StringToHGlobalAnsi(message);;
			msg.len = message.Length;
									
			smsgw.dwg_send_sms(ref number, ref msg, port);			
		}
		
		/// <summary>
		/// Stops this messaging server.
		/// </summary>
		public static void StopListener()
		{
			smsgw.dwg_stop_server();
		}
		
		/// <summary>
		/// Starts this messaging server
		/// </summary>
		/// <param name='port'>
		/// Network port to bind
		/// </param>
		public static void StartListener(int port)
		{
			StartListener(port, true);
		}
		
		public static void StartListener(int port, bool autoBan)
		{
			dwg_message_callback_t callback = new dwg_message_callback_t();
			IntPtr ptr =  Marshal.AllocHGlobal(Marshal.SizeOf(callback));
			
			/*callback.status_callback = Gateway.smsgw_status_changed;
			callback.msg_response_callback = Gateway.smsgw_response;
			callback.msg_sms_recv_callback = Gateway.smsgw_sms_received;
			callback.msg_ussd_recv_callback = Gateway.smsgw_ussd_received;*/
			
			callback.status_callback = statusChanged;
			callback.msg_response_callback = sendSmsSts;
			callback.msg_sms_recv_callback = smsRecv;
			callback.msg_ussd_recv_callback = ussdRcv;
			
			Marshal.StructureToPtr(callback, ptr, true);
			
			smsgw.dwg_start_server(port, ptr);			
		}
		
		static string strToString(str_t str)
		{
			return Marshal.PtrToStringAnsi(str.s, str.len);
		}
		
		static void smsgw_ussd_received(IntPtr gwIpPtr, ref dwg_ussd_received ussd)
		{
			str_t gwIP = (str_t) Marshal.PtrToStructure(gwIpPtr, typeof(str_t));			
			
			string ussdContent;
			
			if (ussd.message.len == 0)
				ussdContent = string.Empty;
			else
				ussdContent = strToString(ussd.message);
			
			if (Gateway.USSDReceived != null)
				Gateway.USSDReceived(strToString(gwIP),
									 ussd.port,
									 (dwg_ussd_result_code) ussd.type,
									 ussdContent);
		}
		
		static void smsgw_sms_received(IntPtr gwIpPtr, ref dwg_sms_received newsms)
		{	
			str_t gwIP = (str_t) Marshal.PtrToStructure(gwIpPtr, typeof(str_t));			
			
			string smsContent;
			
			if (newsms.message.len == 0)
				smsContent = string.Empty;
			else
				smsContent = strToString(newsms.message);
			
			if (Gateway.SMSReceived != null)
				Gateway.SMSReceived(strToString(gwIP), 
									strToString(newsms.str_number), 
									smsContent, 
									newsms.port, 
									new string(newsms.timestamp));
		}
 
		static unsafe void smsgw_response(IntPtr gwIpPtr, ref dwg_sms_res sms_status)
		{				
			//Int64 id = (Int64) Marshal.PtrToStructure(gw_id, typeof(Int64));	
			str_t gwIP = (str_t) Marshal.PtrToStructure(gwIpPtr, typeof(str_t));		
			
			if (Gateway.SentSMSStatus != null)
				Gateway.SentSMSStatus(strToString(gwIP), 
										strToString(sms_status.str_number), 
										sms_status.port, 
										(dwg_sms_result_code) sms_status.result);			
		}
		
		static unsafe void smsgw_status_changed(IntPtr gwIpPtr, IntPtr statusPtr)
		{
			str_t gwIP = (str_t) Marshal.PtrToStructure(gwIpPtr, typeof(str_t));		
			
			string gw								= strToString(gwIP);
			dwg_ports_status *status = (dwg_ports_status *) statusPtr;	
			dwg_port_status_value[] ports_status	= new dwg_port_status_value[status->size];
			_ports									= new Port[status->size];
			_isGwConnected	= true;
			
			for(int i = 0; i < status->size; i++)
			{
				ports_status[i]	= (dwg_port_status_value) status->status_array[i].status;
				
				_ports[i] = new Port();
				_ports[i].Status	= ports_status[i];
			}
			
			_gwIP	= strToString(gwIP);
			_numberOfPorts	= ports_status.Length;
			
			if (Gateway.StatusChanged != null)
				Gateway.StatusChanged(gw, ports_status);
			
			if (_gatewayPorts.ContainsKey(gw))
				_gatewayPorts[gw] = ports_status;
			else
				_gatewayPorts.Add(gw, ports_status);
		}
		
		/// <summary>
		/// Checks if the port index supplied can be used to send a message
		/// </summary>
		/// <returns>
		/// <c>true</c> if it's ready; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='port'>
		/// Index of the port
		/// </param>
		public static bool IsPortReady(int port)
		{
			return IsPortReady(_gwIP, port);
		}
		
		/// <summary>
		/// Checks if the port index supplied for the gateway can be used to send a message
		/// </summary>
		/// <returns>
		/// <c>true</c> if it's ready; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='port'>
		/// Index of the port
		/// </param>
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
			
			return ports_status[port] == dwg_port_status_value.WORKS && !Gateway.IsPortBanned(port);
		}
		
		/// <summary>
		/// Gets the port instance based on its index.
		/// </summary>
		/// <returns>		
		/// </returns>
		/// <param name='port'>
		/// Port instance
		/// </param>		
		public static Port GetPort(int port)
		{
			if (!IsGwConnected)
				throw new Exception("Gw is not connected");
			
			if (port < 0 || port > _ports.Length - 1)
				throw new Exception("Port out of range");
			
			return _ports[port];
		}
			
		/*public static bool BanPort(int port)
		{
			return AssignPort(port, true);
		}*/
		
		/*static bool AssignPort(int port, bool val)
		{
			if (!IsGwConnected)
				throw new Exception("Gw is not connected");
			
			if (port < 0 || port > _ports.Length - 1)
				throw new Exception("Port out of range");
			
			_ports[port].Banned = val;			
			
			return true;
		} */
		
		static bool IsPortBanned(int port)
		{
			if (!IsGwConnected)
				throw new Exception("Gw is not connected");
			
			if (port < 0 || port > _ports.Length - 1)
				throw new Exception("Port out of range");
			
			return _ports[port].Banned;
		}
		
	}
}

