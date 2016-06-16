/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 17.02.2016
 * Time: 17:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogActivityTimeSlot.
	/// </summary>
	public class LogActivityTimeSlot : LogBody, ILogBody
	{
		public LogActivityTimeSlot(string mess) { 
			base.Parse(mess);}
		const string	regPattern1 =  @"Activity-Timeslot: SZ2LogActivityTimeSlot_t{numberOfTimeSlots=(\d+), timeSlotPacketIndex=(\d+), activity=[[]((\d+), (\d+), (\d+), (\d+), (\d+), (\d+), (\d+), (\d+))[]][}]";
		const string	regPattern2 = @"activity,hour,(\d+),(\d+) [|] Activity-Timeslot: { NumberOfTimeslots=(\d+), TimeslotIndex=(\d+), (Activity={ value_0=(\d+), value_1=(\d+), value_2=(\d+), value_3=(\d+), value_4=(\d+), value_5=(\d+), value_6=(\d+), value_7=(\d+)) }";
		
	    public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }
		
		public UInt16 dayIndex 			{get;set;}
		public UInt16 slotIndex 			{get;set;}
		public UInt16 numberOfTimeSlots 				{get;set;}
		public UInt16 timeSlotPacketIndex				{get;set;}
		public string ActivityString					{get;set;}
		public UInt16[] activities						{get;set;}

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
					numberOfTimeSlots = System.Convert.ToUInt16(match.Groups[1].Value);
					timeSlotPacketIndex = System.Convert.ToUInt16(match.Groups[2].Value);

					ActivityString = match.Groups[3].Value;
					var splt = ActivityString.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
					if(splt.Length == 8)
					{
						activities = new ushort[8];
						for (int i = 0; i < splt.Length; i++) {
							activities[i] = System.Convert.ToUInt16(splt[i]);
						}
					}
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
					
					dayIndex = System.Convert.ToUInt16(match.Groups[1].Value);
					slotIndex = System.Convert.ToUInt16(match.Groups[2].Value);
					
					numberOfTimeSlots = System.Convert.ToUInt16(match.Groups[3].Value);
					timeSlotPacketIndex = System.Convert.ToUInt16(match.Groups[4].Value);

					ActivityString = match.Groups[5].Value;
					
					activities = GetTimeslots(match, 6, 8);

					parsed = true;
				}
			}
			return parsed;		
		}
		

		
		public override string ToHeader()
		{
			return string.Format("NumberOfTimeSlots;TimeSlotPacketIndex;Activity_1;Activity_2;Activity_3;Activity_4;Activity_5;Activity_6;Activity_7;Activity_8;DayIndex;");
		}

		public override string ToLine()
		{
			return string.Format("{0};{1};{2};{3};", numberOfTimeSlots, timeSlotPacketIndex, 
			                     string.Join(";", new System.Collections.Generic.List<ushort>(activities)),
			                    dayIndex);
		}
		public override string ToString()
		{
			return string.Format("[LogActivityTimeSlot dayIndex={3}, NumberOfTimeSlots={0}, TimeSlotPacketIndex={1}, activity={2}]", numberOfTimeSlots, timeSlotPacketIndex, ActivityString, dayIndex);
		}
	}
}
