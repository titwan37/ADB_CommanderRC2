/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/22/2016
 * Time: 18:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for TimeSettingsControl.xaml
	/// </summary>
	public partial class TimeSettingsControl : UserControl, INotifyPropertyChanged
	{
		//public event EventHandler OnButtonClick;
		public event PropertyChangedEventHandler PropertyChanged;
	    protected void OnPropertyChanged(string name)
	    {
	        if (PropertyChanged != null)
	        {
	            PropertyChanged(this, new PropertyChangedEventArgs(name));
	        }
	    }
		
	    private string _dateTimeCulture = "EU";
	    [System.ComponentModel.DefaultValue("EU")]
		public string DateTimeCulture
		{
		    get { return _dateTimeCulture; } 
		    set { _dateTimeCulture = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DateTimeCulture")); }
		}
        private string _hourFormat = "24";
		[System.ComponentModel.DefaultValue("24")]
        public string HourFormat
		{
		    get { return _hourFormat; } 
		    set { _hourFormat = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("HourFormat")); }
		}
        private string _beep = "on";
		[System.ComponentModel.DefaultValue("on")]
        public string Beep
		{
		    get { return _beep; } 
		    set { _beep = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Beep")); }
		}
        
        private string _beatTime = "999";
		[System.ComponentModel.DefaultValue("999")]
        public string BeatTime
		{
		    get { return _beatTime; } 
		    set { _beatTime = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BeatTime"));}
		}
        
		public TimeSettingsControl()
		{
			InitializeComponent();
			this.DataContext = this;
		}

	}
}