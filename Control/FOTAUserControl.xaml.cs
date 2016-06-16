/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 15.04.2016
 * Time: 19:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Diagnostics;
using WPF_S39_Commander.WH;
using WPF_S39_Commander.Diagnostic;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for FOTAUserControl.xaml
	/// </summary>
	public partial class FOTAUserControl : UserControl
	{
		public delegate void FOTAEventHandler (FOTACore fota);
		public event FOTAEventHandler OnFOTAPushCommand;
		public event FOTAEventHandler OnFOTAUpdateCommand;
		public event FOTAEventHandler OnFOTALogSaveCommand;
		
		public FOTACore myFOTA { get; private set; }
	
		bool initialized = false;
		public FOTAUserControl()
		{
			myFOTA = new FOTACore();
			InitializeComponent();
			this.DataContext = myFOTA;
			this.Loaded +=UserControl_Loaded;	
		}


		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (initialized==false) 
			{
				var appSettings = WPF_S39_Commander.Properties.Settings.Default;
				this.ADBPath = appSettings["setting_ADBpath"] as string;
				this.ADBFOTAFilter = appSettings["setting_ADBFOTAFilter"] as string;
				this.FOTADefaultPath = appSettings["setting_FOTADefaultPath"] as string;
				//this.ADBFilter = "logcat -v time SwatchClient:V *:S -c";
				//this.ADBFilter = "logcat -v time BleProfileALPWFotaManager:V BleServiceALPWFota:V BaseFotaActivity:V FotaUpdateManager:V *:S -c";
				//this.ADBFOTAFilter = "logcat -c -v time SwatchClient:V BleProfileALPWFotaManager:V BleServiceALPWFota:V BaseFotaActivity:V FotaUpdateManager:V *:S";
				this.ADBPath = ProcessClass.GetRealADBPath(ADBPath);
				this.StartLogCat();
				initialized = true;
			}
		}
		
		private void ButtonFileOpen1_Click(object sender, RoutedEventArgs e)
		{
			this.OpenHexFile(myFOTA,03);
		}
		private void ButtonFileOpen2_Click(object sender, RoutedEventArgs e)
		{
			this.OpenHexFile(myFOTA,04);
		}
		
		private void OpenHexFile(FOTACore fota, int mode)
		{
			// Create OpenFileDialog 
    		Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
    		// Set filter for file extension and default file extension 
    		dlg.DefaultExt = ".hex";
    		dlg.Filter = "FOTA HEX Files (*.hex)|*.hex|All Files (*.*)|*.*"; 
    		dlg.InitialDirectory = FOTADefaultPath; // C:\loc\Deliveries\WH_APP_SW_RC06\FOTA


		    // Display OpenFileDialog by calling ShowDialog method 
		    Nullable<bool> result = dlg.ShowDialog();

		    // Get the selected file name and display in a TextBox 
		    if (result == true)
		    {
		    	
		    	if (mode==03) {
			        // Open document 
			        fota.HexAppFilepath = dlg.FileName;
		    	}
		    	else if (mode==04) {
			        // Open document 
			        fota.HexFotaFilepath = dlg.FileName;
		    	}
		    }
		}
		
		private void ButtonPushFile_Click(object sender, RoutedEventArgs e)
		{
			// Push FOTA File
			if (OnFOTAPushCommand!=null) 
				OnFOTAPushCommand(myFOTA);
		}

		private void ButtonUploadFile_Click(object sender, RoutedEventArgs e)
		{
			if (OnFOTAUpdateCommand!=null) 
				OnFOTAUpdateCommand(myFOTA);
		}
		
		private void ButtonSaveFOTAlog_Click(object sender, RoutedEventArgs e)
		{
			if (OnFOTALogSaveCommand!=null) 
				OnFOTALogSaveCommand(myFOTA);
		}

		public string ADBPath;

		public string FOTADefaultPath ;

		//bool LogcatStatus =false;
		public string ADBFOTAFilter;
		private Process StartLogCat()
		{
	      	// register Events
            //reader.BaseStream.BeginRead(buffer, 0, SIZE, new AsyncCallback(ReadAsync), reader);
			
            var startInfo = new ProcessStartInfo(ADBPath, ADBFOTAFilter)
			{
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false
			};
			
            var LogcatPro = new Process { StartInfo = startInfo };
			LogcatPro.EnableRaisingEvents = true;
			LogcatPro.OutputDataReceived += new DataReceivedEventHandler(this.LogcatProcess_OutputDataReceived);
			LogcatPro.ErrorDataReceived += new DataReceivedEventHandler(this.LogcatProcess_OutputDataReceived);
			LogcatPro.Start();
			LogcatPro.BeginOutputReadLine();
			LogcatPro.BeginErrorReadLine();
        	//LogcatStatus = true;
    

            return LogcatPro;
		}
		
		private void LogcatProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			bool flag = this.HandleLogcatMessage(e.Data);
			//if (flag==false) Console.Beep(2000,500);
		}

		public void Trace(string command)
		{
			HandleLogcatMessage(command);
		}

		bool HandleLogcatMessage(string data)
		{
	        if (FOTA_ADBBox.Dispatcher.Thread != Thread.CurrentThread)
				FOTA_ADBBox.Dispatcher.BeginInvoke(DispatcherPriority.Input,new Action(delegate { AppendTextAndScrollADB_FotaBox(FOTA_ADBBox, data); }));
	        else
	        	AppendTextAndScrollADB_FotaBox(FOTA_ADBBox, data);
			
	        return true;
		}
		
		private void AppendTextAndScrollADB_FotaBox(AdvancedSuperTextBlock sender, string msg)
		{
			// update ADB Box
         	if (!string.IsNullOrWhiteSpace(msg))
         	{
         		LogEnty log = null;
         		var fot = new FotaEntry(msg);
         		
         		if (fot.IsParsed == false)
         			log = new LogEnty(msg);

         		if (fot.IsParsed)
         		{
         			if (fot.FOTAMode==FotaEntry.FOTAMODENUM.Push) {
         				sender.AddInline(msg, Colors.DarkGreen);
         			}
         			else if (fot.FOTAMode==FotaEntry.FOTAMODENUM.StartUpdate) {
         				sender.AddInline(msg, Colors.LightSeaGreen);
         			}
         		}
         		else if(log!=null && log.IsParsed){
         			if (log.Status=="FAILED")
	         			sender.AddInline(msg, Colors.Red);
         			else if (log.Status=="UNEXPECTED")
	         			sender.AddInline(msg, Colors.OrangeRed);
         			else if (log.LogType=="W") // warning
	         			sender.AddInline(msg, Colors.DarkOrange);
         			else if (log.LogType=="E") // error
	         			sender.AddInline(msg, Colors.Coral);
         			else if  (log.PatternType==0)
	         			sender.AddInline(msg, Colors.LightSteelBlue);
         			else
         				if(log.IsFullyParsed)
	         				sender.AddInline(msg, Colors.DarkGreen);
         				else
         					sender.AddInline(msg, Colors.DarkMagenta);
         		}
         		else
					sender.AddInline(msg, Colors.Gray);

				//LogCatScroller.ScrollToEnd();
         	}
		}
		
		
		
		public void SaveFOTALogLive(string saveFolder, string MacRadical)
		{
	        if (FOTA_ADBBox.Dispatcher.Thread != Thread.CurrentThread)
				FOTA_ADBBox.Dispatcher.Invoke(DispatcherPriority.Input,new Action(delegate { SaveFOTALogLiveThread(saveFolder, MacRadical); }));
	        else
	        	SaveFOTALogLiveThread(saveFolder, MacRadical);
		}
		protected void SaveFOTALogLiveThread(string saveFolder, string MacRadical)
		{
			try 
			{
				var input = this.FOTA_ADBBox.GetText();
				string log_filename = string.Format("{0}_{1}_ADB_FOTALog.log", DateTime.Now.ToString("yyyyMMddHHmm"), MacRadical);
				string log_filepath = System.IO.Path.Combine(saveFolder, log_filename);
				if (!string.IsNullOrEmpty(input))
				{
					UtilClass.CheckCreateFolder(saveFolder);
	      			System.IO.File.WriteAllText(log_filepath, input);
				}
			} 
			catch (Exception ex) 
			{
				Debug.WriteLine("Error Save_ADB_FOTALog");
				Debug.WriteLine(ex.ToString());
			}
		}
	}
}