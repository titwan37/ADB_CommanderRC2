/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 08.03.2016
 * Time: 16:56
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
	/// Interaction logic for DynamicInjectionControl.xaml
	/// </summary>
	public partial class DynamicInjectionControl : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		
		public DynamicInjectionControl()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
		double delayBefore = 0;
		public double DelayBefore {
			get { return delayBefore; }
			set { delayBefore = value; OnPropertyChanged("DelayBefore");}
		}
		double pauseTime = 0;
		public double PauseTime {
			get { return pauseTime; }
			set { pauseTime = value; OnPropertyChanged("PauseTime");}
		}
	}
}