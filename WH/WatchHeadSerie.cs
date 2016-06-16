/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 15.02.2016
 * Time: 09:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace WPF_S39_Commander.WH
{
	public class WatchHeadList : System.Collections.Generic.List<WatchHeadSerie>
	{
		/// <summary>
		/// FindWatch
		/// </summary>
		/// <param name="macAddress"></param>
		/// <returns></returns>
		public int FindWatch (string macAddress)
		{
			var ret = this.FindIndex(w => (w.MACaddress == macAddress));
			if (0<=ret && ret< this.Count) 
				return ret;
			else
			{
				Console.Beep(4000,500);
				Console.WriteLine("Warning : Unknown watch has been connected !");
				System.Diagnostics.Debug.WriteLine("Warning : Unknown watch has been connected !");
				return -1;
			}
		}
	}
	
	/// <summary>
	/// Description of WatchHeadSerie.
	/// </summary>
	public class WatchHeadSerie
	{
		  #region	Properties	
		  
		  public string WatchName {get; set;}
		  public string MACaddress {get; set;}
		  public string MACName {get; set;}
		  
		  #endregion

		  public WatchHeadSerie() {}
		  public WatchHeadSerie(string settings)
		  {
				this.ParseSettings(settings);
		  }
		  
		  public void ParseSettings(string settings)
		  {
		  	var rawdata =
		  	settings.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
		  	
		  	if (rawdata.Length==3) {
		  		for (int i = 0; i < rawdata.Length; i++) 
		  		{
		  			string mess = rawdata[i].Trim();
		  			if (i==0) {
		  				this.WatchName = mess;
		  			}	
		  			else if (i==1) {
		  				this.MACaddress = mess;
		  			}	
		  			else if (i==2) {
		  				this.MACName = mess;
		  			}	
		  		}
		  	  }
		  	}
		  	
		public override string ToString()
		{
			return string.Format("[WH Name={0}, MAC={1}, Short={2}]", WatchName, MACaddress, MACName);
		}
	}
}
