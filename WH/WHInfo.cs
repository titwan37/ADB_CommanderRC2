/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/16/2016
 * Time: 21:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

namespace WPF_S39_Commander.WH
{
	public enum RINGTONE {RINGTONE_0, RINGTONE_1, RINGTONE_2,RINGTONE_3}
	public enum CONEXSTATE { UNKNOWN, CONNECTED, DISCONNECTED}

	/// <summary>
	/// Description of WHInfo.
	/// </summary>
	public class WHInfo: INotifyPropertyChanged
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
		
		public WatchConnectState ConnectState = new WatchConnectState();
		
		public DateTime Today = DateTime.MinValue;
		public WatchHeadSerie WHSelected 		{ get; set; }
		public LogConnect Connection 			{ get; set; }
        public LogSettingsWatch Name 			{ get; set; }
        public LogVersionInformation Version 	{ get; set; }
		public LogSettingsTime Time 			{ get; set; }
		public LogSettingsUser User 			{ get; set; }
		public LogSettingsGoals Goals 			{ get; set; }
        public WatchSettingClass WatchSettings 	{ get; set; }
		public EnergyViewModel Energy 			{ get; set; }
		public FanHeat FanHeat 					{ get; set; }
        
        public LogList<LogPedometerDay> LogPedometerDayList 			{ get; set; }
		public LogList<LogPedometerTimeSlot> LogPedometerTimeSlotList 	{ get; set; }
		
		public LogList<LogActivityDay> LogActivityDayList 				{ get; set; }
		public LogList<LogActivityTimeSlot> LogActivityTimeSlotList 	{ get; set; }

		public LogList<LogFanGame> LogFanGameList 						{ get; set; }
		public LogList<LogFanTimeSlot> LogFanTimeSlotList 				{ get; set; }
        
		#endregion
		
		public WHInfo()
		{
			this.Name = new LogSettingsWatch("");
			this.Connection = new LogConnect("");
			this.Time = new LogSettingsTime("");
			this.User = new LogSettingsUser("");
			this.Goals = new LogSettingsGoals("");
			this.WatchSettings = new WatchSettingClass();
			this.Version = new LogVersionInformation();
			
			LogPedometerDayList = new LogList<LogPedometerDay>();
			LogPedometerTimeSlotList = new LogList<LogPedometerTimeSlot>();
			
			LogActivityDayList = new LogList<LogActivityDay>();
			LogActivityTimeSlotList = new LogList<LogActivityTimeSlot>();
			
			LogFanGameList = new LogList<LogFanGame>();
			LogFanTimeSlotList = new LogList<LogFanTimeSlot>();

			this.Energy = new EnergyViewModel();
			this.Energy.PropertyChanged +=EnergyPropertyChanged;
			
			this.FanHeat = new FanHeat();
		}
		
		public DateTime GetToday()
		{
			var date = DateTime.MinValue;
			if (Time!=null && Time.day>0) {
				date = new DateTime(Time.year, Time.month, Time.day);
			}	
			this.Today = date;
			return date;
		}
		
		public string GetMacAddress()
		{
			var mac = "";
			if (Connection!=null)
				if (Connection.Status=="SUCCESS")
				{
					mac = Connection.MacAddress;
				}
			
			if (WHSelected!=null) {
				mac = WHSelected.MACaddress;
			}
			
			return mac;
		}
		
		public void SetLogList(string SaveFolder, bool AutoOpen, bool LineExtEnabled)
		{
			LogPedometerDayList = new LogList<LogPedometerDay>(SaveFolder, AutoOpen);
			LogPedometerTimeSlotList = new LogList<LogPedometerTimeSlot>(SaveFolder, AutoOpen);
			
			LogActivityDayList = new LogList<LogActivityDay>(SaveFolder, AutoOpen);
			LogActivityTimeSlotList = new LogList<LogActivityTimeSlot>(SaveFolder, AutoOpen);
			                                           	
			LogFanGameList = new LogList<LogFanGame>(SaveFolder, AutoOpen);
			LogFanTimeSlotList = new LogList<LogFanTimeSlot>(SaveFolder, AutoOpen);
			LogFanTimeSlotList.LineExtEnabled = LineExtEnabled;
		}
	
		public void HandleLogEntry(LogEnty log)
		{
			// Handle Basic information
			log.HandleType(this);
			
			var com = log.LogComments;
			if (com==null)
				return;
			else if (com is LogSettingsWatch){
				var local = (LogSettingsWatch)com as LogSettingsWatch;
				if (local!=null)
					this.Name.Merge(local); // do not loose previous information
			}
			else if (com is LogSettingsGoals){
         		this.Goals = com as LogSettingsGoals;
			}
			else if (com is LogSettingsUser){
         		this.User = com as LogSettingsUser;
			}
			else if (com is LogVersionInformation){
         		this.Version = com as LogVersionInformation;
			}
            else if (com is LogConnect) {
				var local = (LogConnect)com as LogConnect;
				if (local!=null)
					this.Connection.Merge(local); // do not loose previous information
				
				this.ConnectState.STATE = Connection.STATE;
            }
			// Set Watch Time
			else if (com is LogSettingsTime){
         		this.Time = com as LogSettingsTime;
         		this.GetToday();
            }
			else if (com is LogConnectStatus) {
				var logstatus = (LogConnectStatus)com;
				this.Connection.Status = logstatus.PhonStatus;
			}
			else if (com is LogPedometerDay)
			{
				var pedo = (LogPedometerDay)com;
				// burn calories calculation
				pedo.HandleType(this);
			}
			else if (com is LogFanGame)
			{
				var fan = (LogFanGame)com;
				// fan heat calculation 
				fan.HandleType(this);
			}
			
			// compile the list & publish them
			this.LogPedometerDayList.HandleLogEntry(this, com);
			this.LogPedometerTimeSlotList.HandleLogEntry(this, com);
			this.LogActivityDayList.HandleLogEntry(this, com);
			this.LogActivityTimeSlotList.HandleLogEntry(this, com);
			this.LogFanGameList.HandleLogEntry(this, com);
			this.LogFanTimeSlotList.HandleLogEntry(this, com);
			
			OnPropertyChanged(log.GetType().Name);
		}

		private void EnergyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}
		
		public EnergyViewModel GetTodayEnergy()
		{
			if (this.LogPedometerDayList!=null)
			if (this.LogPedometerDayList.Count > 0)
			if (this.User!=null)
			if (this.User.height > 0)
			{
				if(Today==DateTime.MinValue) 
					this.Today = this.GetToday();
				if (Today>DateTime.MinValue)
				{
					// find the right displayed value
					var pedoDay = (LogPedometerDay)this.LogPedometerDayList.GetToday(Today) as LogPedometerDay; // date
					if (Energy!=null)
						this.Energy.Calculate(pedoDay, User.height, User.weight);
				}
			}
			return this.Energy;
		}
		
		public void PublishAllLogList(string folder)
		{
			try 
			{
				if (!LogPedometerDayList.IsPublished) 		LogPedometerDayList.PublishLogList(this);
				if (!LogPedometerTimeSlotList.IsPublished) 	LogPedometerTimeSlotList.PublishLogList(this);
				
				if (!LogActivityDayList.IsPublished) 		LogActivityDayList.PublishLogList(this);
				if (!LogActivityTimeSlotList.IsPublished) 	LogActivityTimeSlotList.PublishLogList(this);
	
				if (!LogFanGameList.IsPublished) 			LogFanGameList.PublishLogList(this);
				if (!LogFanTimeSlotList.IsPublished) 		LogFanTimeSlotList.PublishLogList(this);
				
			} 
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error PublishAllLogList");
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}
		}
		
		public static string ReGenerateCSVfiles(string file)
		{
	        var LogFilename = file;
	        if (System.IO.File.Exists(LogFilename)) 
	        {
				var localWH = new WHInfo();
		    	string saveFolder = System.IO.Path.GetDirectoryName(file);
				localWH.SetLogList(saveFolder, true, true);
				var lines =  System.IO.File.ReadAllLines(LogFilename);
				foreach (var element in lines) 
				{
					try 
					{
						var log =  new LogEnty(element);
						var ms2 = log.ToString();
						if (!string.IsNullOrWhiteSpace(ms2))
						{
							localWH.HandleLogEntry(log);	
						}
					} catch (Exception e) 
					{
						System.Diagnostics.Debug.WriteLine(e.ToString());
					}
				}
				lines = null;
				localWH.PublishAllLogList(saveFolder);
		    	return saveFolder;
	        }
	        return "";
		}
	}
}
