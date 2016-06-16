/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 25.02.2016
 * Time: 14:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WPF_S39_Commander.WH;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for WatchUserSettingsControl.xaml
	/// </summary>
	public partial class WatchUserSettingsControl : UserControl, INotifyPropertyChanged 
	{
		public WatchUserSettingsControl()
		{
			InitializeComponent();
			this.DataContext = this.User;
			SetCurrentValue (UserProperty, new LogSettingsUser("")) ;
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		public static readonly DependencyProperty UserProperty = DependencyProperty.Register("User", typeof(LogSettingsUser), typeof(WatchUserSettingsControl), new UIPropertyMetadata(new LogSettingsUser(""), OnUserPropertyChanged));
		public static void OnUserPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	    {
		    var myUserBox = dependencyObject as WatchUserSettingsControl;  
			myUserBox.OnUserPropertyChanged(e);
		    myUserBox.OnPropertyChanged("User");  
	    }
		public WH.LogSettingsUser User {
			get { return (LogSettingsUser)GetValue(UserProperty); }
			set { 	SetValue(UserProperty, value);
					OnPropertyChanged(new DependencyPropertyChangedEventArgs(WatchUserSettingsControl.UserProperty, null, value));
			}
		}
		private void OnUserPropertyChanged(DependencyPropertyChangedEventArgs e) 
		{ 
			//TextBox1.Text = TextBoxText;
			SetSubProperties((LogSettingsUser)e.NewValue);
		}
		private void BirthdayDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			var sel = this.BirthdayDatePicker.SelectedDate;
			if (sel != null)
			{
				var dt = sel.Value;
				this.birthdayYearBox.TextBoxText = (string)dt.Year.ToString();
				this.birthdayMonthyBox.TextBoxText = (string)dt.Month.ToString();
				this.birthdayDayBox.TextBoxText = (string)dt.Day.ToString();
			}
			OnPropertyChanged("BirthdayDate");
		}
		
//		private int _Goal = 100;
//	    [System.ComponentModel.DefaultValue(100)]
//		public int Goal
//		{
//		    get { return _Goal; } 
//		    set { _Goal = value; OnPropertyChanged("Goal"); }
//		}
		
		public void SetSubProperties(WH.LogSettingsUser p)
		{
			this.StrideBox.TextBoxText = System.Convert.ToString(p.stride);
			this.RankBox.TextBoxText = System.Convert.ToString(p.rank);
			this.AgeBox.TextBoxText = System.Convert.ToString(p.age);
			this.GenderBox.TextBoxText = System.Convert.ToString(p.gender);

			this.WeightBox.TextBoxText = System.Convert.ToString(p.weight);
			this.HeightBox.TextBoxText = System.Convert.ToString(p.height);
			
			this.birthdayDayBox.TextBoxText = System.Convert.ToString(p.birthdayDay);
			this.birthdayMonthyBox.TextBoxText = System.Convert.ToString(p.birthdayMonth);
			this.birthdayYearBox.TextBoxText = System.Convert.ToString(p.birthdayYear);
			
			this.BirthdayDatePicker.SelectedDate = p.GetBirthdayDate();
		}
		
		public string GetSettings()
		{
			var ret = string.Join(",", new string[]{StrideBox.TextBoxText, 
			                      	HeightBox.TextBoxText, WeightBox.TextBoxText, 
			                      	GenderBox.TextBoxText, AgeBox.TextBoxText, 
			                      	birthdayDayBox.TextBoxText, birthdayMonthyBox.TextBoxText, birthdayYearBox.TextBoxText, 
			                      	RankBox.TextBoxText });
			
			return ret;
		}
		
		
		// Routed Event 
        public static readonly RoutedEvent ClickedButtonEvent = EventManager.RegisterRoutedEvent("ButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WatchUserSettingsControl));
		public event RoutedEventHandler ButtonClicked
		{
		    add { AddHandler(ClickedButtonEvent, value); } 
		    remove { RemoveHandler(ClickedButtonEvent, value); }
		}        
        private void RaiseClickedButtonEvent()
	    {
	        var newEventArgs = new RoutedEventArgs(WatchUserSettingsControl.ClickedButtonEvent);
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