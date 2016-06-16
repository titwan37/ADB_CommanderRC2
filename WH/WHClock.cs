/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 01.04.2016
 * Time: 19:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

namespace WPF_S39_Commander.WH
{
	public enum DTEnum : int { All=0, Year=1, Month=2, Day=3, Hours=4, Minutes=5, Seconds=6, MilliSeconds=7 }
	/// <summary>
	/// Description of WHClock.
	/// </summary>
	public class WHClock : INotifyPropertyChanged
	{
		
		#region Events
        public event DateTimeChangedEventHandler OnDateChanged;
		public event DateTimeChangedEventHandler OnDateTimeChanged;
		
		public event PropertyChangedEventHandler PropertyChanged;
		/*
		protected virtual void OnPropertyChanging(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		*/
		protected virtual void OnPropertyChanged(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		
        public bool HasDateChanged(DateTime oldDate, DateTime newDate)
        {
        	return (!oldDate.Date.Equals(newDate.Date));
        }
        protected virtual void RaiseDateChanged(object sender, DateTimeChangedEventArgs e)
        {
	            if(e.HasChanged_Day)
	            	if (OnDateChanged!=null) OnDateChanged(this, e);
        }		
		protected virtual void RaiseDateTimeChanged(object sender, DateTimeChangedEventArgs e)
		{
		    if (OnDateTimeChanged != null)
		    	if (e.HasChanged_HMS)
		        	OnDateTimeChanged(this, e);
		}

		public delegate void DateTimeChangedEventHandler(object sender, DateTimeChangedEventArgs e);
		public class DateTimeChangedEventArgs : EventArgs 
		{
			public DateTimeChangedEventArgs(DateTime old, DateTime date)
			{ 	OldDate=old; 
				Date=date;
				HasChanged_Day = !old.Date.Equals(date.Date);
				HasChanged_Time = !old.TimeOfDay.Equals(date.TimeOfDay);
				//if (HasChanged_Time)
				HasChanged_HMS = !(old.Hour.Equals(date.Hour)&&old.Minute.Equals(date.Minute)&&old.Second.Equals(date.Second));
				
				HasChanged_Hours = !(old.Hour.Equals(date.Hour));
				HasChanged_Minutes = !(old.Minute.Equals(date.Minute));
				HasChanged_Seconds = !(old.Second.Equals(date.Second));
			}
			public bool HasChanged_Day; 
			public bool HasChanged_Time;
			public bool HasChanged_HMS;
			public bool HasChanged_Hours;
			public bool HasChanged_Minutes;
			public bool HasChanged_Seconds;
			public DateTime OldDate {get;set;} 
			public DateTime Date{get;set;} 
		}
        #endregion
		
        #region properties
		/// <summary>
		/// Datetime
		/// </summary>
		public DateTime DT { get {	return _dt;	} set { SetDateTime(DTEnum.All, value);	} } 
		DateTime _dt = DateTime.MinValue;
		public TimeSpan Freq 	{get; set;}
		
		
		public DTEnum LastChange { get; private set; }
		private static readonly object UnitChangedKey = new object();
//	    public event EventHandler UnitChanged
//	    {
//	        add { Events.AddHandler(UnitChangedKey, value);}
//	        remove {Events.AddHandler(UnitChangedKey, value);}
//	    }
		
		/// <summary>
		/// SetDateTime
		/// </summary>
		/// <param name="prop">enum Datetime member</param>
		/// <param name="value"></param>
		public bool SetDateTime(DTEnum pro, object value)
		{
			bool changed = false;
			int ipro =(int)pro;
			
			if (ipro == 0) //(int)DTEnum.All
				if (!value.Equals(_dt)) {  try {this._dt = new DateTime(((DateTime)value).Ticks); }catch{}; changed= true; OnPropertyChanged(pro.ToString());}
			// date
			if(ipro==1) //(int)DTEnum.Year
				if (!value.Equals(Year)) {  this._dt = new DateTime((int)value, DT.Month, DT.Day, DT.Hour, DT.Minute, DT.Second, DT.Millisecond); changed= true; OnPropertyChanged(pro.ToString());}
			if(ipro==2)// (int)DTEnum.Month
				if (!value.Equals(Month)) {	this._dt = new DateTime(DT.Year, (int)value, DT.Day, DT.Hour, DT.Minute, DT.Second, DT.Millisecond); changed= true; OnPropertyChanged(pro.ToString());}
			if (ipro==3) //(int)DTEnum.Day 
				if (!value.Equals(Day)) {	this._dt = new DateTime(DT.Year, DT.Month, (int)value, DT.Hour, DT.Minute, DT.Second, DT.Millisecond); changed= true; OnPropertyChanged(pro.ToString());}
			
			// time
			if (ipro==4) //(int)DTEnum.Hours 
				if (!value.Equals(Hours)) {	this._dt = new DateTime(DT.Year, DT.Month, DT.Day, (int)value, DT.Minute, DT.Second, DT.Millisecond); changed= true; OnPropertyChanged(pro.ToString());}
			if (ipro==5) //(int)DTEnum.Minutes
				if (!value.Equals(Minutes)) {	this._dt = new DateTime(DT.Year, DT.Month, DT.Day, DT.Hour, (int)value, DT.Second, DT.Millisecond); changed= true; OnPropertyChanged(pro.ToString());}
			if (ipro==6) //(int)DTEnum.Seconds
				if (!value.Equals(Seconds)) {	this._dt = new DateTime(DT.Year, DT.Month, DT.Day, DT.Hour, DT.Minute, (int)value, DT.Millisecond); changed= true; OnPropertyChanged(pro.ToString());}
			
			if (changed)
				this.LastChange = pro;
			
			return changed;
		}
		
		/// <summary>
		/// Date
		/// </summary>
		public int Year 			{ get{return DT.Year;} 				set { SetDateTime(DTEnum.Year, value); } }
		public int Month 			{ get{return DT.Month;} 			set { SetDateTime(DTEnum.Month, value);} }
		public int Day 				{ get{return DT.Day;} 					set { SetDateTime(DTEnum.Day, value);  } }
		public string Date 			{ get{return _date = string.Format("{0:D}", DT);} 				set { _date = value; SetDateTime(DTEnum.Day, value); } } string _date = "Wednesday 25 November";
// alternatively 0:f
		/// <summary>
		/// Time
		/// </summary>
		public int Hours 			{ get{return DT.Hour;} 				set { SetDateTime(DTEnum.Hours, value); } }
		public int Minutes 			{ get{return DT.Minute;} 			set { SetDateTime(DTEnum.Minutes, value); } }
		public int Seconds 			{ get{return DT.Second;} 			set { SetDateTime(DTEnum.Seconds, value); } }
		public int MilliSeconds 	{ get{return DT.Millisecond;} 		set { SetDateTime(DTEnum.MilliSeconds, value); } }
		
		#endregion
		
		public WHClock()
		{
			DT = DateTime.MinValue;
			Freq = new TimeSpan(0, 0, 0, 0, 100); // 100 Milliseconds
		}
		
		bool running = false;
		public bool Running {
			get { return running; }
			set { running = value; }
		}		
		
		#region Timers
        //http://blogs.msdn.com/silverlight_sdk/archive/2008/03/27/make-a-silverlight-timer-silverlight-2.aspx
        private System.Windows.Threading.DispatcherTimer myDispatcherTimer;
        private System.Diagnostics.Stopwatch myStopWatch;
        
        public void StartTimer()
        {
        	if (Running==false) 
        	{
        		RestartTimer();
        	}
        	else
        	{
        		// NOPE !!!
        		//StopTimer();
        	}
        }
        
        public void RestartTimer()
        {
        	if (myDispatcherTimer!=null) myDispatcherTimer=null;
        	
        	myDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            myDispatcherTimer.Interval = Freq; // 100 Milliseconds
            myDispatcherTimer.Tick += new EventHandler(Each_Tick);
            myDispatcherTimer.Start();

            myStopWatch = new System.Diagnostics.Stopwatch();
    		myStopWatch.Restart();

    		this.Running = true;
        }
        
        public void StopTimer()
        {
			if (Running) 
			{
        		if (myDispatcherTimer!=null)
	        		myDispatcherTimer.Stop();

        		if (myStopWatch!=null)
					myStopWatch.Stop();

        		Running = false;
			}
        }
        
        public void ResetTimer()
        {
        	this.StopTimer();
        	this.ChangeDateTime(DT, DateTime.MinValue);
        }

        // Fires every 1000 miliseconds while the DispatcherTimer is active.
        public void Each_Tick(object o, EventArgs sender)
        {
        	if (Running) 
        	{
        		var e = RunningClockTime();
	            
        		// if date changed
	            if (e.HasChanged_Day)
	            	RaiseDateChanged(this, e);
	            
	            // raise event
	            if (e.HasChanged_HMS)
	            	RaiseDateTimeChanged(this, e);
        	}
        }

		private DateTimeChangedEventArgs RunningClockTime()
		{
    		var elapsedTime = myStopWatch.Elapsed;
    		
    		var old = DT;
    		var tt = DT.Add(elapsedTime);
    		
    		myStopWatch.Restart();
    		
    		ChangeDateTime(old, tt);
        	
    		return new DateTimeChangedEventArgs(old, tt);
		}
        
		public void ChangeDateTime(DateTime old, DateTime date)
		{
        	var e = new DateTimeChangedEventArgs(old, date);
			
        	this.DT = date;
			
			this.RaiseDateTimeChanged(this, e);
		}
        
        public DateTime GetDateTime()
        {
        	return DT;
        }
        #endregion
	}
}
