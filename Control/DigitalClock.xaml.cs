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

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for DigitalClock.xaml
	/// </summary>
	public partial class DigitalClock : UserControl
	{
        public event EventHandler OnDateChanged;
		
		public DateTime DT 		{get; set;}
		public TimeSpan Freq 	{get; set;}
		
		public int Hours 		{get; set;}
		public int Minutes 		{get; set;}
		public int Seconds 		{get; set;}
		public int MSeconds 	{get; set;}
		
		public DigitalClock()
		{
			DT = DateTime.Now;
			Freq = new TimeSpan(0, 0, 0, 0, 100); // 100 Milliseconds
            // Required to initialize variables
            InitializeComponent();
            this.DataContext = this;
            //StartTimer(null, null);
        }

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
        	this.DT = DateTime.MinValue;
            this.SetTimeToDisplay();
        }

        // Fires every 1000 miliseconds while the DispatcherTimer is active.
        public void Each_Tick(object o, EventArgs sender)
        {
        	if (EditMode==false && UpdateDisplay==false) 
        	{
	        	var elapsedTime = myStopWatch.Elapsed;

        		var oldDate = this.DT.Date;
	        	var dt = this.DT.Add(elapsedTime);
	            this.SetTimeToDisplay(dt);
	            // if date changed
	            // raise event
	            RaiseDateChangeEvent(oldDate, dt);
        	}
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
        	var oldDate = DT;

        	this.UpdateDisplay = true;
        	
        	DateTextBlock.Text = string.Format("{0:D}", dt);
        	
        	hourText.Text = twoDigit(dt.Hour.ToString());
            minuteText.Text = twoDigit(dt.Minute.ToString());
            secondText.Text = twoDigit(dt.Second.ToString());
            msecondText.Text = twoDigit(dt.Millisecond.ToString());

            hourText1.Text = twoDigit(dt.Hour.ToString());
            minuteText1.Text = twoDigit(dt.Minute.ToString());
            secondText1.Text = twoDigit(dt.Second.ToString());
            msecondText1.Text = twoDigit(dt.Millisecond.ToString());
            
        	this.UpdateDisplay = false;

        	//RaiseDateChangeEvent(oldDate, dt);
        }
        
        public DateTime GetTimeFromDisplay()
        {
        	Hours = System.Convert.ToInt32(hourText.Text);
        	Minutes = System.Convert.ToInt32(minuteText.Text);
        	Seconds = System.Convert.ToInt32(secondText.Text);
        	MSeconds = System.Convert.ToInt32(msecondText.Text);

        	var dt = new DateTime(DT.Year, DT.Month, DT.Day,
        	                      Hours, Minutes, Seconds, MSeconds);

        	return dt;
        }

        private string twoDigit(string input)
        {
            if (input.Length < 2)
                return "0" + input;
            else
                return input;
        }
        
        #endregion
       
        
        #region Editable Clock
         public Visibility DateVisibility {
        	get { return this.DateTextBlock.Visibility; }
        	set { this.DateTextBlock.Visibility = value; }
        }
        private void OnTextBlockDate_MouseDown(object sender, MouseButtonEventArgs e)
		{
       		if (EditableDate) 
       		{
	       		//ToggleDate(true);
        	}
        }
//        private void DateEditor_LostFocus(object sender, RoutedEventArgs e)
//		{
//        	ToggleDate(false);
//        	this.DateTextBlock.Text = DateEditor.SelectedDate.Value.ToString();
//		}
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

		private void SetfromDisplayToEditor()
		{
	       		hourTextEdit.Text= hourText.Text;
	       		minuteTextEdit.Text= minuteText.Text;
	       		secondTextEdit.Text= secondText.Text;
		}		
		private void SetfromEditorToDisplay()
		{
		   		hourText.Text = hourTextEdit.Text;
		   		minuteText.Text = minuteTextEdit.Text;
		   		secondText.Text = secondTextEdit.Text;
		}
		
		public void ChangeDateTime(DateTime dt)
		{
			this.EditMode = true;
	       	this.DT = dt;
			this.EditMode = false;
		}
		
//		private void ToggleDate(bool editMode)
//		{
//			if (editMode) 
//        	{
//        		DateTextBlock.Visibility = Visibility.Collapsed;
//        		DateEditor.Visibility = Visibility.Visible;
//        	}
//        	else
//        	{
//        		DateTextBlock.Visibility = Visibility.Visible;
//        		DateEditor.Visibility = Visibility.Collapsed;
//        	}
//		}

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