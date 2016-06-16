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
using System.Windows.Media.TextFormatting;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogPedometerTimeSlot.
	/// </summary>
	public class LogPedometerTimeSlot : LogBody
	{
		public LogPedometerTimeSlot(string mess) { 
			this.Parse(mess);
		}

		const string regPattern1 =  @"Pedometer-Timeslot: SZ2LogPedometerTimeSlot_t{numberOfTimeSlots=(\d+), timeSlotPacketIndex=(\d+), steps=[[]((\d+), (\d+), (\d+), (\d+), (\d+), (\d+), (\d+), (\d+))[]][}]";
		const string regPattern2 =  @"pedo,hour,(\d+),(\d+) [|] Pedometer-Timeslot: { NumberOfTimeslots=(\d+), TimeslotIndex=(\d+), (Steps={ value_0=(\d+), value_1=(\d+), value_2=(\d+), value_3=(\d+), value_4=(\d+), value_5=(\d+), value_6=(\d+), value_7=(\d+)) }";

	    public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }
		
		public UInt16 dayIndex 			{get;set;}
		public UInt16 slotIndex 			{get;set;}
		public UInt16 numberOfTimeSlots 			{get;set;}
		public UInt16 timeSlotPacketIndex			{get;set;}
		public string Stepstring					{get;set;}
		public UInt16[] steps						{get;set;}
		
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

					Stepstring = match.Groups[3].Value;
					var splt = Stepstring.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
					if(splt.Length == 8)
					{
						steps = new ushort[8];
						for (int i = 0; i < splt.Length; i++) {
							steps[i] = System.Convert.ToUInt16(splt[i]);
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
				if (match.Success) 
				{
					dayIndex = System.Convert.ToUInt16(match.Groups[1].Value);
					slotIndex = System.Convert.ToUInt16(match.Groups[2].Value);
					
					numberOfTimeSlots = System.Convert.ToUInt16(match.Groups[3].Value);
					timeSlotPacketIndex = System.Convert.ToUInt16(match.Groups[4].Value);

					Stepstring = match.Groups[5].Value;
					
					steps = GetTimeslots(match, 6, 8);

					parsed = true;
					//System.Diagnostics.Debug.WriteLine(Message);
				}
			}
			return parsed;	
		}
		
		public override string ToHeader()
		{
			return string.Format("NumberOfTimeSlots;TimeSlotPacketIndex;Step_1;Step_2;Step_3;Step_4;Step_5;Step_6;Step_7;Step_8;DayIndex;");
		}

		public override string ToLine()
		{
			return string.Format("{0};{1};{2};{3};", numberOfTimeSlots, timeSlotPacketIndex, 
			                     string.Join(";", new System.Collections.Generic.List<ushort>(steps)), 
			                     dayIndex);
		}

		public override string ToString()
		{
			return string.Format("[LogPedometerTimeSlot dayIndex={3}, NumberOfTimeSlots={0}, TimeSlotPacketIndex={1}, Steps={2}]", numberOfTimeSlots, timeSlotPacketIndex, Stepstring,dayIndex);
		}
	}
}
