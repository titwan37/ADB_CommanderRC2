/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 25.03.2016
 * Time: 21:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogCommandOnStack.
	/// </summary>
	public class LogCommandOnStack: LogBody, ILogBody
	{
		const string regPattern1 = @"Commands on Stack: 1 NexCommand: (\S+)";
		
		public LogCommandOnStack(string mess) { 
			this.Parse(mess);
		}
		
		public static bool Is(string mess) 		{ return IsPattern1T(mess); }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return false; }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return false; }
		
		public string Command					{get;set;}
		
		public override void HandleType(WHInfo wh)
		{
			
		}
		public override bool ParsePattern1(string mess)
		{
			var parsed = false;
			System.Text.RegularExpressions.Regex reg = new Regex(regPattern1);
			{
				var match = reg.Match(mess);
				if (match.Success) {
					Command = System.Convert.ToString(match.Groups[1].Value);
					parsed = true;
				}
			}
			return parsed;	
		}
		public override bool ParsePattern2(string mess)
		{
			return false;	
		}
		
		public override string ToHeader()
		{
			return string.Format("Commands;");
		}

		public override string ToLine()
		{
			return string.Format("{0};", Command);
		}
		
		public override string ToString()
		{
			return string.Format("[CommandOnStack={0}]", Command);
		}

		
	}
}
