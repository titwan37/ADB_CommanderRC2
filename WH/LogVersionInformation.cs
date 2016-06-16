/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 31.03.2016
 * Time: 21:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogVersionInformation.
	/// </summary>
	public class LogVersionInformation: LogBody, ILogBody
	{
		const string regPattern1 = @"(([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2}))";
		//  GET_VERSION_INFORMATION | - | Version-Information: SZ2VersionInformation_t{coolRisc_HW_version=18, coolRisc_SW_version=130, arm_HW_version=27, arm_miniBootLoader_SW_version=128, arm_fotaBootLoader_SW_version=130, arm_app_SW_version=130, stack_HW_version=0, stack_SW_version=16843008, em6420_HW_version=0, adxl_HW_version=0, em9301_HW_version=0}
		const string regPattern2 = @"VersionInformation_t{coolRisc_HW_version=(\d+), coolRisc_SW_version=(\d+), arm_HW_version=(\d+), arm_miniBootLoader_SW_version=(\d+), arm_fotaBootLoader_SW_version=(\d+), arm_app_SW_version=(\d+), stack_HW_version=(\d+), stack_SW_version=(\d+), em6420_HW_version=(\d+), adxl_HW_version=(\d+), em9301_HW_version=(\d+)}";

		public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }
		
		public int coolRisc_HW_version  	{get;set;}
		public int coolRisc_SW_version  	{get;set;}
		
		public int arm_HW_version  	  	{get;set;}
		public int arm_miniBootLoader_SW_version  	  	{get;set;}
		public int arm_fotaBootLoader_SW_version	  	{get;set;}
		public int arm_app_SW_version 	  	{get;set;}
		
		public string stack_HW_version	  	{get;set;}
		public string stack_SW_version	  	{get;set;}
		public string em6420_HW_version	  	{get;set;}
		public string adxl_HW_version	  	{get;set;}
		public string em9301_HW_version	  	{get;set;}
		
		
		public LogVersionInformation()
		{
			
		}
		public LogVersionInformation(string mess) {
			this.Parse(mess);
		}
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
					//parsed = true;
				}
				else {
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
				if (match.Success) 
				{

				this.coolRisc_HW_version = System.Convert.ToInt32( match.Groups[1].Value);
				this.coolRisc_SW_version = System.Convert.ToInt32( match.Groups[2].Value);
			
				this.arm_HW_version  	 = System.Convert.ToInt32( match.Groups[3].Value);
				this.arm_miniBootLoader_SW_version  	 = System.Convert.ToInt32( match.Groups[4].Value);
				this.arm_fotaBootLoader_SW_version	 = System.Convert.ToInt32( match.Groups[5].Value);
				this.arm_app_SW_version 	 = System.Convert.ToInt32( match.Groups[6].Value);
				
				this.stack_HW_version	 = match.Groups[7].Value;
				this.stack_SW_version	 = match.Groups[8].Value;
				this.em6420_HW_version	 = match.Groups[9].Value;
				this.adxl_HW_version	 = match.Groups[10].Value;
				this.em9301_HW_version	 = match.Groups[11].Value;					
					
				parsed = true;
				}
			}
			return parsed;	
		}
		
		public string ToDisplayString()
		{
			
			return string.Format("Version (c{0:X},a{1:X})", coolRisc_SW_version, arm_app_SW_version);
		}
		public override string ToHeader()
		{
			return string.Format("c{0:X},a{1:X}", coolRisc_SW_version, arm_app_SW_version);
		}
		public override string ToLine()
		{
			return string.Format("[CoolRisc_HW_version={0}, CoolRisc_SW_version={1}, Arm_HW_version={2}, Arm_miniBootLoader_SW_version={3}, \nArm_fotaBootLoader_SW_version={4}, Arm_app_SW_version={5}, Stack_HW_version={6}, Stack_SW_version={7}, \nEm6420_HW_version={8}, Em9301_HW_version={9}, Adxl_HW_version={10}]", coolRisc_HW_version, coolRisc_SW_version, arm_HW_version, arm_miniBootLoader_SW_version, arm_fotaBootLoader_SW_version, arm_app_SW_version, stack_HW_version, stack_SW_version, em6420_HW_version, em9301_HW_version, adxl_HW_version);
		}		
		public override string ToString()
		{
			return string.Format("[CoolRisc_HW_version={0}, CoolRisc_SW_version={1}, Arm_HW_version={2}, Arm_miniBootLoader_SW_version={3}, Arm_fotaBootLoader_SW_version={4}, Arm_app_SW_version={5}, Stack_HW_version={6}, Stack_SW_version={7}, Em6420_HW_version={8}, Em9301_HW_version={9}, Adxl_HW_version={10}]", coolRisc_HW_version, coolRisc_SW_version, arm_HW_version, arm_miniBootLoader_SW_version, arm_fotaBootLoader_SW_version, arm_app_SW_version, stack_HW_version, stack_SW_version, em6420_HW_version, em9301_HW_version, adxl_HW_version);
		}
	}
}
