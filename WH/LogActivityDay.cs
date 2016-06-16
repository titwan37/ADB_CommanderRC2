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
	/// Description of LogActivityDay.
	/// </summary>
	public class LogActivityDay : LogBody, ILogBody, IDate
	{
		public LogActivityDay(string mess) 
		{
			this.Parse(mess);
		}
	
		const string	regPattern1 =  @"Activity-Day: SZ2LogActivityDayMessage_t{numberOfDays=(\d+), dayIndex=(\d+), day_0=(\d+), month_0=(\d+), year_0=(\d+), absActivityRed_0=(\d+), GoalReached_0=(\d+), day_1=(\d+), month_1=(\d+), year_1=(\d+), absActivityRed_1=(\d+), GoalReached_1=(\d+)}";
		const string	regPattern2 =  @"Activity-Day: { NumberOfDays=(\d+), DayIndex=(\d+), Day 0={ Date=(\d+).(\d+).(\d+), AbsActivityRed=(\d+), GoalReached=([0-9]+\.[0-9]+)%} Day 1={ Date=(\d+).(\d+).(\d+), AbsActivityRed=(\d+), GoalReached=([0-9]+\.[0-9]+)% }";

	    public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }
		
		public string numberOfDays 			{get;set;}
		public string dayIndex				{get;set;}
		
		public string date_0					{get;set;}
		public string day_0					{get;set;}
		public string month_0				{get;set;}
		public string year_0				{get;set;}
		public string absActivityRed_0		{get;set;}
		public string GoalReached_0	{get;set;}
		
		public string date_1					{get;set;}
		public string day_1					{get;set;}
		public string month_1				{get;set;}
		public string year_1				{get;set;}
		public string absActivityRed_1		{get;set;}
		public string GoalReached_1	{get;set;}

	
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
					numberOfDays = match.Groups[1].Value;
					dayIndex = match.Groups[2].Value;

					day_0 = match.Groups[3].Value;
					month_0 = match.Groups[4].Value;
					year_0 = match.Groups[5].Value;
					date_0 = string.Format("{0}.{1}.{2}", day_0, month_0, year_0);
					absActivityRed_0 = match.Groups[6].Value;
					GoalReached_0 = match.Groups[7].Value;

					day_1 = match.Groups[8].Value;
					month_1 = match.Groups[9].Value;
					year_1 = match.Groups[10].Value;
					date_1 = string.Format("{0}.{1}.{2}", day_1, month_1, year_1);
					absActivityRed_1 = match.Groups[11].Value;
					GoalReached_1 = match.Groups[12].Value;
					
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
				if (match.Success) {
					numberOfDays = match.Groups[1].Value;
					dayIndex = match.Groups[2].Value;

					day_0 = match.Groups[3].Value;
					month_0 = match.Groups[4].Value;
					year_0 = match.Groups[5].Value;
					
					date_0 = string.Format("{0}.{1}.{2}", day_0, month_0, year_0);
					
					absActivityRed_0 = match.Groups[6].Value;
					GoalReached_0 = match.Groups[7].Value;

					day_1 = match.Groups[8].Value;
					month_1 = match.Groups[9].Value;
					year_1 = match.Groups[10].Value;
					date_1 = string.Format("{0}.{1}.{2}", day_1, month_1, year_1);
					
					absActivityRed_1 = match.Groups[11].Value;
					GoalReached_1 = match.Groups[12].Value;
					
					parsed = true;
				}
			}
			return parsed;
		}
		
		public override string ToHeader()
		{
			return string.Format("NumberOfDays;DayIndex;Date_0;AbsActivityRed_0;GoalReached_0;Date_1;AbsActivityRed_1;GoalReached_1");
		}

		public override string ToLine()
		{
			return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};", numberOfDays, dayIndex, date_0, absActivityRed_0, GoalReached_0, date_1, absActivityRed_1, GoalReached_1);
		}
		
		public override string ToString()
		{
			return string.Format("[LogActivityDay NumberOfDays={0}, DayIndex={1}, Date_0={2}, AbsActivityRed_0={3}, GoalReached_0={4}, Date_1={5}, AbsActivityRed_1={6}, GoalReached_1={7}]", numberOfDays, dayIndex, date_0, absActivityRed_0, GoalReached_0, date_1, absActivityRed_1, GoalReached_1);
		}
	}
}
