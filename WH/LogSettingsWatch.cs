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
using Microsoft.Scripting.Utils;
using WPF_S39_Commander.Diagnostic;

namespace WPF_S39_Commander.WH
{

		/// <summary>
	/// Description of LogSettingsWatch.
	/// </summary>
	public class LogSettingsWatch : LogBody, ILogBody
	{
		public LogSettingsWatch(string mess) { 
			this.Parse(mess);
		}

		const string regPattern1 =  @"SwatchZero2ServiceClient - Settings-Watch: SZ2SettingsWatch_t{flags=(\d+), watchName=[[]([\d|,|\s]+)[]], ringtone=(\d+), configuration=(\d+)[}]";
		const string regPattern2 = @"Watch-Settings: { TouchBeep=(\w+), DistanceFormat=(\w+), Name=(\S+)[,] Ringtone=(\w+), Menu0=(\w+), Menu1=(\w+), Menu2=(\w+) }";

	    public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }
		
		public string flags {get;set;}
		public string watchNameNbs{get;set;}
		
		private string _Wname = "";
		public string WatchName	{get {return _Wname;} set {_Wname = NormalizeWatchName(value);} }
		string NormalizeWatchName (string name)
		{
			const char chrNul = '\0';
			var del = new char[]{chrNul};
			var index = name.IndexOfAny(del);
			if (index>=0)
			{
				var Rname = name.Substring(0, index);
				return Rname;
			}
			else
				return name;
		}
		
		public void Merge (LogSettingsWatch loc)
		{
			this.TimerRingtone = loc.TimerRingtone;

			if (loc.WatchName!=null) this.WatchName = loc.WatchName;
			if (loc.watchNameNbs!=null) this.watchNameNbs = loc.watchNameNbs;
			if (loc.configuration!=null) this.configuration = loc.configuration;
			if (loc.flags!=null) this.flags = loc.flags;		
			if (loc.Menu1!=null) this.Menu1 = loc.Menu1;
			if (loc.Menu2!=null) this.Menu2 = loc.Menu2;
			if (loc.Menu3!=null) this.Menu3 = loc.Menu3;
			if (loc.TouchBeep!=null) this.TouchBeep = loc.TouchBeep;
			if (loc.DistanceFormat!=null) this.DistanceFormat = loc.DistanceFormat;
		}
		
		
		public string TouchBeep { get; set; }
		public string DistanceFormat { get; set; }
		
		public RINGTONE TimerRingtone	{get;set;}
		public string sTimerRingtone{
			get {	return (TimerRingtone).ToString();}
			set { 	TimerRingtone = UtilEnum<WH.RINGTONE>.ParseEnum(value);}}
		public int iTimerRingtone{
			get {	return (int)TimerRingtone;}
			set { 	TimerRingtone = UtilEnum<WH.RINGTONE>.ParseEnum(value);}}		
		
		public string configuration{get;set;}

		public string Menu1		{get;set;}
		public string Menu2		{get;set;}
		public string Menu3		{get;set;}

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
					flags = match.Groups[1].Value;
					watchNameNbs = match.Groups[2].Value;
					WatchName = GetWatchName();
					
					iTimerRingtone = System.Convert.ToInt32( match.Groups[3].Value);
					
					configuration = match.Groups[4].Value;
					
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
					
					TouchBeep = match.Groups[1].Value;
					DistanceFormat = match.Groups[2].Value;

					WatchName = match.Groups[3].Value;
					
					sTimerRingtone = match.Groups[4].Value;
					
					Menu1 = match.Groups[5].Value;
					Menu2 = match.Groups[6].Value;
					Menu3 = match.Groups[7].Value;
					
					parsed = true;
				}
			}
			return parsed;	
		}		
		public string GetWatchName()
		{
			string whName = "";
			if(!string.IsNullOrEmpty(watchNameNbs)){
			var splt = watchNameNbs.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < splt.Length; i++) 
				{
					UInt16 d = Convert.ToUInt16(splt[i]);
					char c = Convert.ToChar(d);
					whName+=	c.ToString();
				}
			}
			return whName;
		}
		
		public string ToDisplayString()
		{
			return string.Format("{0}", WatchName);
		}	
		
		public string FlagToString()
		{
			return string.Format("{0} {1} {2}", TimerRingtone, configuration, flags);
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
			return string.Format("[LogSettingsWatch Flags={0}, WatchName={1}, Timer_Ringtone={2}, Configuration={3}] [Namecode={4}]", flags, WatchName, TimerRingtone, configuration, watchNameNbs);
		}
	}
}