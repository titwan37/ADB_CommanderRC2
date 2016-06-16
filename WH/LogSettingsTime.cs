/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 16.02.2016
 * Time: 13:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;
using IronPython.Modules;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogSettingsTime.
	/// </summary>
	public class LogSettingsTime : LogBody
	{
		public LogSettingsTime(string mess) { 
			this.Parse(mess);}

		const string regPattern1 =  @"SZ2SettingsTime_t[{], timeSettings=(\d+), alarmRepeat=(\d+), weekday=(\d+), day=(\d+), month=(\d+), year=(\d+), hour=(\d+), minute=(\d+), second=(\d+), alarmHour=(\d+), alarmMinute=(\d+)[}]";
		const string regPattern2 =  @"Time-Settings: [{] Format=(\w+)[\/](\D+), Repeat=[{]Mo[\/](\w+), Tu[\/](\w+), We[\/](\w+), Th[\/](\w+), Fr[\/](\w+), Sa[\/](\w+), Su[\/](\w+)}, AlarmStatus=(\w+), Ringtone=(\w+), AlarmBeep=(\w+), Date[\/]Time=((\d+)[.](\d+)[.](\d+))[-]?((\d+):(\d+):(\d+))?, Alarm=((\d+):(\d+)), BeatTime=(\d+) [}]";

	    public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }
		
	    public string timeSettings 	{get;set;}
		public string timeFormat 	{get;set;}
		public string dateFormat 	{get;set;}
		public string alarmStatus	{get;set;}
		public string alarmRepeat	{get;set;}
		
		public RINGTONE AlarmRingtone	{get;set;}
		public string sAlarmRingtone{
			get {	return (AlarmRingtone).ToString();}
			set { 	AlarmRingtone = UtilEnum<WH.RINGTONE>.ParseEnum(value);}}
		public int iAlarmRingtone{
			get {	return (int)AlarmRingtone;}
			set { 	AlarmRingtone = UtilEnum<WH.RINGTONE>.ParseEnum(value);}}	
		
		public string alarmBeep		{get;set;}
		
		public int weekday  {get;set;}
		
		public string stringDate		{get;set;}
		public int day{get;set;}
		public int month{get;set;}
		public int year{get;set;}
		
		public string stringTime		{get;set;}
		public int hour{get;set;}
		public int minute{get;set;}
		public int second{get;set;}
		
		public string stringAlarmtime		{get;set;}
		public int alarmHour{get;set;}
		public int alarmMinute{get;set;}

		public int BeatTime					{get;set;}
		
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
					timeSettings = match.Groups[1].Value;
					alarmRepeat = match.Groups[2].Value;
					weekday =  System.Convert.ToInt32( match.Groups[3].Value );
					day =  System.Convert.ToInt32( match.Groups[4].Value);
					month =  System.Convert.ToInt32( match.Groups[5].Value);
					year =  System.Convert.ToInt32( match.Groups[6].Value);
					hour =  System.Convert.ToInt32( match.Groups[7].Value);
					minute =  System.Convert.ToInt32( match.Groups[8].Value);
					second =  System.Convert.ToInt32( match.Groups[9].Value);
					alarmHour = System.Convert.ToInt32(  match.Groups[10].Value);
					alarmMinute =  System.Convert.ToInt32( match.Groups[11].Value);
					
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
					timeFormat = match.Groups[1].Value;
					dateFormat = match.Groups[2].Value;
					for (int i = 3; i < 10; i++) 
					{
						alarmRepeat += match.Groups[i].Value + ",";
					}
					
					alarmStatus =  match.Groups[10].Value ;
					
					sAlarmRingtone =  match.Groups[11].Value ;
					
					weekday = 0;

					alarmBeep =  match.Groups[12].Value ;
					
					stringDate = match.Groups[13].Value;
					
					day =  System.Convert.ToInt32( match.Groups[14].Value);
					month =  System.Convert.ToInt32( match.Groups[15].Value);
					year =  System.Convert.ToInt32( match.Groups[16].Value);
					
					stringTime = match.Groups[17].Value;
					
					hour =  System.Convert.ToInt32( match.Groups[18].Value);
					minute =  System.Convert.ToInt32( match.Groups[19].Value);
					second =  System.Convert.ToInt32( match.Groups[20].Value);
					
					stringAlarmtime = match.Groups[21].Value;
					
					alarmHour = System.Convert.ToInt32(  match.Groups[22].Value);
					alarmMinute =  System.Convert.ToInt32( match.Groups[23].Value);
					
					BeatTime =  System.Convert.ToInt32( match.Groups[24].Value);
					
					parsed = true;
				}
			}
			return parsed;	
		}		
		
		public DateTime GetDateTime()
		{
			if (year==0)
				return DateTime.MinValue;
			else
				return new DateTime(
						this.year,
						this.month,
						this.day,
						this.hour,
						this.minute,
						this.second
				                   );
		}
		
		public DateTime GetAlarmTime()
		{
			if (year==0)
				return DateTime.MinValue;
			else
				return new DateTime(
						System.Convert.ToInt32(this.year),
						System.Convert.ToInt32(this.month),
						System.Convert.ToInt32(this.day),
						System.Convert.ToInt32(this.alarmHour),
						System.Convert.ToInt32(this.alarmMinute),
						0
				                             		);
		}		
		
		public DateTime ChangeDate(DateTime newdate)
		{
			this.year = newdate.Year;
			this.month = newdate.Month;
			this.day = newdate.Day;
			this.weekday = getETA_WeekDay((int)newdate.DayOfWeek);
	
			return this.GetDateTime();
		}
		
		public DateTime SetDateTime(DateTime dt)
		{
			this.year = dt.Year;
			this.month = dt.Month;
			this.day = dt.Day;
			this.hour = dt.Hour;
			this.minute = dt.Minute;
			this.second = dt.Second;
			this.weekday = getETA_WeekDay((int)dt.DayOfWeek);
			
			return this.GetDateTime();
		}
		
		int getETA_WeekDay(int microsoftWeekday) { var val = (int)microsoftWeekday; return val==0 ? 6 : val-1;}
		
		public bool SetClockTime(DateTime clocktime)
		{
			var dtD = this.GetDateTime();
			
			DateTime dt = new DateTime(dtD.Year, dtD.Month, dtD.Day, clocktime.Hour, clocktime.Minute, clocktime.Second);
			
			this.hour = clocktime.Hour;
			this.minute = clocktime.Minute;			
			this.second = clocktime.Second;
			
			return true;
		}
		
		public bool SetAlarm(DateTime alarmTime)
		{
			var dtD = this.GetDateTime();
			DateTime dt = new DateTime(dtD.Year, dtD.Month, dtD.Day,alarmTime.Hour, alarmTime.Minute, alarmTime.Second);
			
			this.alarmHour = alarmTime.Hour;
			this.alarmMinute = alarmTime.Minute;
				
			return true;
		}
		
		public void SetBeatTime(int beat)
		{
			this.BeatTime = beat;
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
			return string.Format("[SettingsTime TimeSettings={0}, alarmRepeat={1}, Weekday={2}, Day={3}, Month={4}, Year={5}, Hour={6}, Minute={7}, Second={8}, AlarmHour={9}, AlarmMinute={10}, BeatTime={11}]", timeSettings, alarmRepeat, weekday, day, month, year, hour, minute, second, alarmHour, alarmMinute, BeatTime);
		}
	}
}
