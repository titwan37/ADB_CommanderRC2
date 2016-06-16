/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/16/2016
 * Time: 11:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogConnectStatus.
	/// </summary>
	public class LogConnectStatus : LogBody, ILogBody
	{
		public LogConnectStatus(string mess) { 
		//GET_DEVICE_CONNECTION_STATUS | Status = SUCCESS | Runtime = 0ms | Device = 0C:F3:EE:8D:2C:CF Device-status = CONNECTED
	
		this.Parse(mess);}
		
		const string regPattern1 =  @"GET_DEVICE_CONNECTION_STATUS \| Status = (\w+) \| Runtime = \d+ms \| Device = (([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})) Device-status = (\w+)";
		const string regPattern2 =  @"GET_DEVICE_CONNECTION_STATUS";


	    public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }	
		
		public string ComStatus{get;set;}
		public string MacAddress{get;set;}
		public string PhonStatus{get;set;}

		
		public override void HandleType(WHInfo wh)
		{
			
		}

		public override bool ParsePattern1(string mess)
		{
			var parsed = false;
			var reg = new Regex(regPattern1);
			{
				var match = reg.Match(mess);
				if (match.Success) {
					
					ComStatus   = match.Groups[1].Value;
					MacAddress 	= match.Groups[2].Value;
					PhonStatus 	= match.Groups[5].Value;
					
					parsed = true;
				}
			}
			return parsed;	
		}
		public override bool ParsePattern2(string mess)
		{
			var parsed = false;
			var reg = new Regex(regPattern2);
			{
				var match = reg.Match(mess);
				if (match.Success) {
					parsed = false;
				}
			}
			return parsed;	
		}
		
		
		public string ToDisplayString()
		{
			return string.Format("{1} is connected {0}", PhonStatus, MacAddress);
		}

		public override string ToHeader()
		{
			return "";
		}	
		public override string ToLine()
		{
			return "";
		}		
		public override string ToString()
		{
			return string.Format("[LogConnectStatus Com={0}, MacAddress={1}, Status={2}]", ComStatus, MacAddress, PhonStatus);
		}
	}
}
