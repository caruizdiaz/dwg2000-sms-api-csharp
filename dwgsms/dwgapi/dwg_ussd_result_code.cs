// 
// dwg_ussd_result_code.cs by Carlos Ruiz Díaz
// carlos.ruizdiaz@gmail.com
// 2/22/2013, Asunción - Paraguay
// 
using System;

namespace dwgsms.API
{
	public enum dwg_ussd_result_code
	{
		NoFurtherActionRequired,
		FutherActionRequired,
		TerminatedByNetwork,
		OperationNotSupported = 4
		//Unknown
	}
}

