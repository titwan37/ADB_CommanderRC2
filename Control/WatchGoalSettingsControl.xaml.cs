/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 29.02.2016
 * Time: 12:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WPF_S39_Commander.WH;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for WatchGoalSettingsControl.xaml
	/// </summary>
	public partial class WatchGoalSettingsControl : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		
//		WH.LogSettingsGoals goals;
//		public WH.LogSettingsGoals Goals {
//			get { 
//				goals.Goal = this.Goal;
//				goals.GoalState = this.GoalState=="on"? true: false;
//				return goals;
//			}
//			set { 
//				goals = value;
//				this.Goal = value.Goal;
//				this.GoalState = value.GoalState? "on" : "off";
//				OnPropertyChanged("Goals"); 
//			}
//		}
		
		private string _GoalState = "on";
	    [System.ComponentModel.DefaultValue("on")]
		public string GoalState
		{
			get { return _GoalState; }
		    set { _GoalState = value; OnPropertyChanged("GoalState"); }
		}
		
		private int _Goal = 100;
		[System.ComponentModel.DefaultValue(100)]
		public int Goal
		{
		    get { return _Goal; } 
		    set { _Goal = SetGoal(value); OnPropertyChanged("Goal"); }
		}
		
		int SetGoal(int value)
		{
			//this.goalBox.TextBoxText=value.ToString(); 
			return value;
		}
		
		[System.ComponentModel.DefaultValue("100")]
	    public string sGoal
		{
			get { return Goal.ToString(); }
			//set { Goal = System.Convert.ToInt32(value); OnPropertyChanged("sGoal"); }
		}
	    const string OFF = "off";
	    const string sZero = "0";
		/// <summary>
		/// GetSettings
		/// </summary>
		/// <example>SET_SETTINGS_GOALS,on,on,on,on,on,on,off,off,off,off,off,off,10,220,30,146,191,192,4482,1023,9293,1927,1929,8383</example>
		/// <returns></returns>
		public string GetSettings()
		{
			Goal = System.Convert.ToInt32(goalBox.TextBoxText);
//			var ret = string.Join(",", new string[]{GoalState,GoalState,GoalState,GoalState,GoalState,GoalState,
//													GoalState,GoalState,GoalState,GoalState,GoalState,GoalState,			                      	
//													sGoal, sGoal, sGoal, sGoal, sGoal, sGoal, sGoal, sGoal, sGoal, sGoal, sGoal, sGoal});
			var ret = string.Join(",", new string[]{OFF,OFF,OFF,OFF,OFF,OFF,
													GoalState,OFF,OFF,OFF,OFF,OFF,			                      	
													sZero, sZero, sZero, sZero, sZero, sZero, sGoal, sZero, sZero, sZero, sZero, sZero});
			
			return ret;
		}
		

		
        public event System.Windows.RoutedEventHandler OnBtnCal_Click;
		public void BtnCal_Click(object sender, RoutedEventArgs e)
		{
			//RaiseClickedButtonEvent();
			if(OnBtnCal_Click!=null) OnBtnCal_Click(null,null);
		}
		
		public void CalculateKCal(WHInfo myWH)
		{
			if (Dispatcher.Thread == System.Threading.Thread.CurrentThread) 
			{
				this.UpdateEnergyUI(myWH);
			}
			else
			{
				this.Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.ContextIdle, 
					new Action(() => UpdateEnergyUI(myWH)));
			}
			
			
		}
		
		public void UpdateEnergyUI(WHInfo myWH)
		{
			if (myWH!=null)
				this.Energy = myWH.GetTodayEnergy();

//			this.kcalBox.TextBoxText = Energy.KCAL;
//			this.WalkBox.TextBoxText = Energy.Dwalk;
//			this.RunBox.TextBoxText = Energy.Drun;
			
			//Console.Beep(500,500);
		}
		
		//public EnergyViewModel Energy { get; set; }
		public EnergyViewModel Energy
        {
            get { return (EnergyViewModel)GetValue(EnergyProperty); }
            set { SetValue(EnergyProperty, (EnergyViewModel)value); }
        }
        public static readonly DependencyProperty EnergyProperty =
            DependencyProperty.Register("Energy", typeof(EnergyViewModel), typeof(WatchGoalSettingsControl), 
        	                            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		
		public WatchGoalSettingsControl()
		{
			InitializeComponent();
			this.DataContext = this;
			//this.FitnessStack.DataContext = this.Energy;
			//this.cbGoal1.DataContext = this;
			//this.goalBox.DataContext = this;
		}
		
		// Routed Event 
        public static readonly RoutedEvent ClickedButtonEvent = EventManager.RegisterRoutedEvent("ButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WatchGoalSettingsControl));
		public event RoutedEventHandler ButtonClicked
		{
		    add { AddHandler(ClickedButtonEvent, value); } 
		    remove { RemoveHandler(ClickedButtonEvent, value); }
		}        
        private void RaiseClickedButtonEvent()
	    {
	        var newEventArgs = new RoutedEventArgs(WatchGoalSettingsControl.ClickedButtonEvent);
	        RaiseEvent(newEventArgs);
	    }
        
        public event System.Windows.RoutedEventHandler OnBtnGet_Click;
		public void BtnGet_Click(object sender, RoutedEventArgs e)
		{
			//RaiseClickedButtonEvent();
			if(OnBtnGet_Click!=null) OnBtnGet_Click(null,null);
		}
		public event System.Windows.RoutedEventHandler OnBtnSet_Click;
		public void BtnSet_Click(object sender, RoutedEventArgs e)
		{
			//RaiseClickedButtonEvent();
			if(OnBtnSet_Click!=null) OnBtnSet_Click(null,null);
		}
	}
}