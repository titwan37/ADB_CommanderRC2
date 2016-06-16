/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/16/2016
 * Time: 11:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogConnect.
	/// </summary>
	public class LogConnect : LogBody, ILogBody
	{
		public LogConnect(string mess) {
			this.Parse(mess);
		}
		
		const string regPattern3 =  @"CONNECT_DEVICE \| Status = (\w+) \| Runtime = \d+ms \| Successfully connected to Device: (([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2}))";
		// CONNECT_DEVICE | - | Successfully connected to Device: 0C:F3:EE:8E:05:2C
		const string regPattern1 = @"Successfully connected to Device: (([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2}))";
		//  onConnectionStateChanged - BluetoothProfile.STATE_DISCONNECTED 
		const string regPattern2 = @"onConnectionStateChanged [-] BluetoothProfile.STATE_DISCONNECTED";
		
		public static bool Is(string mess) 		{ return IsPattern1T(mess)|IsPattern2T(mess)|IsPattern3T(mess); }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		static bool IsPattern3T(string mess) { return Regex.IsMatch(mess, regPattern3); }

		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }
		
		public CONEXSTATE STATE {get;set;}
		public string Status{get;set;}
		public string MacAddress{get;set;}
		public string MacRadical{get; private set;}
		public string AppVersion{get;set;}
		public string Device{get;set;}
		public string Model{get;set;}
		public string OS{get;set;}
		
		public void SetMacRadical()
		{
			if (!string.IsNullOrEmpty(MacAddress))
				this.MacRadical = MacAddress.Substring(MacAddress.Length-5, 5).Replace(":","");
		}
		
		public override void HandleType(WHInfo wh)
		{
			
		}
		
		public void Merge (LogConnect loc)
		{
			if (loc.STATE!=CONEXSTATE.UNKNOWN) this.STATE = loc.STATE;
			
			if (loc.Status!=null) this.Status = loc.Status;
			if (loc.MacAddress!=null) this.MacAddress = loc.MacAddress;
			if (loc.MacRadical!=null) this.MacRadical = loc.MacRadical;
			if (loc.AppVersion!=null) this.AppVersion = loc.AppVersion;
			if (loc.Device!=null) this.Device = loc.Device;
			if (loc.Model!=null) this.Model = loc.Model;
			if (loc.OS!=null) this.OS = loc.OS;
		}
		
		
		public override bool Parse(string mess)
		{
			IsParsed = false;
			
			if (string.IsNullOrEmpty(mess)) return false;
			
			if(IsPattern1(mess)) IsParsed |= ParsePattern1(mess);
			if(IsPattern2(mess)) IsParsed |= ParsePattern2(mess);
			if(IsPattern3T(mess)) IsParsed |= ParsePattern3(mess);
			
			return base.Parse(mess);
		}
		
		/// <summary>
		/// OLD log entry pattern
		/// </summary>
		/// <param name="mess"></param>
		/// <returns></returns>
		public bool ParsePattern3(string mess)
		{
			var parsed = false;
			var reg = new Regex(regPattern1);
			{
				var match = reg.Match(mess);
				if (match.Success) {
					Status 		= match.Groups[1].Value;
					MacAddress 	= match.Groups[2].Value;
					SetMacRadical();
					parsed = true;
				}
				else {
					Status = "Unknown";
					MacAddress = "Unknown";
				}
			}
			return parsed;	
		}
		public override bool ParsePattern1(string mess)
		{
			var parsed = false;
			var reg = new Regex(regPattern1);
			{
				var match = reg.Match(mess);
				if (match.Success) {
					Status = "connected";
					STATE = CONEXSTATE.CONNECTED;
					MacAddress 	= match.Groups[1].Value;
					SetMacRadical();
					
					parsed = true;
				}
				else {
					Status = "Unknown";
					MacAddress = "Unknown";
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
					Status = "disconnected";
					STATE = CONEXSTATE.DISCONNECTED;
					MacAddress 	= MacAddress;
					SetMacRadical();
					
					parsed = true;
				}
				else {
					Status = "Unknown";
					MacAddress = "Unknown";
				}
			}
			return parsed;	
		}
		
		public string ToDisplayString()
		{
			return string.Format("{0} is {1}\n {2} {3} {4}\n App-Version:{5}", MacAddress, Status, Device, Model, OS, AppVersion);
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
			return string.Format("[LogConnect Status={0}, MacAddress={1}]", Status, MacAddress);
		}
	}
}
