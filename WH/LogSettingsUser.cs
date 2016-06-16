/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/16/2016
 * Time: 11:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogSettingsUser.
	/// </summary>
	public class LogSettingsUser : LogBody
	{
		public LogSettingsUser(string mess)
		{
			this.Parse(mess);
		}

		const string regPattern1 = @"Settings-User: SZ2SettingsUser_t[{]stride=(\d+), height=(\d+), weight=(\d+), gender=(\d+), age=(\d+), birthdayDay=(\d+), birthdayMonth=(\d+), birthdayYear=(\d+), rank=(\d+)[}]";
		const string regPattern2 = @"User-Settings: { Stride=(\d+), Height=(\d+), Weight=(\d+), Gender=(\w+), Age=(\d+), Birthday=(\d+).(\d+).(\d+), Rank=(\d+) }";

	    public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }
		
		public int stride { get; set; }
		public int height{ get; set; }
		public int weight{ get; set; }
		public string gender{ get; set; }
		public int age{ get; set; }
		public int birthdayDay{ get; set; }
		public int birthdayMonth{ get; set; }
		public int birthdayYear{ get; set; }
		public int rank{ get; set; }
	
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
					stride = 	System.Convert.ToInt32( match.Groups[1].Value);
					height = 	System.Convert.ToInt32( match.Groups[2].Value);
					weight = 	System.Convert.ToInt32( match.Groups[3].Value);
					gender = 	match.Groups[4].Value;
					age = 		System.Convert.ToInt32( match.Groups[5].Value);
					birthdayDay = System.Convert.ToInt32( match.Groups[6].Value);
					birthdayMonth = System.Convert.ToInt32( match.Groups[7].Value);
					birthdayYear = System.Convert.ToInt32( match.Groups[8].Value);
					rank = System.Convert.ToInt32( match.Groups[9].Value);
					
					parsed = true;
					OnPropertyChanged("User");
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
				if (match.Success) {
					stride = 	System.Convert.ToInt32( match.Groups[1].Value);
					height = 	System.Convert.ToInt32( match.Groups[2].Value);
					weight = 	System.Convert.ToInt32( match.Groups[3].Value);
					gender = 	match.Groups[4].Value=="male"?"0":"1";
					age = 		System.Convert.ToInt32( match.Groups[5].Value);
					birthdayDay = System.Convert.ToInt32( match.Groups[6].Value);
					birthdayMonth = System.Convert.ToInt32( match.Groups[7].Value);
					birthdayYear = System.Convert.ToInt32( match.Groups[8].Value);
					rank = System.Convert.ToInt32( match.Groups[9].Value);
					
					parsed = true;
					OnPropertyChanged("User");
				}
			}
			return parsed;	
		}		
		public DateTime GetBirthdayDate()
		{
			if (birthdayYear==0) {
				return DateTime.MinValue;
			}else
			return new DateTime(birthdayYear, birthdayMonth, birthdayDay);
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
			return string.Format("[LogSettingsUser Stride={0}, Height={1}, Weight={2}, Gender={3}, Age={4}, BirthdayDay={5}, BirthdayMonth={6}, BirthdayYear={7}, Rank={8}]", stride, height, weight, gender, age, birthdayDay, birthdayMonth, birthdayYear, rank);
		}
	}
}
