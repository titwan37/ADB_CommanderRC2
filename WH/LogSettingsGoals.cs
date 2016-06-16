/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 17.02.2016
 * Time: 18:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogSettingsGoals.
	/// </summary>
	public class LogSettingsGoals : LogBody, ILogBody
	{
		public LogSettingsGoals(string mess){ this.Parse(mess);}

		const string regPattern1 = @"SZ2SettingsGoals_t[{]goals=(\d+), goals_8b=[[](\d+, \d+, \d+, \d+, \d+, \d+)[]], goals_16b=[[](\d+, \d+, \d+, \d+, \d+, \d+)[]][}]";
	    const string regPattern2 = @"Goal-Settings: { Goal_0=(\w+)[\/](\d+), Goal_1=(\w+)[\/](\d+), Goal_2=(\w+)[\/](\d+), Goal_3=(\w+)[\/](\d+), Goal_4=(\w+)[\/](\d+), Goal_5=(\w+)[\/](\d+), Goal_6=(\w+)[\/](\d+), Goal_7=(\w+)[\/](\d+), Goal_8=(\w+)[\/](\d+), Goal_9=(\w+)[\/](\d+), Goal_10=(\w+)[\/](\d+), Goal_11=(\w+)[\/](\d+)}";

	    public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }

	    public string flags 		{get;set;}
		public string offString		{get;set;}
		public string[] states		{get;set;}
		public string goalString	{get;set;}
		public int Goal				{get;set;}
		public int[] goals			{get;set;}
		public bool GoalState		{get;set;}
		
		public override void HandleType(WHInfo wh)
		{
			
		}
		public override bool ParsePattern1(string mess)
		{
			var parsed = false;
			System.Text.RegularExpressions.Regex reg = new Regex(regPattern1);
			{
				var match = reg.Match(mess);
				if (match.Success) 
				{
					flags = match.Groups[1].Value;
					GoalState=(flags=="64");
					
					goalString = "";
					goalString += match.Groups[2].Value;
					goalString += ",";
					goalString += match.Groups[3].Value;
					
					goals = GetGoals(goalString);
					Goal = goals[6];
					
					parsed = true;
				}
			}
			return parsed;	
		}
		public override bool ParsePattern2(string mess)
		{
			var parsed = false;
			System.Text.RegularExpressions.Regex reg = new Regex(regPattern2);
			{
				var match = reg.Match(mess);
				if (match.Success) 
				{

//					flags = match.Groups[1].Value;
//					GoalState=(flags=="64");


					offString = "";
					for (int i = 1; i < match.Groups.Count; i++) {
						offString += match.Groups[i].Value;
						offString += ",";
						i++;
					}
					goalString = "";
					for (int i = 2; i < match.Groups.Count; i++) {
						goalString += match.Groups[i].Value;
						goalString += ",";
						i++;
					}
					states = GetStates(offString);
					goals = GetGoals(goalString);
					
					Goal = System.Convert.ToInt32(goals[6]);
					GoalState = System.Convert.ToBoolean(states[6]=="on"?true:false);
					
					parsed = true;
				}
			}
			return parsed;	
		}
		
		protected static string[] GetStates(string offString)
		{
			var ret = new string[12];
			if(!string.IsNullOrEmpty(offString)){
			var splt = offString.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < splt.Length; i++) 
				{
					string d = (splt[i]);
					ret[i] = d;
				}
			}
			return ret;
		}
		
		protected static int[] GetGoals(string goalString)
		{
			var ret = new int[12];
			if(!string.IsNullOrEmpty(goalString)){
			var splt = goalString.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < splt.Length; i++) 
				{
					int d = Convert.ToInt32(splt[i]);
					ret[i] = d;
				}
			}
			return ret;
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
			return string.Format("[LogSettingsGoals State={0}, Goal={1}, Flags={2}, goalString={3}]", GoalState,  Goal, flags, goalString);
		}

	}
}