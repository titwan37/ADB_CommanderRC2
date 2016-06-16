/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 03/23/2016
 * Time: 17:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of FanHeat.
	/// </summary>
	public class FanHeat: INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		
		private int heat = 0;
		public int Heat {
			get { return heat; }
			set { heat = value; OnPropertyChanged("Heat"); }
		}
		public FanHeat(){}
		public FanHeat(string clapCount, string olaCount, string power, string perf)
		{
			this.CalculateHeat(clapCount, olaCount, power, perf);
		}
		public void CalculateHeat(string clapCount, string olaCount, string power, string perf)
		{
			this.Heat = CalculateFanHeat(clapCount, olaCount, power, perf);
		}
		public static int CalculateFanHeat(string clapCount, string olaCount, string clapPower, string perf)
		{
			int iclapCount  = int.Parse(clapCount);
			int iolaCount  = int.Parse(olaCount);
			int iclapPower  = int.Parse(clapPower);
			int iperf  = int.Parse(perf);
			
			const float fola = 20/1f;
			const float fclap = 1/10f;
			const float fpower = 1/750;
			double fperf = Math.Pow(2,14) * 1/25000f;
			
			var Hpower	= iclapPower * iclapCount * fpower;
			var Hclap 	= iclapCount * fclap;
			var Hperf	= iperf * fperf;
			var Hola 	= iolaCount * fola;
			
			return System.Convert.ToInt32(Hpower +  Hperf + Hclap + Hola);
		}
	}
}
