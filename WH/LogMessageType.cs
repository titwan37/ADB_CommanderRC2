/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 31.03.2016
 * Time: 18:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogMessageType.
	/// </summary>
	public static class LogMessageType
	{
		/// <summary>
		/// IsTimeoutRelevant for resetting the timeout
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
	    public static bool IsTimeoutRelevant(string command)
		{
	    	var o = StringComparison.InvariantCulture;
	    	
	    	if (string.IsNullOrEmpty(command)) 
	    		return false;
	    	else if(command.StartsWith("SET_", o))
	    		return true;
	    	else if(command.StartsWith("GET_LOG", o)||command.StartsWith("GET_FAN_LOG", o))
	    		return true;
	    	else
	    		return false;
		}
	}
}
