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
	/// Description of LogPedometerDay.
	/// </summary>
	public class LogPedometerDay : LogBody, ILogBody, IDate
	{
		public LogPedometerDay(string mess) 
		{
			this.Parse(mess);
		}
		const string regPattern1 =  @"Pedometer-Day: SZ2LogPedometerDayMessage_t{numberOfDays=(\d+), dayIndex=(\d+), day_0=(\d+), month_0=(\d+), year_0=(\d+), walkStep_0=(\d+), runStep_0=(\d+), day_1=(\d+), month_1=(\d+), year_1=(\d+), walkStep_1=(\d+), runStep_1=(\d+)}";
		const string regPattern2 = @"Pedometer-Day: { NumberOfDays=(\d+), DayIndex=(\d+), Day 0={ Date=(\d+).(\d+).(\d+), Steps-walking=(\d+), Steps-running=(\d+)} Day 1={ Date=(\d+).(\d+).(\d+), Steps-walking=(\d+), Steps-running=(\d+)}";

	    public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }
		
		public string numberOfDays {get;set;}
		public string dayIndex{get;set;}
		
		public string date_0  	{get;set;}
		public string day_0  	{get;set;}
		public string month_0	{get;set;}
		public string year_0 	{get;set;}
		public int walkStep_0	{get;set;}
		public int runStep_0	{get;set;}
		
		public string walkDist_0{get;set;}
		public string runDist_0{get;set;}
		public string kCal_0{get;set;}
		
		public string date_1  	{get;set;}
		public string day_1{get;set;}
		public string month_1{get;set;}
		public string year_1{get;set;}
		public int walkStep_1{get;set;}
		public int runStep_1{get;set;}
	
		public override void HandleType(WHInfo wh)
		{
			if (wh.User !=null && wh.User.height>0)
			{
				var energy = wh.Energy.Calculate(this, wh.User.height, wh.User.weight);
				if (energy!=null)
				{
					this.walkDist_0 = energy.Dwalk;
					this.runDist_0 = energy.Drun;
					this.kCal_0 = energy.kCAL;
					return;
				}
			}
			
			this.walkDist_0 = "N/A";
			this.runDist_0 = "N/A";
			this.kCal_0 = "N/A";
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
					walkStep_0 = System.Convert.ToUInt16(match.Groups[6].Value);
					runStep_0 = System.Convert.ToUInt16(match.Groups[7].Value);

					day_1 = match.Groups[8].Value;
					month_1 = match.Groups[9].Value;
					year_1 = match.Groups[10].Value;
					date_1 = string.Format("{0}.{1}.{2}", day_1, month_1, year_1);
					walkStep_1 = System.Convert.ToUInt16(match.Groups[11].Value);
					runStep_1 = System.Convert.ToUInt16(match.Groups[12].Value);
					
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
					
					
					walkStep_0 = System.Convert.ToUInt16(match.Groups[6].Value);
					runStep_0 = System.Convert.ToUInt16(match.Groups[7].Value);

					day_1 = match.Groups[8].Value;
					month_1 = match.Groups[9].Value;
					year_1 = match.Groups[10].Value;
					date_1 = string.Format("{0}.{1}.{2}", day_1, month_1, year_1);
					
					walkStep_1 = System.Convert.ToUInt16(match.Groups[11].Value);
					runStep_1 = System.Convert.ToUInt16(match.Groups[12].Value);
					
					parsed = true;
				}
			}
			return parsed;
		}			
		
		public override string ToHeader()
		{
			return string.Format("NumberOfDays;DayIndex;Date_0;WalkStep_0;RunStep_0;walkDist_0;runDist_0;kCal_0;Date_1;WalkStep_1;RunStep_1");
		}

		public override string ToLine()
		{
			return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};", numberOfDays, dayIndex, date_0, walkStep_0, runStep_0, walkDist_0, runDist_0, kCal_0, date_1, walkStep_1, runStep_1);
		}
		public override string ToString()
		{
			return string.Format("[LogPedometerDay NumberOfDays={0}, DayIndex={1}, Date_0={2}, WalkStep_0={3}, RunStep_0={4}, walkDist_0={5}, runDist_0={6}, kCal_0={7}]", numberOfDays, dayIndex, date_0, walkStep_0, runStep_0, walkDist_0, runDist_0, kCal_0);
		}
	}
}
