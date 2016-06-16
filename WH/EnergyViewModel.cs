/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 03/22/2016
 * Time: 19:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of EnergyViewModel.
	/// </summary>
	public class EnergyViewModel: INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		
//		private readonly Queue<T> queue = new Queue<T>();
//		public virtual void Enqueue(T item)
//	    {
//	        queue.Enqueue(item);
//	        OnChanged();
//	    }
//	    public int Count { get { return queue.Count; } }
//	
//	    public virtual T Dequeue()
//	    {
//	        T item = queue.Dequeue();
//	        OnChanged();
//	        return item;        
//	    }
		
		public EnergyViewModel()
		{
			this._dwalk = "0 km";
			this._drun = "0 km";
			this._kCAL = "0 kCal";
		}
		
		public EnergyViewModel Calculate(LogPedometerDay pedoDay, int height, int weight)
		{
			if(pedoDay!=null)
			{
				var d_wlk = (height * 0.445 /100f) *  pedoDay.walkStep_0 /1000f;
				var d_run = (height* 0.445 * 1.3 /100f) *  pedoDay.runStep_0 / 1000f;
				var k_Cal  = weight * ((d_wlk * 1.058) + (d_run * 1.03));
				
				this._dwalk = string.Format("{0:#0.000}", d_wlk);
				this._drun = string.Format("{0:#0.000}", d_run);
				this.kCAL = string.Format("{0:#0.00}", k_Cal); // trigger change
			}			
			return this;
		}

		string _dwalk = "";
		public string Dwalk {
			get { return _dwalk; }
			set { _dwalk = value; OnPropertyChanged("Dwalk"); }
		}
		string _drun = "";
		public string Drun {
			get { return _drun; }
			set { _drun = value; OnPropertyChanged("Drun"); }
		}
		private string _kCAL = "";
		public string kCAL {
			get { return _kCAL; }
			set { _kCAL = value; OnPropertyChanged("kCAL"); }
		}
	}
	
}
