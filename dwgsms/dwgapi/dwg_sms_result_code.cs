// 
// dwg_sms_result_code.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 9/19/2012, Asunción - Paraguay
// 
using System;
using System.Runtime.InteropServices;

namespace dwgsms.API
{
	public enum dwg_sms_result_code
	{
		SMS_RC_SUCCEED = 0,
		SMS_RC_FAIL,
		SMS_RC_TIMEOUT,
		SMS_RC_BAD_REQUEST,
		SMS_RC_PORT_UNAVAILABLE,
		SMS_RC_PARTIAL_SUCCEED,
		SMS_RC_OTHER_ERROR = 255
	}	
}

