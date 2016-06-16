/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/22/2016
 * Time: 18:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of WatchSettingClass.
	/// </summary>
	public class WatchSettingClass
	{
		public WatchSettingClass(){ 
			DateTimeCulture="EU";
			HourFormat="24";AlarmBeep="on";
			AlarmStatus="off";
			AlarmRingtone= WH.RINGTONE.RINGTONE_0.ToString();
			AlarmRepetitionSettings="off,off,off,off,off,off,off"; BeatTime="999";}
		
		public string DateTimeCulture { get; set; }		
		public string HourFormat { get; set; }		
		public string AlarmBeep { get; set; }		
		public string AlarmStatus { get; set; }	
		public string AlarmRingtone	{ get; set; }
		public string AlarmRepetitionSettings { get; set; }	
		public string BeatTime { get; set; }		
	}
}
