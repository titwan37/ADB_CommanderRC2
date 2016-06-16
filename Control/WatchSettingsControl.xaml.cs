/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/23/2016
 * Time: 11:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WPF_S39_Commander.Diagnostic;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for WatchSettingsControl.xaml
	/// </summary>
	public partial class WatchSettingsControl : UserControl, INotifyPropertyChanged 
	{
		#region properties
		/// <summary>
		/// PropertyChangedEvent
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
	    protected void OnPropertyChanged(string name)
	    {
	        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
	    }

	    protected void NotifyPropertyChanged(string name)
	    {
	    	if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
	    }
		
	    private string _DistanceCulture = "km";
	    [System.ComponentModel.DefaultValue("km")]
		public string DistanceCulture
		{
		    get { return _DistanceCulture; } 
		    set { _DistanceCulture = value; NotifyPropertyChanged("DistanceCulture"); }
		}
		
	    private string _Beep = "on";
	    [System.ComponentModel.DefaultValue("on")]
		public string Beep
		{
		    get { return _Beep; } 
		    set { _Beep = value; NotifyPropertyChanged("Beep"); }
		}
		
	    private string _WatchName = "NoName";
	    [System.ComponentModel.DefaultValue("NoName")]
		public string WatchName
		{
		    get { return _WatchName; } 
		    set { _WatchName = value; WatchNameBox.Text = value; NotifyPropertyChanged("WatchName"); }
		    //
		}
		private string _WatchFlags = "-1";
	    [System.ComponentModel.DefaultValue("-1")]
		public string WatchFlags
		{
		    get { return _WatchFlags; } 
		    set { _WatchFlags = value; WatchFlagBox.Text = value; NotifyPropertyChanged("WatchFlags"); }
		}
	    private string _MacAddress = "AA:AA:AA:AA:AA:AA";
	    [System.ComponentModel.DefaultValue("AA:AA:AA:AA:AA:AA")]
		public string MacAddress
		{
		    get { return _MacAddress; } 
		    set { _MacAddress = value; WatchMACBox.Text = value; NotifyPropertyChanged("MacAddress"); }
		}	
	    private string _ConexStatus = "Unknown";
	    [System.ComponentModel.DefaultValue("Unknown")]
		public string ConexStatus
		{
		    get { return _ConexStatus; } 
		    set { _ConexStatus = value; ConexStatusBox.Text = value; NotifyPropertyChanged("ConexStatus"); }
		}
		
		private IList<string> _RingtoneList = new List<string>();
		public IList<string> RingtoneList
		{
		    get { return _RingtoneList; } 
		    set { _RingtoneList = value; NotifyPropertyChanged("Ringtone"); }
		}			
		
	    [System.ComponentModel.DefaultValue("on")]
		public string Menu1
		{
		    get { return _Menu1; } 
		    set { _Menu1 = value; NotifyPropertyChanged("Menu1Settings"); }
		} private string _Menu1 = "on";

		[System.ComponentModel.DefaultValue("on")]
		public string Menu2
		{
		    get { return _Menu2; } 
		    set { _Menu2 = value; NotifyPropertyChanged("Menu2Settings"); }
		} private string _Menu2 = "on";

		[System.ComponentModel.DefaultValue("on")]
		public string Menu3
		{
		    get { return _Menu3; } 
		    set { _Menu3 = value; NotifyPropertyChanged("Menu3Settings"); }
		} private string _Menu3 = "on";		
		#endregion
		
		public WatchSettingsControl()
		{
			//RingtoneList = new List<string>(){"0", "1", "2", "3"};
			InitializeComponent();
			this.DataContext = this;
			this.RingtoneList =  UtilEnum<WH.RINGTONE>.GetNames();
			this.RingtoneListView.ItemsSource = RingtoneList;
			this.RingtoneListView.SelectedIndex = 0;
		}

		public string GetSettings1()
		{
			return string.Format("{0},{1}", Beep, DistanceCulture);
		}
		public string GetSettings2()
		{
			return string.Format("{0}", WatchName);
		}
		public string GetSettings3()
		{
			var timer_ringtone = 0;
			var selectedRingtone = RingtoneListView.SelectedValue.ToString();
			if (selectedRingtone!="") timer_ringtone = (int)UtilEnum<WH.RINGTONE>.ParseEnum(selectedRingtone);
			return string.Format("{0},{1},{2},{3}", timer_ringtone, Menu1, Menu2, Menu3);
		}
		
		#region Events
		
		private void RingtoneListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Ringtone"));
		}
		
		private void cbAllFeatures_CheckedChanged(object sender, RoutedEventArgs e)
        {
                /*
  				bool newVal = (cbAllFeatures.IsChecked == true);
                cbMO.IsChecked = newVal;
                cbTU.IsChecked = newVal;
                cbWE.IsChecked = newVal;
                cbTH.IsChecked = newVal;
                cbFR.IsChecked = newVal;
                cbSA.IsChecked = newVal;
                cbSU.IsChecked = newVal;
                */
        }
		        
        private void cbFeature_CheckedChanged(object sender, RoutedEventArgs e)
        {
            /*
        	cbAllFeatures.IsChecked = null;
            if((cbMO.IsChecked == true) && (cbTU.IsChecked == true) && (cbWE.IsChecked == true)&& (cbTH.IsChecked == true)&& (cbFR.IsChecked == true)&& (cbSA.IsChecked == true)&& (cbSU.IsChecked == true))
                    cbAllFeatures.IsChecked = true;
            if((cbMO.IsChecked == false) && (cbTU.IsChecked == false) && (cbWE.IsChecked == false)&& (cbTH.IsChecked == false)&& (cbFR.IsChecked == false)&& (cbSA.IsChecked == false)&& (cbSU.IsChecked == false))
                    cbAllFeatures.IsChecked = false;
             */
        }
        
        // Routed Event 
        public static readonly RoutedEvent ClickedButtonEvent = EventManager.RegisterRoutedEvent("ButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WatchSettingsControl));
		public event RoutedEventHandler ButtonClicked
		{
		    add { AddHandler(ClickedButtonEvent, value); } 
		    remove { RemoveHandler(ClickedButtonEvent, value); }
		}        
        
		private void RaiseClickedButtonEvent()
	    {
	        RoutedEventArgs newEventArgs = new RoutedEventArgs(WatchSettingsControl.ClickedButtonEvent);
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
        #endregion
	}
}