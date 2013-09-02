// 
// Main.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 11/2/2012, Asunción - Paraguay
// 
using System;

using dwgsms.DWG;
using dwgsms.API;

namespace dinstarsmsclient
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			int port		= 0;						
			
			if (args.Length != 1)
					throw new ArgumentException("Invalid command line parameters.");
			
			if (!int.TryParse(args[0], out port))
				throw new Exception("<port> must be numerical");
			
			Gateway.StatusChanged += HandleGatewayStatusChanged;
			Gateway.SMSReceived += HandleGatewaySMSReceived;
			Gateway.SentSMSStatus += HandleGatewaySentSMSStatus;
			Gateway.USSDReceived += HandleGatewayUSSDReceived;
			
			Console.WriteLine("Starting server at port {0}... waiting for connection", port);
			
			Gateway.StartListener(port);
			
			
			string number	= "+595981146623";
			string text		= "Hello World";
			int sendingPort	= 0;
			
			for(;;)
			{
				Console.ReadLine();
				
				if (!Gateway.IsGwConnected)
				{
					Console.WriteLine("GW is not connected. Please wait...");
					continue;
				}
				
				sendingPort = new Random().Next(0, 8);
				
				if (!Gateway.IsPortReady(sendingPort))
				{
					Console.WriteLine("Port #{0} is not ready for sending sms through it. Check status.", sendingPort);
					continue;
				}
				
				Console.WriteLine("Sending sms to {0}, using port {1}", number, sendingPort);
				
				Gateway.SendMessage(number, text, sendingPort);
			}
		}

		static void HandleGatewayUSSDReceived (string gatewayIP, int port, dwg_ussd_result_code status, string message)
		{
			Console.WriteLine("RCVD USSD: gw {0} | port {1} | text '{3}'", gatewayIP, port, message);		
		}
		
		static void HandleGatewaySentSMSStatus (string gatewayIP, string number, int port, dwg_sms_result_code status)
		{			
			Console.WriteLine("SENT SMS: gw {0} | number {1} | port {2} | status {3}", gatewayIP, number, port, Enum.GetName(typeof(dwg_sms_result_code), status));		
		}

		static void HandleGatewaySMSReceived(string gatewayIP, string number, string text, int port, string timestamp)
		{			
			if (text == null)
				text = string.Empty;
			
			Console.WriteLine("RCVD SMS: gw {0} | number {1} | port {2} | text '{3}'", gatewayIP, number, port, text);		
		}

		static void HandleGatewayStatusChanged (string gatewayIP, dwg_port_status_value[] PortsStatus)
		{
			Console.WriteLine("STS CHANGED: gw {0}", gatewayIP);		
			
			for(int i = 0; i < PortsStatus.Length; i++)
				Console.WriteLine("Port#{0}: {1}", i, Enum.GetName(typeof(dwg_port_status_value), (dwg_port_status_value) PortsStatus[i]));
		}
	}
}
