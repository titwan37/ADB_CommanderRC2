/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/16/2016
 * Time: 11:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using IronPython.Modules;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogEnty.
	/// </summary>
	public class LogEnty : LogBody
	{
		public LogEnty(string mess) 
		{
			Message = mess;
			((LogEnty)this).Parse(mess);
		}
		//03-25 20:29:16.521 D/SwatchClient(24674): Commands on Stack: 1 NexCommand: EventLog
		const string regPattern0 = @"(\d{2}[-]\d{2} \d{2}[:]\d{2}[:]\d{2}[.]\d{3}) (\w)[\/](\w+)[(]\s*(\d+)[)][:]";
		const string regPattern00 = @"(\d{2}[-]\d{2} \d{2}[:]\d{2}[:]\d{2}[.]\d{3}) (\w)[\/]([\s\S]*)\s*[(]\s*(\d+)[)][:]";
		const string regPattern1 =  @"(\[\w+\]) (\d{2}[:]\d{2}[:]\d{2}[,]\d{3}) App-version: (\d[.]\d[.]\d) [|] (\w+) [|] Status = (\w+) [|] Runtime = (\d+)ms [|] ";
		// 03-22 13:44:52.581 D/SwatchClient(19326): logToDisk: App-version: 1.3.5 | GET_FAN_LOG | Status = SUCCESS | Runtime = 153ms | LogCommands - Fan-Timeslot: SZ2LogFanTimeSlot_t{numberOfTimeSlots=0, timeSlotPacketIndex=8, activity=[0, 0, 0, 0, 0, 0, 0, 0]}
		const string regPattern2 =  @"(\d{2}[-]\d{2} \d{2}[:]\d{2}[:]\d{2}[.]\d{3}) (\w)[\/]([\s\S]*)\s*[(]\s*(\d+)[)][:]([\s|\w]+.)[:] App-version: (\d[.]\d[.]\d) [|] (\w+) [|] Status = (\w+) [|] Runtime = (\d+)ms [|] ";
		// 03-24 13:07:03.981 D/SwatchClient(30570): logToDisk: App-version: 1.3.6 | Device: samsung | Model: SM-T365 | OS: 4.4.4 | Status = SUCCESS | GET_SETTINGS_TIME | - | Time-Settings: { Format=24/EU, Repeat=Mo/off, Tu/off, We/off, Th/off, Fr/off, Sa/off, Su/off, AlarmStatus=off, Ringtone=RINGTONE_0, AlarmBeep=on, Date/Time=24.02.2016-10:55, Alarm=07:00, BeatTime=455 } | Runtime = 228ms
		const string regPattern3 =  @"(\d{2}[-]\d{2} \d{2}[:]\d{2}[:]\d{2}[.]\d{3}) (\w)[\/]([\s\S]*)\s*[(]\s*(\d+)[)][:]([\s|\w]+.)[:] App-version: ([\s\S]*) [|] Device: ([\s\S]*) [|] Model: ([\s\S]*) [|] OS: ([\s\S]*) [|] Status = (\w+) [|] (\w+) [|] ";
		// | Runtime = 228ms
		const string regPattern3Tail = @"[|] Runtime = (\d+)ms$";		

		public static bool Is(string mess) 		{ return IsPattern0T(mess)|IsPattern1T(mess)|IsPattern2T(mess)|IsPattern3T(mess); }
		static bool IsPattern0T(string mess) { return Regex.IsMatch(mess, regPattern00); }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		static bool IsPattern3T(string mess) { return Regex.IsMatch(mess, regPattern3); }
		protected bool IsPattern0(string mess) { return IsPattern0T(mess); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }		
		
		public bool IsFullyParsed = false;
		public int PatternType = 0;
		
		public string datetime { get; set;}
		public string LogType { get; set;}
		public string LogPID { get; set;}
		public string LogAction { get; set;}
		public string LogChannel { get; set;}
		public string LogCommand { get; set;}
		public string Appversion { get; set;}
		public string Device { get; set;}
		public string Model { get; set;}
		public string OS { get; set;}
		public string Command { get; set;}
		public string Status { get; set;}
		public string Runtime { get; set;}
		public LogBody LogComments { get; set;}
		public string sLogComments { get; set;}

		public override void HandleType(WHInfo wh)
		{
			wh.Connection.AppVersion = this.Appversion;
			wh.Connection.Device = this.Device;
			wh.Connection.Model = this.Model;
			wh.Connection.OS = this.OS;
		}
		
		public override bool Parse(string mess)
		{
			IsParsed = false;
			try {
					if (string.IsNullOrEmpty(mess)) return false;
					
					if (LogCommandOnStack.Is(mess))			{ 
						var com = new LogCommandOnStack(mess); IsParsed=this.ParsePattern0(mess);
						this.LogComments = com; this.Command = com.ToString(); IsParsed=com.IsParsed;}
					else
					{
						if(IsPattern0(mess)) IsParsed |= ParsePattern0(mess);
						if(IsPattern1(mess)) IsParsed |= ParsePattern1(mess);
						if(IsPattern2(mess)) IsParsed |= ParsePattern2(mess);
						if(IsPattern3T(mess)) IsParsed |= ParsePattern3(mess);
						
						if(IsParsed)
							ParseComments(mess);
					}
					if(IsParsed)
						OnPropertyChanged("LogEntry");
					else
						System.Diagnostics.Debug.WriteLine("FAILED Parse : "+mess);
				} 
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine(e.ToString());
				}
			return IsParsed;					
		}
		/// <summary>
		/// ParsePattern0 for CommandOnStack
		/// </summary>
		/// <param name="mess"></param>
		/// <returns></returns>
		protected bool ParsePattern0(string mess)
		{
			var parsed = false;
			System.Text.RegularExpressions.Regex reg00 = new Regex(regPattern00);
			{
				var match00 = reg00.Match(mess);
				if (match00.Success) {
					PatternType = 00;

					datetime = match00.Groups[1].Value;
					LogType = match00.Groups[2].Value;
					LogChannel = match00.Groups[3].Value;
					LogPID = match00.Groups[4].Value;
					
					parsed = true;
					IsParsed = parsed;
				}
			}
			System.Text.RegularExpressions.Regex reg0 = new Regex(regPattern0);
			{
				var match = reg0.Match(mess);
				if (match.Success) {
					PatternType = 0;

					datetime = match.Groups[1].Value;
					LogType = match.Groups[2].Value;
					LogChannel = match.Groups[3].Value;
					LogPID = match.Groups[4].Value;
					
					parsed = true;
					IsParsed = parsed;
				}
			}
			return parsed;
		}
		public override bool ParsePattern1(string mess)
		{
			var parsed = false;
			System.Text.RegularExpressions.Regex reg = new Regex(regPattern1);
			{
				var match = reg.Match(mess);
				if (match.Success) {
					PatternType = 1;
					
					datetime = match.Groups[1].Value;
					LogType = match.Groups[2].Value;
					LogChannel = match.Groups[3].Value;
					LogPID = match.Groups[4].Value;
					LogCommand = match.Groups[5].Value;
					Appversion = match.Groups[6].Value;
					Command = match.Groups[7].Value;
					Status = match.Groups[8].Value;
					Runtime = match.Groups[9].Value;
					
					parsed = true;
					IsParsed = parsed;
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
					PatternType = 2;

					datetime = match.Groups[1].Value;
					LogType = match.Groups[2].Value;
					LogChannel = match.Groups[3].Value;
					LogPID = match.Groups[4].Value;
					LogCommand = match.Groups[5].Value;
					Appversion = match.Groups[6].Value;
					Command = match.Groups[7].Value;
					Status = match.Groups[8].Value;
					Runtime = match.Groups[9].Value;
					
					parsed = true;
					IsParsed = parsed;
				}
			}
			return parsed;
		}
		protected bool ParsePattern3(string mess)
		{
			var parsed = false;
			System.Text.RegularExpressions.Regex reg = new Regex(regPattern3);
			{
				var match = reg.Match(mess);
				if (match.Success) {
					PatternType = 3;

					datetime = match.Groups[1].Value;
					LogType = match.Groups[2].Value;
					LogChannel = match.Groups[3].Value;
					LogPID = match.Groups[4].Value;
					LogAction = match.Groups[5].Value;
					Appversion = match.Groups[6].Value;
					Device = match.Groups[7].Value;
					Model = match.Groups[8].Value;
					OS = match.Groups[9].Value;
					Status = match.Groups[10].Value;
					LogCommand = match.Groups[11].Value;
					
					parsed = true;
					IsParsed = parsed;
				}
			}

//			var strings = mess.Split(new char[]{'|'}, StringSplitOptions.RemoveEmptyEntries);
//			var len = strings.Length;
//			if (len>0)
//			{
//				sLogComments = strings[len-2];
//			}

			var matchRuntime = Regex.Match(mess, regPattern3Tail);
			if (matchRuntime.Success && matchRuntime.Groups.Count>1)
				Runtime = Regex.Match(mess, regPattern3Tail).Groups[1].Value;
			
			return parsed;
		}

		protected void ParseComments(string mess)
		{
			if (LogConnect.Is(mess))						{ this.LogComments = new LogConnect(mess); 					IsFullyParsed=LogComments.IsParsed;}
			else if (LogConnectStatus.Is(mess))				{ this.LogComments = new LogConnectStatus(mess); 			IsFullyParsed=LogComments.IsParsed;}
		
			else if (LogSettingsTime.Is(mess))				{ this.LogComments = new LogSettingsTime(mess); 			IsFullyParsed=LogComments.IsParsed;}
			else if (LogSettingsWatch.Is(mess))				{ this.LogComments = new LogSettingsWatch(mess); 			IsFullyParsed=LogComments.IsParsed;}
			else if (LogSettingsUser.Is(mess))				{ this.LogComments = new LogSettingsUser(mess); 			IsFullyParsed=LogComments.IsParsed;}
			else if (LogSettingsGoals.Is(mess))				{ this.LogComments = new LogSettingsGoals(mess); 			IsFullyParsed=LogComments.IsParsed;}
			else if (LogVersionInformation.Is(mess))		{ this.LogComments = new LogVersionInformation(mess); 		IsFullyParsed=LogComments.IsParsed;}
		
			else if (LogPedometerDay.Is(mess))		{ this.LogComments = new LogPedometerDay(mess); 			IsFullyParsed=LogComments.IsParsed;}
			else if (LogPedometerTimeSlot.Is(mess))	{ this.LogComments = new LogPedometerTimeSlot(mess); 		IsFullyParsed=LogComments.IsParsed;}

			else if (LogActivityDay.Is(mess))		{ this.LogComments = new LogActivityDay(mess); 				IsFullyParsed=LogComments.IsParsed;}
			else if (LogActivityTimeSlot.Is(mess))	{ this.LogComments = new LogActivityTimeSlot(mess); 		IsFullyParsed=LogComments.IsParsed;}

			else if (LogFanGame.Is(mess))			{ this.LogComments = new LogFanGame(mess); 					IsFullyParsed=LogComments.IsParsed;}
			else if (LogFanTimeSlot.Is(mess))		{ this.LogComments = new LogFanTimeSlot(mess); 				IsFullyParsed=LogComments.IsParsed;}			                                                                                                   
			                                                                                                   
			                                                                                                   
//			else if (new LogConnectStatus("mess").Is())	{ this.LogComments = (LogConnectStatus)tO; 			IsFullyParsed=true;}
//		
//			else if (new LogSettingsTime("").Is())	{ this.LogComments = new LogSettingsTime(mess); 			IsFullyParsed=true;}
//			else if (new LogSettingsWatch("").Is())	{ this.LogComments = new LogSettingsWatch(mess); 			IsFullyParsed=true;}
//			else if (new LogSettingsUser("").Is())	{ this.LogComments = new LogSettingsUser(mess); 			IsFullyParsed=true;}
//			else if (new LogSettingsGoals("").Is())	{ this.LogComments = new LogSettingsGoals(mess); 			IsFullyParsed=true;}
//		
//			else if (new LogPedometerDay("").Is())	{ this.LogComments = new LogPedometerDay(mess); 			IsFullyParsed=true;}
//			else if (new LogPedometerTimeSlot("").Is())	{ this.LogComments = new LogPedometerTimeSlot(mess); 	IsFullyParsed=true;}
//
//			else if (new LogActivityDay("").Is(mess))	{ this.LogComments = new LogActivityDay(mess); 				IsFullyParsed=true;}
//			else if (new LogActivityTimeSlot("").Is(mess))	{ this.LogComments = new LogActivityTimeSlot(mess); 	IsFullyParsed=true;}
//
//			else if (new LogFanGame("").Is(mess))		{ this.LogComments = new LogFanGame(mess); 					IsFullyParsed=true;}
//			else if (new LogFanTimeSlot("").Is(mess))	{ this.LogComments = new LogFanTimeSlot(mess); 				IsFullyParsed=true;}
		}
		
		
		public override string ToHeader()
		{
			return string.Join(";", new string[]{"LogType", "LogChannel", "PID",  "datetime", "Appversion", "Command", "Status", "Runtime", "Arguments"});
		}
		
		public override string ToLine()
		{
			if(IsParsed)
				return string.Format("{0};{1};{2}];{3};{4};{5};{6};{7};{8}", 
			                   LogType, LogChannel, LogPID,  datetime, Appversion, Command, Status, Runtime, LogComments);
			else return "";
		}
		
		/*
		*/

		public override string ToString()
		{
			if(IsParsed)
				return string.Format("[{0}/{1}-{2}] {3} App-version: {4} | {5} | Status = {6} | Runtime = {7}ms | {8}", 
			                   LogType, LogChannel, LogPID,  datetime, Appversion, Command, Status, Runtime, LogComments);
			else return "";
		}

		/*
		public override string ToString()
		{
			return string.Format("Datetime={3}, LogType={4}, LogPID={5}, LogAction={6}, LogChannel={7}, LogCommand={8}, Appversion={9}, Device={10}, Model={11}, OS={12}, Command={13}, Status={14}, Runtime={15}, LogComments={16}, SLogComments={17}]", 
			                     datetime, LogType, LogPID, LogAction, LogChannel, LogCommand, Appversion, Device, Model, OS, Command, Status, Runtime, LogComments, sLogComments);
		}*/
	}

}
