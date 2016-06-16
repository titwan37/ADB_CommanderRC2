/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 16.02.2016
 * Time: 17:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for ClockControl.xaml
	/// </summary>
	public partial class ClockControl : UserControl
	{
		public DateTime dt {get;set;}
		private static readonly DispatcherTimer _timer = new DispatcherTimer();
		
		/// <summary>
		/// ClockControl
		/// </summary>
		public ClockControl()
		{
			InitializeComponent();
			this.dt = DateTime.Now;
			_timer.Interval = new TimeSpan(0,0,0,0,1);
			_timer.Tick += (s, e) => OnPropertyChanged(this, null);
		}
		
		private void OnPropertyChanged(object sender, EventArgs e)
		{	
			var dtt = dt.AddMilliseconds(1);
            this.dt = dtt;
			//this.christianityCalendar.Content;
		}
	}

	/// <summary>
	/// ExtensionMethods
	/// </summary>
	public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate() { };
		/// <summary>
		/// Refresh ExtensionMethods
		/// </summary>
		/// <param name="uiElement"></param>
        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}