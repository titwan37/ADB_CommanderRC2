/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/16/2016
 * Time: 19:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPF_S39_Commander.WH;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for DigitalClockSimple.xaml
	/// </summary>
	public partial class DigitalClockSimple : UserControl
	{
        public event EventHandler OnDateChanged;
        public event EventHandler OnTimeChanged;
		
        public WHClock Clock = null; // = new WHClock();
        
        public DateTime DT 		{get {return Clock.DT;} 	set {Clock.DT = value;}}
        public TimeSpan Freq 	{get {return Clock.Freq;} 	set {Clock.Freq = value;}}
		
        public string Date 			{get {return Clock.Date;} private set {Clock.Date = value;}}
		public int Hours 			{get {return Clock.Hours;} private set {Clock.Hours = value;}}
		public int Minutes 		{get {return Clock.Minutes;} private set {Clock.Minutes = value;}}
		public int Seconds 		{get {return Clock.Seconds;} private set {Clock.Seconds = value;}}
		public int MSeconds 		{get {return Clock.MilliSeconds;} private set {Clock.MilliSeconds = value;}}
		
		public DigitalClockSimple()
		{
            // Required to initialize variables
            InitializeComponent();
            //this.DataContext = this;
            this.SetNewWatchClock(new WHClock());
        }


		public void SetNewWatchClock(WHClock newClock)
		{
			this.Clock = null;
			this.Clock = newClock;
			this.Clock.OnDateChanged+=OnDateChangedEventHandler;
			this.Clock.OnDateTimeChanged+=OnDateTimeChangedEventHandler;
			this.Clock.PropertyChanged +=OnDateTimePropertyChangedEventHandler;
			
			LayoutRootEditable.DataContext = this.Clock;
            LayoutRootEditable.DataContext = this.Clock;
            LayoutRoot.DataContext = this.Clock;
            DateTextBlock.DataContext = this.Clock;

            this.Clock.DT = DateTime.MinValue;
		}

		void OnDateChangedEventHandler(object sender, WHClock.DateTimeChangedEventArgs e)
		{
//        	if (EditMode==false && UpdateDisplay==false) 
//				this.SetTimeToDisplay();
			if (OnDateChanged!=null)
				OnDateChanged(sender, e);
		}

		void OnDateTimeChangedEventHandler(object sender, WHClock.DateTimeChangedEventArgs e)
		{
        	if (EditMode==false && UpdateDisplay==false) 
				this.SetTimeToDisplay();
			if (OnTimeChanged!=null)
				OnTimeChanged(sender, e);
			if (e.HasChanged_Day)
				OnDateChangedEventHandler(sender, e);

		}
		void OnDateTimePropertyChangedEventHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var cl = (WHClock)sender;
			var prop = (DTEnum)UtilEnum<DTEnum>.ParseEnum(e.PropertyName);
			if (prop==DTEnum.All)
			{

			}
//			else if (prop==DTEnum.Hours) {
//				this.hourText.Text = cl.DT.Hour;
//			} 
		}
		


		
		#region Timers
        
        public void StartTimer()
        {
    		Clock.RestartTimer();
        }
        
        public void StopTimer()
        {
			if (Clock.Running) 
			{
        		Clock.Running = false;
			}
        }
        
        public void ResetTimer()
        {
        	this.Clock.ResetTimer();
            this.SetTimeToDisplay();
        }

        public void RaiseDateChangeEvent(DateTime oldDate, DateTime newDate)
        {
	            if(HasDateChanged(oldDate, newDate))
	            	if (OnDateChanged!=null) OnDateChanged(newDate, null);
        }
        public bool HasDateChanged(DateTime oldDate, DateTime newDate)
        {
        	return (!oldDate.Date.Equals(newDate.Date));
        }
        
        public bool UpdateDisplay = false;        	
        public void SetTimeToDisplay()
        {
        	SetTimeToDisplay(this.DT);
        }
        public void SetTimeToDisplay(DateTime dt)
        {
        	var oldDate = this.DT;
        	var e = new WHClock.DateTimeChangedEventArgs(oldDate, dt);
        	//if (e.HasChanged_Time | e.HasChanged_Day)
        	{
	        	this.UpdateDisplay = true;
	//        	// date
				this.DT = dt;
				
				UpdateTimeOnDisplay(Clock.Date, dt);
	
	        	this.UpdateDisplay = false;
        	}
        	//RaiseDateChangeEvent(oldDate, dt);
        }
        
        void UpdateTimeOnDisplay(string date, DateTime dt)
		{
				this.DateTextBlock.Text = date;
				this.hourText.Text = string.Format("{0:00}", dt.Hour);
				this.minuteText.Text = string.Format("{0:00}", dt.Minute);
				this.secondText.Text = string.Format("{0:00}", dt.Second);
				this.msecondText.Text = string.Format("{0:000}", dt.Millisecond);
		}
        
        public DateTime GetTimeFromDisplay()
        {
//        	Hours = System.Convert.ToInt32(hourText.Text);
//        	Minutes = System.Convert.ToInt32(minuteText.Text);
//        	Seconds = System.Convert.ToInt32(secondText.Text);
//        	MSeconds = System.Convert.ToInt32(msecondText.Text);
        	
			var  ob = this.Clock.DT;

			var dt = new DateTime(ob.Year, ob.Month, ob.Day, ob.Hour, ob.Minute, ob.Second, ob.Millisecond);
        	return dt;
        }
		public void ChangeDateTime(DateTime dt)
		{
			this.EditMode = true;
			var old = DT;
			this.Clock.ChangeDateTime(DT, dt);
			this.EditMode = false;
			
			RaiseDateChangeEvent(old, dt);
		}
		
        private string twoDigit(string input)
        {
            if (input.Length < 2)
                return "0" + input;
            else
                return input;
        }
		private void SetfromEditorToDisplay()
		{
//		   		hourText.Text = hourTextEdit.Text;
//		   		minuteText.Text = minuteTextEdit.Text;
//		   		secondText.Text = secondTextEdit.Text;
		   		
			this.Hours = System.Convert.ToInt32(hourTextEdit.Text);
			this.Minutes = System.Convert.ToInt32(minuteTextEdit.Text);
			this.Seconds = System.Convert.ToInt32(secondTextEdit.Text);
			//this.MSeconds = System.Convert.ToInt32(hourTextEdit.Text);
		   	
		}   
		private void SetfromDisplayToEditor()
		{
//	       		hourTextEdit.Text= hourText.Text;
//	       		minuteTextEdit.Text= minuteText.Text;
//	       		secondTextEdit.Text= secondText.Text;
			
			hourTextEdit.Text = this.Hours.ToString();
			minuteTextEdit.Text = this.Minutes.ToString();
			secondTextEdit.Text = this.Seconds.ToString();
		}		

    
        #endregion
       
        
        #region Editable Clock
		bool editableTime = true;
		public bool EditableTime {
			get { return editableTime; }
			set { editableTime = value; }
		}
		bool editableDate = true;
		public bool EditableDate {
			get { return editableDate; }
			set { editableDate = value; }
		}		
		bool running = false;
		public bool Running {
			get { return running; }
			set { running = value; }
		}
		bool editMode = false;
		public bool EditMode {
			get { return editMode; }
			set { editMode = value; }
		}

		public Visibility DateVisibility {
        	get { return this.DateTextBlock.Visibility; }
        	set { this.DateTextBlock.Visibility = value; }
        }

		private void OnTextBlockDate_MouseDown(object sender, MouseButtonEventArgs e)
		{
       		if (EditableDate) 
       		{
	       		ToggleDate(true);
        	}
        }
        private void DateEditor_LostFocus(object sender, RoutedEventArgs e)
		{
        	ToggleDate(false);
        	this.DateTextBlock.Text = DateEditor.SelectedDate.Value.ToString();
		}
       	private void OnTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
       		if (EditableTime) 
       		{
	       		TogglePanel(true);
	
	       		if (sender is TextBlock)
	       		{
	       			var ctr = (TextBlock)sender;
	       			var ctrtag = ctr.Tag.ToString();
	       			
	       			SetfromDisplayToEditor();
		       		this.EditMode = true;
	
			       		if (ctrtag == "hour"){
			       		hourTextEdit.Text= hourText.Text;
			       		hourTextEdit.Focus();
	       			}
	       			else if (ctrtag == "min") {
			       		minuteTextEdit.Text= minuteText.Text;
			       		minuteTextEdit.Focus();
	       			} 
	       			else if (ctrtag == "sec") {
			       		secondTextEdit.Text= secondText.Text;
			       		secondTextEdit.Focus();
	       			}
	       		}
       		}
		}
       	private void OnTextBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			//TogglePanel();
       	}
		private void OnTextBlock_LostFocus(object sender, RoutedEventArgs e)
		{
			//TogglePanel();
		}
		private void OnTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (EditableTime) {
				TogglePanel(false);
	
				if (sender is TextBox)
	       		{
	       			var ctr = (TextBox)sender;
	       			var ctrtag = ctr.Tag.ToString();
	       			
	       			SetfromEditorToDisplay();
	
			       	if (ctrtag == "hour")	{
			       		hourText.Text = hourTextEdit.Text;
			       		hourText.Focus();
	       			}
	       			else if (ctrtag == "min") {
			       		minuteText.Text = minuteTextEdit.Text;
			       		minuteText.Focus();
	       			} 
	       			else if (ctrtag == "sec") {
			       		secondText.Text = secondTextEdit.Text;
			       		secondText.Focus();
	       			}
		       	
		       		var dt = GetTimeFromDisplay();
		       		if (!dt.Equals(DT)) {
		       			ChangeDateTime(dt);
		       		}
		       		this.EditMode=false;
	       		}	
			}
		}

		
		private void ToggleDate(bool editMode)
		{
			if (editMode) 
        	{
        		DateTextBlock.Visibility = Visibility.Collapsed;
        		DateEditor.Visibility = Visibility.Visible;
        	}
        	else
        	{
        		DateTextBlock.Visibility = Visibility.Visible;
        		DateEditor.Visibility = Visibility.Collapsed;
        	}
		}

		private void TogglePanel(bool editMode)
		{
			if (editMode) 
			{
				LayoutRootEditable.Visibility = Visibility.Visible;
	       		LayoutRoot.Visibility = Visibility.Collapsed;
			}
			else
			{
	       		LayoutRoot.Visibility = Visibility.Visible;
	       		LayoutRootEditable.Visibility = Visibility.Collapsed;
			}
		}
        #endregion
        
        
       
        // Routed Event 
        public static readonly RoutedEvent ClickedButtonEvent = EventManager.RegisterRoutedEvent("ButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DigitalClock));
		public event RoutedEventHandler ButtonClicked
		{
		    add { AddHandler(ClickedButtonEvent, value); } 
		    remove { RemoveHandler(ClickedButtonEvent, value); }
		}        
        private void RaiseClickedButtonEvent()
	    {
	        RoutedEventArgs newEventArgs = new RoutedEventArgs(DigitalClock.ClickedButtonEvent);
	        RaiseEvent(newEventArgs);
	    }

        public object BtnGetContent {
        	get { return BtnGet.Content; }
        	set { BtnGet.Content = value; }
        }
        public event System.Windows.RoutedEventHandler OnBtnGet_Click;
		public void BtnGet_Click(object sender, RoutedEventArgs e)
		{
			//RaiseClickedButtonEvent();
			if(OnBtnGet_Click!=null) OnBtnGet_Click(null,null);
		}
        public object BtnSetContent {
        	get { return BtnSet.Content; }
        	set { BtnSet.Content = value; }
        }
		public event System.Windows.RoutedEventHandler OnBtnSet_Click;
		public void BtnSet_Click(object sender, RoutedEventArgs e)
		{
			//RaiseClickedButtonEvent();
			if(OnBtnSet_Click!=null) OnBtnSet_Click(null,null);
		}
	}
}