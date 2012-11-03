// 
// Main.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 9/18/2012, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;
using System.Security;

using dwgsms.API;
using dwgsms.DWG;

namespace dwgsms
{
	class MainClass
	{	
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");								
			
			Gateway.StatusChanged += HandleGatewayStatusChanged;
			Gateway.SMSReceived += HandleGatewaySMSReceived;
			Gateway.StartListener(7008);
			
			
			Console.ReadLine();
			
			Gateway.SendMessage("595981146623", "hola mundo desde PY");
			
			Console.ReadLine();								
			
			Gateway.StopListener();
			
			Console.WriteLine ("Listener stopped!");						
		
			//
			
		}

		static void HandleGatewaySMSReceived (long GatewayID, dwg_sms_received SMS)
		{
			
		}

		static void HandleGatewayStatusChanged (long GatewayID, dwg_port_status_value[] PortsStatus)
		{
			Console.WriteLine("status has changed for gw ID {0}", GatewayID);
			
			for(int i = 0; i < PortsStatus.Length; i++)
				Console.WriteLine("Port#{0}: {1}", i, Enum.GetName(typeof(dwg_port_status_value), (dwg_port_status_value) PortsStatus[i]));
			
		}
		/*
		static unsafe void sms_rcv(IntPtr ptr)
		{
			Console.WriteLine("rcv");
		}
		
		static unsafe void gw_response(IntPtr ptr)
		{
			Console.WriteLine("response");
		}
		*/
		static unsafe void status_changed(IntPtr ptr)
		{
			Console.WriteLine("status");
		
			
			dwg_ports_status *sts = (dwg_ports_status *) ptr;
			
			Console.WriteLine(sts->size);
			
			
			//var structSize	= Marshal.SizeOf(typeof(dwg_ports_status))
			
		}
		
		
	}
}