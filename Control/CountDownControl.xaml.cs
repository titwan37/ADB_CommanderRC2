/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 03/09/2016
 * Time: 13:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for CountDownControl.xaml
	/// </summary>
	public partial class CountDownControl : UserControl
	{
		public CountDownControl()
		{
			this.InitializeComponent();
			this.DataContext = this;
			this.SegmentBackground.SegmentColor = new SolidColorBrush(Colors.LightBlue);
			this.SegmentBackground.Percentage = 0d;
			this.SegmentForeground.SegmentColor = new RadialGradientBrush(Colors.YellowGreen, Colors.Green);
			this.SegmentForeground.Percentage = 0d;
			this.StartTime = DateTime.Now;
			this.Percentage = 0d;
			this.initTimerSimple();
			//this.Init();
		}
		
		#region TimerComplex
		protected DateTime StartTime { get; set; }
		/// <summary>
		/// TimeSpan Goal
		/// </summary>
		public TimeSpan TimeSpanGoal {
			get { return _timeSpanGoal; }
			set { _timeSpanGoal = value; TimeSpanCurrent = value; }
		}
		
		private TimeSpan _timeSpanGoal;
		protected TimeSpan TimeSpanCurrent {
			get { return _timeSpanCurrent; }
			set { _timeSpanCurrent = value; Percentage= Divide(TimeSpanCurrent,TimeSpanGoal); }
		}
		private TimeSpan _timeSpanCurrent;
		private static double Divide(TimeSpan dividend, TimeSpan divisor)
		{
		    return (double) dividend.Ticks / (double) divisor.Ticks;
		}
		/// <summary>
		/// TimeFormat for formatting the elapsed time displayed
		/// </summary>
		public string TimeFormat  	{ get; set; }
		/// <summary>
		/// IsStarted to start/stop the timer
		/// </summary>
		public bool IsStarted = false;
		/// <summary>
		/// IsCountDown to increase or decrease the TimeSpan at each tick
		/// </summary>
		public bool IsCountDown = true;
		/// <summary>
		/// OnCountDownComplete that is raised when the count down completes
		/// </summary>
		public event EventHandler OnCountDownComplete;
		//public static event EventHandler OnTick;
		
//		private static Timer _UpdateTimer = new Timer(new TimerCallback(UpdateTimer), null, 1000, 1000);
//		private static void UpdateTimer(object state)
//		{
//		    EventHandler onTick = OnTick;
//		    if (onTick != null) onTick(null, EventArgs.Empty);
//		}
//		
//		private void Init()
//		{
//		    this.Loaded += new RoutedEventHandler(TimerBlock_Loaded);
//		    this.Unloaded += new RoutedEventHandler(TimerBlock_Unloaded);
//		}
//	
//		Action _UpdateTimeInvoker; // defines a delegate
//		
//		void TimerBlock_Loaded(object sender, RoutedEventArgs e)
//		{
//		    Binding binding = new Binding("TimeSpan");
//		    binding.Source = this;
//		    binding.Mode = BindingMode.OneWay;
//		    binding.StringFormat = TimeFormat;
//		
//		    //SetBinding(TextProperty, binding);
//		
//		    _UpdateTimeInvoker = new Action(() => UpdateTime());
//		
//		    OnTick += new EventHandler(TimerBlock_OnTick);
//		}
//		private void TimerBlock_OnTick(object sender, EventArgs e)
//		{
//			Dispatcher.Invoke(_UpdateTimeInvoker);
//		}		
//		
//		private void TimerBlock_Unloaded(object sender, RoutedEventArgs e)
//		{
//		    OnTick -= new EventHandler(TimerBlock_OnTick);
//		}
		
		private void UpdateTime()
		{
		    if (IsStarted)
		    {
		        TimeSpan step = TimeSpan.FromSeconds(1);
		        if (IsCountDown)
		        {
		            if (TimeSpanCurrent >= TimeSpan.FromSeconds(1))
		            {
		                TimeSpanCurrent -= step;
		                if (TimeSpanCurrent.TotalSeconds <= 0)
		                {
		                    TimeSpanCurrent = TimeSpan.Zero;
		                    IsStarted = false;
		                    NotifyCountDownComplete();
		                }
		            }
		        }
		        else
		        {
		            TimeSpanCurrent += step;
		        }
		    }
		}
		
		private void NotifyCountDownComplete()
		{
		    // raise event
		    if (OnCountDownComplete != null)
		        OnCountDownComplete(this, EventArgs.Empty);
		}
		#endregion
		
		#region  TimeSimple
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
		    PropertyChangedEventHandler e = PropertyChanged;
		    if (e != null)	{ e(this, new PropertyChangedEventArgs(propertyName));}
		}	
		private void initTimerSimple()
		{
			_myTimer.Interval = new TimeSpan(0,0,1);
			_myTimer.Tick += new EventHandler(MyTimerTick);
			_myTimer.Start();
		}
		public DispatcherTimer _myTimer = new DispatcherTimer();
		public void TimerRestart(object sender, SelectionChangedEventArgs e)
		{
		    _myTimer.Stop();
		    this.StartTime = DateTime.Now;
		    _myTimer.Start();
		}
		private void MyTimerTick(object sender, EventArgs e)
		{
			var now = DateTime.Now;
			// get the time elapsed since we started the test 
			// start date was set somewhere else before this code is called
			TimeSpan span = now - StartTime;
		    if (span >= TimeSpanGoal)
		    {
		    	NotifyCountDownComplete();
		    	TimerRestart(sender, null);
		    }
		}
		#endregion
		
		public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }
	    // Using a DependencyProperty as the backing store for Percentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(CountDownControl), 
        	                            new PropertyMetadata(65d, new PropertyChangedCallback(OnPercentageChanged)));
										//new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static void OnPercentageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CountDownControl circle = sender as CountDownControl;
            circle.SegmentBackground.Percentage = 100d;
            circle.SegmentStripe1.Percentage = circle.Percentage*100d;
            circle.SegmentForeground.Percentage = circle.Percentage*100d;
            //circle.Label1.Content = circle.Percentage.ToString("P0");
            //circle.Angle = (circle.Percentage * 360) / 100;
        }
        
        public void SetCountDownCounter(TimeSpan timespan)
		{
			this.TimeSpanGoal = timespan;
			this.TimeSpanCurrent = TimeSpanGoal;
		}
        
		private void Start_Click(object sender, RoutedEventArgs e)
		{
			this.IsStarted = true;
			this.IsCountDown = true;
			UpdateTime();
		}
	}
}