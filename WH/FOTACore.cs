/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 04/15/2016
 * Time: 20:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of FOTACore.
	/// </summary>
	public class FOTACore : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
	        	
		        System.Diagnostics.Debug.WriteLine("OnPropertyChanged -> " + propertyName);
			}
		}

		#region properties
		string hexAppFilepath = "";
		public string HexAppFilepath {
			get { return hexAppFilepath; }
			set { hexAppFilepath = value; SetHexFilename(03); OnPropertyChanged("HexAppFilepath"); }
		}

		string hexAppFilename = "";
		public string HexAppFilename {
			get { return hexAppFilename; }
			set { hexAppFilename = value; OnPropertyChanged("HexAppFilename"); }
		}
		
		string hexFotaFilepath= "";
		public string HexFotaFilepath {
			get { return hexFotaFilepath; }
			set { hexFotaFilepath = value; SetHexFilename(04); OnPropertyChanged("HexFotaFilepath"); }
		}

		string hexFotaFilename= "";
		public string HexFotaFilename {
			get { return hexFotaFilename; }
			set { hexFotaFilename = value; OnPropertyChanged("HexFotaFilename"); }
		}

		public string FOTAVendorSpecific {
			get;
			set;
		}

		#endregion
		
		public FOTACore()
		{ 
			var appSettings = WPF_S39_Commander.Properties.Settings.Default;
			this.FOTAVendorSpecific = appSettings["setting_FOTAVendorSpecific"] as string;
		}

		
		private void SetHexFilename(int mode)
		{
			if (mode==03)
				if (!string.IsNullOrEmpty(HexAppFilepath))
			   		this.HexAppFilename = System.IO.Path.GetFileName(this.HexAppFilepath);

			if (mode==04)
				if (!string.IsNullOrEmpty(HexFotaFilepath))
			   		this.HexFotaFilename = System.IO.Path.GetFileName(this.HexFotaFilepath);
		}
		
		
		public string GetFotaUpdateCommand()
		{
			if (!string.IsNullOrEmpty(this.HexFotaFilepath)) {
				return string.Format("{0},{1},{2}", FOTAVendorSpecific, HexAppFilename, HexFotaFilename);
			}
			else
			{
				return string.Format("{0},{1}", FOTAVendorSpecific, HexAppFilename);
			}
		}
	}
}
