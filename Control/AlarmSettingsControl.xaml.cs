/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/22/2016
 * Time: 18:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WPF_S39_Commander.WH;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for AlarmSettingsControl.xaml
	/// </summary>
	public partial class AlarmSettingsControl : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
	    protected void OnPropertyChanged(string name)
	    {
	        if (PropertyChanged != null)
	        {
	            PropertyChanged(this, new PropertyChangedEventArgs(name));
	        }
	    }
	    protected void NotifyPropertyChanged(string name)
	    {
	    	if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
	    }
		
		public string MO
		{
		    get { return _MO; } 
		    set { _MO = value; NotifyPropertyChanged("AlarmRepetitionSettings"); }
		} private string _MO = "off";
		public string TU
		{
		    get { return _TU; } 
		    set { _TU = value; NotifyPropertyChanged("AlarmRepetitionSettings"); }
		} private string _TU = "off" ;
		public string WE
		{
		    get { return _WE; } 
		    set { _WE = value; NotifyPropertyChanged("AlarmRepetitionSettings"); }
		} private string _WE = "off" ;
		public string TH
		{
		    get { return _TH; } 
		    set { _TH = value; NotifyPropertyChanged("AlarmRepetitionSettings"); }
		} private string _TH = "off" ;
		public string FR
		{
		    get { return _FR; } 
		    set { _FR = value; NotifyPropertyChanged("AlarmRepetitionSettings"); }
		} private string _FR = "off" ;
		public string SA
		{
		    get { return _SA; } 
		    set { _SA = value; NotifyPropertyChanged("AlarmRepetitionSettings"); }
		} private string _SA = "off" ;
		public string SO
		{
		    get { return _SO; } 
		    set { _SO = value; NotifyPropertyChanged("AlarmRepetitionSettings"); }
		} private string _SO  = "off";
	    /// <summary>
	    /// AlarmStatus
	    /// </summary>
		public string AlarmStatus
		{
		    get { return _AlarmStatus; } 
		    set { _AlarmStatus = value; NotifyPropertyChanged("AlarmStatus"); }
		} private string _AlarmStatus  = "off";
		
		public string AlarmRingtone
		{
		    get { return _AlarmRingtone; } 
		    set { _AlarmRingtone = value; NotifyPropertyChanged("AlarmRingtone"); }
		} private string _AlarmRingtone  = WH.RINGTONE.RINGTONE_0.ToString();
		
		private IList<string> _RingtoneList = new List<string>();
		public IList<string> RingtoneList
		{
		    get { return _RingtoneList; } 
		    set { _RingtoneList = value; NotifyPropertyChanged("AlarmRingtone"); }
		}	
		
		public AlarmSettingsControl()
		{
			InitializeComponent();
			this.DataContext = this;
			this.stackCheckBoxes.DataContext = this;
			
			this.RingtoneList =  UtilEnum<WH.RINGTONE>.GetNames();
			this.RingtoneListView.ItemsSource = RingtoneList;
			this.RingtoneListView.SelectedIndex = 0;
		}
		
		public bool SetAlarmOptions(WH.WHInfo wh)
		{
			this.AlarmStatus = wh.Time.alarmStatus;
			this.AlarmRingtone = wh.Time.sAlarmRingtone;
			if (wh.Time.alarmRepeat!=null){
				var spt = wh.Time.alarmRepeat.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
				return SetAlarmRepetitionSettings(spt);
			}
			else
				return false;
		}
		
		public bool SetAlarmRepetitionSettings(string[] split)
		{	
			if (split.Length >= 7) {
				MO=split[0];
				TU=split[1];
				WE=split[2];
				TH=split[3];
				FR=split[4];
				SA=split[5];
				SO=split[6];
				return true;
			}
			return false;
		}
		public string GetAlarmRepetitionSettings()
		{
			return string.Join(",",MO,TU,WE,TH,FR,SA,SO);
		}
		
        private void cbAllFeatures_CheckedChanged(object sender, RoutedEventArgs e)
        {
                bool newVal = 
                	(cbAllFeatures.IsChecked == true);
                cbMO.IsChecked = newVal;
                cbTU.IsChecked = newVal;
                cbWE.IsChecked = newVal;
                cbTH.IsChecked = newVal;
                cbFR.IsChecked = newVal;
                cbSA.IsChecked = newVal;
                cbSU.IsChecked = newVal;
        }
		        
        private void cbFeature_CheckedChanged(object sender, RoutedEventArgs e)
        {
            cbAllFeatures.IsChecked = null;
            if((cbMO.IsChecked == true) && (cbTU.IsChecked == true) && (cbWE.IsChecked == true)&& (cbTH.IsChecked == true)&& (cbFR.IsChecked == true)&& (cbSA.IsChecked == true)&& (cbSU.IsChecked == true))
                    cbAllFeatures.IsChecked = true;
            if((cbMO.IsChecked == false) && (cbTU.IsChecked == false) && (cbWE.IsChecked == false)&& (cbTH.IsChecked == false)&& (cbFR.IsChecked == false)&& (cbSA.IsChecked == false)&& (cbSU.IsChecked == false))
                    cbAllFeatures.IsChecked = false;
        }
        
		#region RINGTONE

		public string GetRingTone()
		{
			var ringtone = WH.RINGTONE.RINGTONE_0;
			var selectedRingtone = RingtoneListView.SelectedValue.ToString();
			if (selectedRingtone!="") 
				ringtone = UtilEnum<WH.RINGTONE>.ParseEnum(selectedRingtone);
			return ringtone.ToString();
		}
		
		private void RingtoneListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.AlarmRingtone = GetRingTone();
			NotifyPropertyChanged("AlarmRingtone");
		}
        
		#endregion		
        
        
        
	}
}