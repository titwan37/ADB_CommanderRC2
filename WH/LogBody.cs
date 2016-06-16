/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 24.03.2016
 * Time: 16:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using IronPython.Modules;
using WPF_S39_Commander.Control;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogBody.
	/// </summary>
	public abstract class LogBody : ILogBody, IScribe, INotifyPropertyChanged
	{
//		string regPattern1 ;//{get; set;}
//		string regPattern2 ;//{get; set;}
		
		public int PatternNumber = 0;

		public string Message; public string GetMessage() {return Message;}
		
		public LogBody(){}
		public LogBody(string mess){ Message=mess; ((LogBody)this).Parse(mess);}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		
		public static string GetDayIndexForTimeSlotIndex(UInt16 timeslotIndex)
		{
			return (System.Convert.ToInt32(Math.Truncate(timeslotIndex / 3.0))).ToString();
		}
		
		public abstract void HandleType(WHInfo wh);
		public abstract string ToHeader();
		//public static abstract bool Is(string mess);
		
		public bool IsParsed {get;set;}
		public virtual bool Parse(string mess)
		{	
			IsParsed = false;
			
			if (mess==string.Empty) return IsParsed;

			Message = mess;
			
			if(IsPattern1(mess)) { IsParsed |= ParsePattern1(mess); PatternNumber = 1;}
			if(IsPattern2(mess)) { IsParsed |= ParsePattern2(mess); PatternNumber = 2;}

			return IsParsed;
		}
		
		public static ushort[] GetTimeslots(Match match, int startIndex, int numberOfTimeSlots)
		{
			var Slots = new ushort[numberOfTimeSlots];
			try {
					if (match!=null && match.Groups!=null)
					if(startIndex+numberOfTimeSlots <= match.Groups.Count)
					{
						int tabIndex = 0;
						for (int i = startIndex; i < match.Groups.Count; i++) {
							var val = match.Groups[i].Value;
							Slots[tabIndex++] = System.Convert.ToUInt16(val);
						}
					}
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine("GetTimeslots :" + e.ToString());
				return null;
			} 
			return Slots;
		}
		
		
		protected abstract bool IsPattern1(string mess) ;
		protected abstract bool IsPattern2(string mess) ;
		public abstract bool ParsePattern1(string mess);
		public abstract bool ParsePattern2(string mess);
		
		public abstract string ToLine();
	}
	
	/// <summary>
	/// ILogBody interface : brings homogenization of LogBody classes for Generic usage
	/// </summary>
	public interface ILogBody
	{
		bool IsParsed {get;set;}
		string GetMessage();
		//bool Is(string mess);
		bool Parse(string mess);
		bool ParsePattern1(string mess);
		bool ParsePattern2(string mess);
		void HandleType(WHInfo wh);
	}
	
	public interface IDate
	{
		string day_0 			{get; set;}
		string month_0 		{get; set;}
		string year_0 		{get; set;}
	}
	
	public interface IScribe
	{
		string ToHeader();
		string ToLine();
		
		//public static class FaceMethod{ public static string ToHeader(){return "";} }
	}
	
	public interface IScribeExt
	{
		string ToHeaderExt();
		string ToLineExt();
	}
	
	
	/*
	public static class IScribeMethods 
	{
    	public static string ToHeader(this IScribe instanceOfiScribe) 
    	{
        	//Console.WriteLine("static extension rocks");
        	return "";
    	}
	}
	*/
}
