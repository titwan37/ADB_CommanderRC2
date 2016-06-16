/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 03/09/2016
 * Time: 20:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace WPF_S39_Commander.Diagnostic
{
	/// <summary>
	/// Description of StopwatchCountDown.
	/// </summary>
	public class StopwatchCountDown : INotifyPropertyChanged
	{
		public StopwatchCountDown(bool autoStart) {Initialize(autoStart);}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		
		/// <summary>
		/// OnCountDownComplete that is raised when the count down completes
		/// </summary>
		public event EventHandler OnCountDownTick;
		public event EventHandler OnCountDownComplete;
		private DispatcherTimer _myTimer = new DispatcherTimer();
		public bool _running = false;
		private bool _autoStart = false;
	    private DateTime _startTime;
	    private DateTime _stopTime;
		public TimeSpan TimeSpanGoal { get; set; }
		public TimeSpan TimeSpanElapsed { get {return new TimeSpan(GetTimePassed());} }
		public double Percentage { get { return Divide(TimeSpanElapsed,TimeSpanGoal);} }
		
	    private void Initialize(bool autoStart)
		{
	    	_autoStart = autoStart;
			_myTimer.Interval = new TimeSpan(0,0,1);
			_myTimer.Tick += new EventHandler(MyTimerTick);
			if (autoStart) _myTimer.Start();
		}

	    private void MyTimerTick(object sender, EventArgs e)
		{
	    	if (_running) 
	    	{
				// get the time elapsed since we started the test 
				// start date was set somewhere else before this code is called
				TimeSpan span = this.TimeSpanElapsed;
			    if (span >= this.TimeSpanGoal)
			    {
			    	Stop();
			    	NotifyCountDownComplete();
			    }
			    if (OnCountDownTick!=null) OnCountDownTick(this, EventArgs.Empty);
	    	}
		}
		private void NotifyCountDownComplete()
		{
		    // raise event
		    if (OnCountDownComplete != null) OnCountDownComplete(this, EventArgs.Empty);
		}
		
		private static double Divide(TimeSpan dividend, TimeSpan divisor)
		{
		    return (double) dividend.Ticks / (double) divisor.Ticks;
		}
		
	    public long GetTimePassed() {
	        
	    	if(_running) 
	    	{ 	
	    		var now = DateTime.Now;
	            return (now - _startTime).Ticks;
	        } 
	    	else
	    	{
	            return (_stopTime - _startTime).Ticks;
	        }
	    }
	    
	    public void Start() 
	    {
	        _running = true;
	        _startTime = DateTime.Now;
	        if (!_autoStart) _myTimer.Start();
	    }
	
	    public void Stop() 
	    {
	        _stopTime = DateTime.Now;
	        _running = false;
	    }
	    
	    public void Dispose()
	    {
	    	if (_running) Stop();
	    	_myTimer = null;
	    }
	}
}
