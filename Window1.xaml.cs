/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 27.01.2016
 * Time: 13:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using IronPython.Modules;
using Microsoft.Win32.SafeHandles;
//using SharedElements;
using WPF_S39_Commander.WH;
using WPF_S39_Commander.Diagnostic;

namespace WPF_S39_Commander
{
	//public class CommandListClass: List<string>{}
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		#region Properties
		internal static string DateRadical { get {return ExecutionDate.ToString("yyyyMMddHHmm");} }
		internal static DateTime ExecutionDate ;
		internal static string ExecutionFolder;
		
  		private string ADBPath;
		private string ADBFilter;
  		private string InjectionPath;
  		private string StatisticsPath;
  		private string ExcelTemplatePath;

  		private double DelayTime = 60;
		private double PauseTime = 25;

		private bool AutoOpenCSV = false;
		private bool LineExtEnabled = false;
		
		private bool ConnectionCheck = false;
  		private bool LogCatAutoStart = false;
		
  		internal WatchHeadList WHList  {get; set; }
        internal WHInfo myWH { get; private set; }
        internal PythonConsole py ;
        internal StopwatchCountDown heartBeatCountDown;
        internal string heartBeatMessage;
        internal TimeSpan heartBeatBluetoothTimeout;
        internal Boolean? HeartBeatEnabled { get; set; }
        
        //public string MACRadical {get; set; }
        private string _MACRadical = "";
        public string MACRadical
		{
		    get { return _MACRadical; }
		    set { if (value != _MACRadical) { var old = _MACRadical; _MACRadical = value;
		            OnPropertyChanged(new DependencyPropertyChangedEventArgs(TextBox.TextProperty, old, value)); } }
		}
        
        private string _TCName = "NoName";
        public string TCName 	 {get {return _TCName;} set {if (value!=_TCName) {_TCName = value; TCNameBox.Text=value; }} }
        
        public string TCFilename {get; set; }
        
        public const string _profilePath = "%USERPROFILE%\\Desktop\\TestResults\\";
        
        public string _ProfileFolder = "";
        public string ProfileFolder
        { 
        	get { if (_ProfileFolder == "")
        			return _ProfileFolder = Environment.ExpandEnvironmentVariables(_profilePath);
        		else
        		{ 	return _ProfileFolder; }
        	}
        }
        
        public string SaveFolder {get {return Path.Combine(ProfileFolder,string.Format("{0}_{1}_{2}",DateRadical, MACRadical, TCName));} }
		#endregion
        
		public Window1()
		{
			ExecutionDate = DateTime.Now;
			ExecutionFolder = Environment.CurrentDirectory;

			AuditTrail.Filename = string.Concat(DateRadical, "_", AuditTrail.Filename);
			AuditTrail.SetFilepath(ExecutionFolder);

			this.myWH = new WH.WHInfo();
			
			InitializeComponent();
			
			this.HeartBeatEnabled = true; // TODO: sequence order impacts on autostart
			this.DataContext = this; // TODO: to be clean up !!!
			this.chkHeartBeat.DataContext = this;
		 
			//DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);

			this.ReadHistorySettings();
			
			this.LoadUIEventHandlers();

			this.myWH.PropertyChanged +=OnWHInfoPropertyChanged;
		}
		
		#region Windows settings
		private void LoadUIEventHandlers()
		{
			WHDClock.OnBtnGet_Click += WHDClock_OnBtnGet_Click;
			WHDClock.OnDateChanged += WHDClock_OnDateChanged;
			WHDClock.OnBtnSet_Click += WHDClock_OnBtnSet_Click;
			WHDClock.StartTimer();
			
			WHDAlarm.OnBtnGet_Click += WHDClock_OnBtnGet_Click;
			WHDAlarm.OnBtnSet_Click += WHDAlarm_OnBtnSet_Click;
			
			PCChrono.OnBtnGet_Click += PCChrono_OnBtnStart_Click;
			PCChrono.OnBtnSet_Click += PCChrono_OnBtnReset_Click;
			
			WHSettingBox.OnBtnGet_Click += WHSettingBox_OnBtnGet_Click;
			WHSettingBox.OnBtnSet_Click += WHSettingBox_OnBtnSet_Click;
			
			WatchUserSettings.OnBtnGet_Click += WHUserSettingBox_OnBtnGet_Click;
			WatchUserSettings.OnBtnSet_Click += WHUserSettingBox_OnBtnSet_Click;
			
			WatchGoalSettingsBox.OnBtnGet_Click += WatchGoalSettingsBox_OnBtnGet_Click;
			WatchGoalSettingsBox.OnBtnSet_Click += WatchGoalSettingsBox_OnBtnSet_Click;
			WatchGoalSettingsBox.OnBtnCal_Click += WatchGoalSettingsBox_OnBtnCal_Click;
			
			InjectionSettingBox.PropertyChanged += InjectionSettingox_PropertyChanged;

			FOTAUserControlBox.OnFOTAPushCommand+= OnFOTAPushCommand_Click;
			FOTAUserControlBox.OnFOTAUpdateCommand +=OnFOTAUploadCommand_Click;
			FOTAUserControlBox.OnFOTALogSaveCommand +=OnFOTALogSaveCommand_Click;

			myWH.ConnectState.ConnectionChanged += OnWatchConnectionChanged;
		}


		private void Window1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			heartBeatCountDown.Dispose();
			LogCat.StopLogCat(this.LogcatProcess);
		}
		
		private void Window1_Closed(object sender, EventArgs e)
		{
			LogCat.KillADBTasks();
		}
		
		private void window1_Loaded(object sender, RoutedEventArgs e)
		{
			LoadUserSettingsGUI();
			
			if (LogCatAutoStart)  
				LogCatStart_Click(sender, e);
		}

		private void LoadUserSettingsGUI()
        {
			var currentExeFolderName = GetCurrentExeFolderName();
			this.window1.Title += " - " + currentExeFolderName;
			
        	//Properties.Settings.Default
			var appSettings = WPF_S39_Commander.Properties.Settings.Default;
			this.LogCatAutoStart = System.Convert.ToBoolean(appSettings["setting_LogCatAutoStart"] as string);
			
			//setting_ConnectionCheck
			this.ConnectionCheck = System.Convert.ToBoolean(appSettings["setting_ConnectionCheck"] as string);
			
			this.ADBPath = appSettings["setting_ADBpath"] as string;
			this.ADBPath = ProcessClass.GetRealADBPath(ADBPath);
			this.ADBFilter = appSettings["setting_ADBFilter"] as string;
			
			this.StartCommand.Text = appSettings["setting_ADBstart"] as string;
			this.StopCommand.Text = appSettings["setting_ADBstop"] as string;
			this.FirstCommand.Text = appSettings["setting_ADBcommand"] as string;
			
			//this.PythonPath = appSettings["setting_PythonPath"] as string;
			this.InjectionPath = appSettings["setting_InjectionPath"] as string;
			this.StatisticsPath = appSettings["setting_StatsPath"] as string;
			
			this.ExcelTemplatePath = appSettings["setting_ExcelTemplate"] as string;
			
			var watchColl = new System.Collections.Specialized.StringCollection();
			watchColl = appSettings["Setting_WatchList"] as System.Collections.Specialized.StringCollection;

			this.DelayTime = System.Convert.ToInt32(appSettings["setting_DelayTime"] as string);
			this.InjectionSettingBox.DelayBefore = DelayTime;
			this.PauseTime = System.Convert.ToInt32(appSettings["setting_PauseDuration"] as string);
			this.InjectionSettingBox.PauseTime = PauseTime;
			
			this.AutoOpenCSV = System.Convert.ToBoolean(appSettings["setting_AutoOpenCSV"] as string);
			this.LineExtEnabled = System.Convert.ToBoolean(appSettings["setting_LineExtEnabled"] as string);
			
			this.heartBeatMessage = appSettings["setting_HeartBeatMessage"] as string;
			this.heartBeatBluetoothTimeout = (TimeSpan)(appSettings["setting_BluetoothTimeout"]);

			Init_HeartBeat();
			
			WHList = new WatchHeadList();
			foreach (var wh in watchColl) { WHList.Add(new WatchHeadSerie(wh)); }
			this.CbxWatchHeads.ItemsSource = WHList;
			this.CbxWatchHeads.SelectedIndex=0;
			
			UpdateUIWatchHeadSettings();
			
			//Init_PythonConsole();
			Task.Factory.StartNew(()=>{	Init_PythonConsole();});
		}

		private void CbxWatchHeads_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.Dispatcher.BeginInvoke(
				DispatcherPriority.Render, new Action(delegate 
				{
				      	UpdateUIWatchHeadSettings();
				}));
		}
		
		private void ReadHistorySettings()
		{
			var MyCommands = Properties.ConfigurationReader.ReadCfgFile("Commands.cfg");
			this.CbxCommands.ItemsSource = MyCommands;
			this.CbxCommands.SelectedIndex=0;

			var MyDefaultSequences = Properties.ConfigurationReader.ReadCfgFile("DefaultSequence.cfg");
			this.SequenceTextBox.Text = String.Join("\r\n", MyDefaultSequences) + "\r\n";
			
			var MyHistory = Properties.ConfigurationReader.ReadCfgFile(AuditTrail.Filepath);
			this.HistoryBox.AddTextEntries(MyHistory);
			//this.ScrollToEnd(HistoryBox);
		}
		
		private void Init_HeartBeat()
		{
			heartBeatCountDown = new StopwatchCountDown(this.HeartBeatEnabled==true? true: false);
			heartBeatCountDown.TimeSpanGoal = heartBeatBluetoothTimeout!=TimeSpan.Zero ? heartBeatBluetoothTimeout : new TimeSpan(0,1,0); // theory 4 min
			heartBeatCountDown.OnCountDownComplete += countDown_OnCountDownComplete;
			heartBeatCountDown.OnCountDownTick+=	countDown_OnCountDownTick;
			this.LblHeartBeat_total.Content = heartBeatCountDown.TimeSpanGoal.ToString();
		}
		
		private void Init_PythonConsole()
		{
			py = new PythonConsole();
			py.OnDataRead += OnDataRead;
			py.DataReceived += new DataReceivedEventHandler(OnPythonConsoleDataReceived);
			py.InputDataReceived += OnPythonConsoleInputDataReceived;
			//Task.Factory.StartNew(()=>{py.Start("0x10");});		
		}
	
		private string GetCurrentExeFolderName()
		{
			//Application.Current
			var cf = Environment.CurrentDirectory;
			cf = (new DirectoryInfo(cf)).Name;
			return cf;
		}
		#endregion
		
		#region SendCommand
		private void RemoteAppStartCommand()
		{
			string command1 = StartCommand.Text;
			var outputString = SendWHCommand(command1, "");
		}
		private string SendWHCommand(string command, string argument)
		{
			return SendWHCommand(command, argument, 2000, true, true);
		}
		private string SendWHCommand(string command, string argument, int delay, bool ReadToEnd, bool exit)
		{
			if(HeartBeatEnabled==true&&LogMessageType.IsTimeoutRelevant(argument)) heartBeatCountDown.Start(); // reset countDown
			
			Process process = ProcessClass.Start(true, true, true, true, "");
			process.StandardInput.WriteLine("{0} {1}", command, argument);
			
			// UI does get updated from here
			AppendHistoryBox(AuditTrail.Log(command + " " + argument, myWH.Name.WatchName, myWH.Connection.MacAddress));
			//this.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => { this.HistoryBox.Text += AuditTrail.Log(command + " " + argument, WHName.Text, MACaddress.Text); this.ScrollToEnd(HistoryBox);}));
			
			var cmdOut = "";
			cmdOut = ProcessClass.Stop(process, exit, delay, ReadToEnd);
			this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { this.MSPromptBox.Text += cmdOut;this.ScrollToEnd(MSPromptBox);}));
			
			return  cmdOut;
		}
		
		private void Send_Command_Click(object sender, RoutedEventArgs e)
		{
			if(ConnectionCheck) 
				CheckConnectionState();
			
			ExecutionDate = DateTime.Now;
			string commmand = FirstCommand.Text;
			string arguments = CbxCommands.Text;
			
			var outputString = SendWHCommand(commmand, arguments);
		}
		
		private bool MultiCommandSending = false;
		private string ButtonSave_Label ="";
		private System.Windows.Media.Brush ButtonSave_Color;
		
		private void ToggleBtn_SendMultiCommand()
		{
			this.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => 
			{
				if (MultiCommandSending) 
				{
					this.Btn_SendMultiCommand.PbValue = 0;
					this.Btn_SendMultiCommand.PbMaximum = SequenceArgumentCount = 0;
					this.Btn_SendMultiCommand.Text1 = ButtonSave_Label;
					this.Btn_SendMultiCommand.Text2 = "";
					this.Btn_SendMultiCommand.Background1 = ButtonSave_Color;
					Debug.WriteLine("Toggle Progress Stop");
				}
				else
				{
					this.Btn_SendMultiCommand.Background1 = new LinearGradientBrush(Colors.Yellow, Colors.Red, 0.2);
					this.Btn_SendMultiCommand.PbMaximum = SequenceArgumentCount;
					this.Btn_SendMultiCommand.PbValue = 0;
					ButtonSave_Label = this.Btn_SendMultiCommand.Text1;
					ButtonSave_Color = this.Btn_SendMultiCommand.Foreground;
					this.Btn_SendMultiCommand.Text1 = "Sending ...";
					this.Btn_SendMultiCommand.Text2 = " Start ";
					Debug.WriteLine("Toggle Progress Start");
				}
			}));
		}
		
		private void CheckConnectionState()
		{
			//if (myWH.ConnectState.STATE==CONEXSTATE.DISCONNECTED)
			switch (myWH.ConnectState.STATE) 
			{
				case CONEXSTATE.DISCONNECTED:
					{
						MessageBox.Show("You are currently disconnected !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
						break;
					}
				case CONEXSTATE.UNKNOWN:
					{
						MessageBox.Show("Your state is unknown ?", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
						break;
					}
				default:
					{
						break;
					}
			}
		}
		
		private void SendMultiCommand_Click(object sender, RoutedEventArgs e)
		{
			if (ConnectionCheck) 
				CheckConnectionState();
			
			if (MultiCommandSending)
			{
				ToggleBtn_SendMultiCommand();
				this.MultiCommandStops();
				this.MultiCommandSending = false;
			}
			else
			{
				ToggleBtn_SendMultiCommand();
				this.MultiCommandSending = true;
				this.MultiCommandStarts();
			}
		}

		private void MultiCommandStarts()
		{
			ExecutionDate = DateTime.Now;
			var input = SequenceTextBox.Text;
		
			string commmand = FirstCommand.Text;
			
			var arguments = new List<string>(input.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
			var outputString = SendWHMultiCommand(commmand, arguments);
		}
		private void MultiCommandStops()
		{
			if (SendingTask!=null)
				if (tokenSource!=null)
				{
					tokenSource.Cancel();
					Debug.WriteLine("\nTask cancellation requested.");
					// Optional: Observe the change in the Status property on the task. 
					// It is not necessary to wait on tasks that have canceled. However, 
					// if you do wait, you must enclose the call in a try-catch block to 
					// catch the TaskCanceledExceptions that are thrown. If you do  
					// not wait, no exception is thrown if the token that was passed to the  
					// StartNew method is the same token that requested the cancellation
					
					if (SendingTask.IsCanceled)
						SendingTask.Dispose();
					
					// Notify a waiting thread that an event has occurred
				    if (awaitReplyOnRequestEvent != null)
				        awaitReplyOnRequestEvent.Set();

				    // If 1 sec later the task is still running, kill it cruelly
				    if (runningTaskThread != null)
				    {
				        try
				        {
				            runningTaskThread.Join(TimeSpan.FromSeconds(1));
				        }
				        catch (Exception)
				        {
				            runningTaskThread.Abort();
				        }
				    }
				}	
		}
		
		//var tasks = new ConcurrentBag<Task>();
		private int SequenceArgumentCount = 0;
		protected Task SendingTask = null;
		protected Thread runningTaskThread= null;
		protected CancellationTokenSource tokenSource = null;
		private AutoResetEvent awaitReplyOnRequestEvent = new AutoResetEvent(false);
		private string SendWHMultiCommand(string command, List<string> arguments) //, 
		{
			var msCommand = "";
			myWH.SetLogList(SaveFolder, AutoOpenCSV, LineExtEnabled);
			
			this.SequenceArgumentCount = arguments.Count;
			this.Btn_SendMultiCommand.PbMaximum = SequenceArgumentCount;
			Debug.WriteLine("Start sending, number of arguments = " + this.Btn_SendMultiCommand.PbMaximum);
			
			tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;	
			SendingTask = Task.Factory.StartNew(new Action(delegate
			   {	//Capture the thread
                	runningTaskThread = Thread.CurrentThread;
                	int argumentIndex = 0;
                   	foreach (var x in arguments) 
                   	{
			      		if (token.IsCancellationRequested) 
			      		{
		                    // another thread decided to cancel
		                    Debug.WriteLine("Task is cancelled");
		                    // Exit the loop
		                    break;
		                }
			      		else
			      		{	
			      			if (myWH.ConnectState.STATE == CONEXSTATE.DISCONNECTED)
			      			{
			      				var message = "Task is stopped due to the disconnected State of the watch.";
			      				AppendHistoryBox(message);
			                    Debug.WriteLine(message);
			                    // Exit the loop
			                    break;
			      			}
			      			
			      			this.Dispatcher.Invoke(new Action(() =>
							{	
								argumentIndex++;
						     	this.Btn_SendMultiCommand.PbValue = argumentIndex;
						      	Debug.WriteLine(argumentIndex + " " + x);
						     }), DispatcherPriority.Normal);
			      			
			      			msCommand += HandleInstructions(command, x);
							//this.HistoryBox.Text += AuditTrail_Log(arguments, WHName.Text, MACaddress.Text);
			      		}
			        } // for each
                   	
                   	// Toggle finish
                   	if(!token.IsCancellationRequested)
                   	{	
                   		ToggleBtn_SendMultiCommand();
                   		this.MultiCommandSending = false;	
                   	}
                   	else
                   	{	
                   		this.MultiCommandSending = true;
                   		ToggleBtn_SendMultiCommand();
                   	}
				}), token);
			return  msCommand;
		}
		
		const string pauseInstruction = "PAUSE";
		const string nextMinInstruction = "NEXTMIN";
	
		protected string HandleInstructions(string command, string x)
		  { 
			var msCommand = "";
			int delayMS = 20;

			if (x.Contains(pauseInstruction))
		  	{
				// UI does get updated from here
				AppendHistoryBox(x);
		  		DoEvents();
		  		var time  = System.Convert.ToInt32(PauseTime);
		  		System.Threading.Thread.Sleep(time * 1000);
		  	}
		  	else if (x.Contains("SHOT")) 
		  	{
				// UI does get updated from here
				AppendHistoryBox(x);
				ScreenShotCapture();
		    }
		  	else if (x.Contains("PREVHOUR")) 
		  	{
		  		this.Dispatcher.BeginInvoke(new Action(()=> {
		  		                                       	SetNextHour(-1);
		  		                                       	Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { })); // do events
		  		                                       	var clockControlTime = WHDClock.GetTimeFromDisplay();
		  		                                       	if (this.myWH.Time.SetClockTime(clockControlTime))
														{
															SendSettingTime();
														}
		  		                                       }));
		    }		  	
		  	else if (x.Contains("NEXTMIN")) 
		  	{
		  		var t = x.Substring(nextMinInstruction.Length);
		  		var time = System.Convert.ToInt32(t);
		  		this.Dispatcher.BeginInvoke(new Action(()=> {
		  		                                       	SetNextMinutes(time);
		  		                                       	Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { })); // do events
		  		                                       	var clockControlTime = WHDClock.GetTimeFromDisplay();
		  		                                       	if (this.myWH.Time.SetClockTime(clockControlTime))
														{
															SendSettingTime();
														}
		  		                                       }));
		  	}
		  	else if (x.Contains("NEXTHOUR")) 
		  	{
		  		this.Dispatcher.BeginInvoke(new Action(()=> {
		  		                                       	SetNextHour(1);
		  		                                       	Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { })); // do events
		  		                                       	var clockControlTime = WHDClock.GetTimeFromDisplay();
		  		                                       	if (this.myWH.Time.SetClockTime(clockControlTime))
														{
															SendSettingTime();
														}
		  		                                       }));
		    }
		  	else if (x.Contains("PREVDAY"))
		  	{
		  		this.Dispatcher.BeginInvoke(new Action(()=> {
		  		                                       	SetNextDay(-1);
		  		                                       	Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { })); // do events
		  		                                       	var clockControlTime = WHDClock.GetTimeFromDisplay();
		  		                                       	if (this.myWH.Time.SetClockTime(clockControlTime))
														{
															SendSettingTime();
														}
		  		                                       }));
		    }
		  	else if (x.Contains("NEXTDAY"))
		  	{
		  		this.Dispatcher.BeginInvoke(new Action(()=> {
		  		                                       	SetNextDay(1);
		  		                                       	Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { })); // do events
		  		                                       	var clockControlTime = WHDClock.GetTimeFromDisplay();
		  		                                       	if (this.myWH.Time.SetClockTime(clockControlTime))
														{
															SendSettingTime();
														}
		  		                                       }));
		    }
		  	
		  	else if (x.Contains("DUMP")) 
		  	{
		  		 GetALLlogsPull();
		    }
		  	else if (x.Contains("PACK")) 
		  	{
		  		 PackTestData();
		    }
		  	else if (x.Contains("FOLDER")) 
		  	{
		  		 OpenSaveFolder();
		    }
		  	else	
		  	{
		  		if(HeartBeatEnabled==true && LogMessageType.IsTimeoutRelevant(x)) heartBeatCountDown.Start();   // reset countdown timer

				Process cmd = ProcessClass.Start(true, true, true, true, "");
		  	    //cmd.StandardInput.WriteLine("{0} {1}", command, x);
		  	    cmd.StandardInput.WriteLine(string.Format("{0} {1}", command, x));
				
		  	    // UI does get updated from here
				AppendHistoryBox(command + " " + x);
		  	    //cmd.StandardInput.Flush();
				var cmdOut = ProcessClass.Stop(cmd, true, 2000, true);
				msCommand += cmdOut;
				// UI does get updated from here
				AppendMSPrompt(cmdOut);
		  	}
		  	System.Threading.Thread.Sleep(delayMS);
		  	
		  	return msCommand;
		  }
		
		private void AppendHistoryBox(string message)
		{
			if (HistoryBox.Dispatcher.Thread != Thread.CurrentThread)
				HistoryBox.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate { HistoryBox.AddInline(AuditTrail.Log(message, myWH.Name.WatchName, myWH.Connection.MacAddress), Colors.DarkBlue);} ));
			else
				HistoryBox.AddInline(AuditTrail.Log(message, myWH.Name.WatchName, myWH.Connection.MacAddress), Colors.DarkBlue);
		}
		private void AppendMSPrompt(string message)
		{
			MSPromptBox.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate { this.MSPromptBox.Text += message; this.ScrollToEnd(MSPromptBox);}));
		}
		
		private void ScreenShotCapture()
		{
			var sav = SaveFolder;
			UtilClass.CheckCreateFolder(sav);
			var shotname = string.Concat(DateTime.Now.ToString("yyyyMMdd_HHmmss"),
			                                "_SnapShot_" , 
			                                TCName,
			                                ".png");
			var shotPath = Path.Combine(sav, shotname);
			const string commandSCR1 = "adb.exe shell screencap -p /sdcard/sc.png";
			string command2 = string.Concat("adb.exe pull /sdcard/sc.png ",	shotPath);

			var t1 = SendWHCommand(commandSCR1, "");
			System.Threading.Thread.Sleep(1000);
			var t2 = SendWHCommand(command2, "");
			
			Console.Beep(1200, 500);
		}
		#endregion		

		#region AuditTrail
		
		/// <summary>
		/// GetTodaysLogsPull
		/// </summary>
		private void GetTodaysLogsPull()
		{
			var pushLogFolder = SaveFolder + @"\swatch_logs";
			var pullLogFolder = @"/swatch_logs/logs/";
			var logFilename = string.Format("log_{0}*.log", DateTime.Now.ToString("yyyyMMdd"));
			UtilClass.CheckCreateFolder(SaveFolder);
			UtilClass.CheckCreateFolder(pushLogFolder);
			//The trick is to add /. to the name of the folder you want to copy
			var commandListLog1 = "adb shell ls -Ral /sdcard" + pullLogFolder + logFilename;
			var commandPullLog1 = "adb shell ls -Ral /sdcard" + pullLogFolder + logFilename;

			var t1 = SendWHCommand(commandListLog1,"");
			
			// Pull multiple files from android device, using a regular expression:

			//Create a new file, name it pullFiles.sh, and add this code in it:
			// #Get all of a directory(sdcard), and filter them
			// adb shell ls "/sdcard/swatch_logs/logs"  | find "log_20160212"
			// for file in 'adb shell ls /sdcard/swatch_logs/logs | find log_20160212'
			// do
			// 	file=`echo -e $file | tr -d "\r\n"`; # osx fix! ghhrrr :@ :(
			//  # pull the command
			//  adb pull /mnt/sdcard/$file /your/directory/$file;
			// done
			// Give it permissions: chmod +x pullFiles.sh
			// And run it: ./pullFiles.sh

			
			
			
			Console.Beep(1500, 500);
		}
		/// <summary>
		/// GetALLlogsPull
		/// </summary>
		private void GetALLlogsPull()
		{
			var autoFilterDumpLogs = true;
			var tsk = Task.Factory.StartNew(new Action(async delegate{
	
					var pushLogFolder = SaveFolder + @"\swatch_logs";
					const string pullLogFolder = @"/swatch_logs/logs";

					UtilClass.CheckCreateFolder(SaveFolder);
					UtilClass.CheckCreateFolder(pushLogFolder);
					
					//The trick is to add /. to the name of the folder you want to copy
					const string pullPath = "/sdcard" + pullLogFolder + "/. ";
					var commandPullLog1 = "adb pull "+ pullPath  + pushLogFolder;
					
					var t1 = SendWHCommand(commandPullLog1, "", 10000, false, false);
					
					if (autoFilterDumpLogs)
						UtilClass.DeleteOldFiles(pushLogFolder, ExecutionDate);
				
					
					await Task.Delay(2000);
					LaunchPythonStatisticsScript();
					
					Console.Beep(1500, 500);
			      }));
			
		}
		
		/// <summary>
		/// OpenSaveFolder
		/// </summary>
		private void OpenSaveFolder()
		{
			var sav = SaveFolder;
			if (Directory.Exists(sav)) {
				Process.Start("explorer.exe", sav);
			}
		}
		private void SaveLogCatLive()
		{
			try {
				var input = ADBBox.GetText();
				string log_filename = string.Format("{0}_{1}_ADBLogCatLive.log", DateTime.Now.ToString("yyyyMMddHHmm"), MACRadical);
				string log_filepath = Path.Combine(SaveFolder, log_filename);
	      		File.WriteAllText(log_filepath, input);
	      		
			} catch (Exception ex) {
				Debug.WriteLine("Error SaveLogCatLive");
				Debug.WriteLine(ex.ToString());
			}
			
		}
		
		private static void OnLogCatProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (sender!=null) {
				var proc = (Process)sender;
			}
			
			if (e!=null)
				if (e.Data!=null)
					Debug.WriteLine(e.Data.ToString());
		}
		private static void OnLogCatProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e!=null)
				if (e.Data!=null)
					Debug.WriteLine(e.Data.ToString());
		}
		
		private void PackTestData()
		{
			UtilClass.CheckCreateFolder(SaveFolder);
			
			// 1-Audit Trail
			SaveAuditTrailFile();
			
			// 2- LogCatLive
			SaveLogCatLive();
			
			// 3- Test Script
			SaveTestScenarioPacking();

			// Publish All
			if (myWH!=null) myWH.PublishAllLogList(SaveFolder);
			
			// Save FOTA LogLive
			if (myWH!=null) FOTAUserControlBox.SaveFOTALogLive(SaveFolder, myWH.Connection.MacRadical);

			// 4 - Python Script
			SaveScriptFile(this.InjectionPath);
			
			// 5 - Statistics Script
			SaveScriptFile(this.StatisticsPath);
			
			Console.Beep(1800, 500);
		}

		private void SaveTestScenarioPacking()
		{
			try {
				if(TCName=="") TCName=	"NoTestName";
				
				string tc_filename = string.Format("{0}_{1}_{2}.tst", DateTime.Now.ToString("yyyyMMddHHmmss"), MACRadical, TCName);
				string tc_filepath = Path.Combine(SaveFolder, tc_filename);
				
				SaveTestScenario(tc_filepath);
				
			} catch (Exception ex) {
				Debug.WriteLine("Error writing TestScenario");
				Debug.WriteLine(ex.ToString());
			}
			
		}
		
		private void SaveTestScenario(string filepath)
		{
			try 
			{
				var input = this.SequenceTextBox.Text;
	      		File.WriteAllText(filepath, input);
			} 
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error SaveTestScenario");
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}
		}
		
		private void SaveAuditTrailFile()
		{
			try 
			{
				string filename1 = Path.GetFileName(AuditTrail.Filepath);
				var filename2 = string.Format("{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), filename1);
				// 
				if (File.Exists(AuditTrail.Filepath))
					File.Copy(AuditTrail.Filepath, Path.Combine(SaveFolder, filename2), true);
				
			} catch (Exception ex) {
				
				Debug.WriteLine("Error writing Audit Trail");
				Debug.WriteLine(ex.ToString());
			}
			
		}

		private string SaveScriptFile(string path)
		{
			try 
			{
				if (!string.IsNullOrEmpty(path))
				{
					string filename1 = Path.GetFileName(path);
					var filename2 = string.Format("{0}_{1}", DateTime.Now.ToString("yyyyMMdd"), filename1);
					var newPath2 = Path.Combine(SaveFolder, filename2);
					if (File.Exists(path))
						File.Copy(path, newPath2, true);
						
					return newPath2;
				}
			} catch (Exception ex) {
				Debug.WriteLine("Error writing Script File " + path);
				Debug.WriteLine(ex.ToString());
			}
			
			return "";
		}

		private void LaunchPythonScript_Click(object sender, RoutedEventArgs e)
		{
			LaunchPythonStatisticsScript();
		}

		private void LaunchPythonStatisticsScript()
		{
			try 
			{
				// py.exe BTConnectionStabilityMetrics.py -d 20160330 > BTConnectionStabilityMetrics-30.log
				
				const string command = "py.exe";
				var savFolder = this.SaveFolder;
				
				var newStatPath = SaveScriptFile(this.StatisticsPath);
				var reportsDate = DateTime.Now.ToString("yyyyMMdd");
				var argument = string.Format(" {0} -d {1} > {1}_BTConnectionStabilityMetrics.csv", newStatPath, reportsDate);
				
				//System.Diagnostics.Process.Start("py.exe", argumnentLine);
				Process process = ProcessClass.Start(true, true, true, true, savFolder);
				process.StandardInput.WriteLine("{0} {1}", command, argument);
				
				// UI does get updated from here
				AppendHistoryBox(AuditTrail.Log(command + " " + argument, myWH.Name.WatchName, myWH.Connection.MacAddress));
				//this.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => { this.HistoryBox.Text += AuditTrail.Log(command + " " + argument, WHName.Text, MACaddress.Text); this.ScrollToEnd(HistoryBox);}));
				
				var batchfile = string.Format("{0}\\{1}_{2}", savFolder, reportsDate,"BTConnectionStabilityMetrics.bat");
				CreateABatchFile(batchfile, command, argument);

				var cmdOut = "";
				cmdOut = ProcessClass.Stop(process, true, 1000, true);
				this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { this.MSPromptBox.Text += cmdOut;this.ScrollToEnd(MSPromptBox);}));
			
			} catch (Exception ex) {
				Debug.WriteLine("Error executing Script File ");
				Debug.WriteLine(ex.ToString());
			}
			

		}

		private void CreateABatchFile(string batchFile, string command, string argument)
		{
			using (var sw = File.CreateText(batchFile)) {
				sw.WriteLine("{0} {1}", command, argument);
				sw.Flush();
				sw.Close();
			}  
		}

		#endregion

		private void RemoteAppStart_Click(object sender, RoutedEventArgs e)
		{
			RemoteAppStartCommand();
		}
		private void RemoteAppShut_Click(object sender, RoutedEventArgs e)
		{
			string command1 = StopCommand.Text;
			var outputString = SendWHCommand(command1,"");
		}
		
		private void WHConnection_Click(object sender, RoutedEventArgs e)
		{
			var mac = myWH.GetMacAddress();
			string commmand = FirstCommand.Text;
			// CONNECT_DEVICE,0C:F3:EE:12:34:56,on/off
			string arguments = string.Format("CONNECT_DEVICE,{0},off",mac);
			var outputString = SendWHCommand(commmand,arguments);
		}
		private void WHConnectStatus_Click(object sender, RoutedEventArgs e)
		{
			var mac = myWH.GetMacAddress();
			string commmand = FirstCommand.Text;
			// GET_DEVICE_CONNECTION_STATUS,0C:F3:EE:12:34:56,10000
			string arguments = string.Format("GET_DEVICE_CONNECTION_STATUS,{0},10000",mac);
			var oldccValue = this.ConnectionCheck;
			this.ConnectionCheck = false;
			var outputString = SendWHCommand(commmand, arguments);
			this.ConnectionCheck = oldccValue;
		}
		private void WHDisconnect_Click(object sender, RoutedEventArgs e)
		{
			string commmand = FirstCommand.Text;
			// DISCONNECT
			string arguments = string.Format("DISCONNECT");
			var outputString = SendWHCommand(commmand,arguments);
		}
		
		private void Toggle_ChkHeartBeat()
		{
			var ischecked = chkHeartBeat.IsChecked;
			if (ischecked==true)
				HeartBeatEnabled = true;
			else
				HeartBeatEnabled = false;
		}
		
		private void ChkHeartBeat_Changed(object sender, RoutedEventArgs e)
		{
			Toggle_ChkHeartBeat();
			
			if(HeartBeatEnabled==false && heartBeatCountDown._running==true)
				heartBeatCountDown.Stop();
			else if (HeartBeatEnabled==true && heartBeatCountDown._running==false)
				heartBeatCountDown.Start();
			else if (HeartBeatEnabled==false && heartBeatCountDown._running==false)
			{}
			else if (HeartBeatEnabled==true && heartBeatCountDown._running==true)
			{}
		}

		private void OnWHInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			try {

				if (e!=null){
				var propName = e.PropertyName;
				Debug.WriteLine(propName);
				if (!string.IsNullOrEmpty(propName))
					if (propName.Contains("Energy"))
					{
						WatchGoalSettingsBox_OnBtnCal_Click(sender, null);
					}
				}
			} 
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				throw ex;
			}
		}	
		
		private void countDown_OnCountDownTick(object sender, EventArgs e)
		{
			var ct = ( StopwatchCountDown)sender;
			this.LblHeartBeat_now.Content = ct.TimeSpanElapsed.ToString(@"hh\:mm\:ss");
			this.LblHeartBeat_total.Content =  string.Format("{0:P2}.",ct.Percentage);
			this.BlETimeoutCountDown.Percentage = ct.Percentage;
		}
		private void countDown_OnCountDownComplete(object sender, EventArgs e)
		{
	    	//WHConnectStatus_Click(sender, null);
	    	//WHDClock_OnBtnGet_Click(sender,null);
	    	HeartBeatMessage();
		}
	    private void BlETimeoutCountDown_OnCountDownComplete(object sender, EventArgs e)
		{
	    	//WHConnectStatus_Click(sender, null);
		}
	    private void HeartBeatMessage()
	    {
	    	// SET GOAL 
	    	string commmand = FirstCommand.Text;
			string arguments = string.Format(this.heartBeatMessage);
			var outputString = SendWHCommand(commmand, arguments);
	    }
	    
	    private void InjectionSettingox_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Contains("Delay"))
				this.DelayTime = InjectionSettingBox.DelayBefore;
			else if (e.PropertyName.Contains("Pause"))
			{
				this.PauseTime  = InjectionSettingBox.PauseTime;
			}
		}
		
		private void WHSettingBox_OnBtnGet_Click(object sender, RoutedEventArgs e)
		{
			string commmand = FirstCommand.Text;
			string arguments = string.Format("GET_SETTINGS_WATCH");
			var outputString = SendWHCommand(commmand,arguments);
		}
		private void WHSettingBox_OnBtnSet_Click(object sender, RoutedEventArgs e)
		{
			var set1 = WHSettingBox.GetSettings1();
			var set2 = WHSettingBox.GetSettings2();
			var set3 = WHSettingBox.GetSettings3();
			
			var WHn = "";
			if (string.IsNullOrWhiteSpace(set2))
				WHn = myWH.Name.WatchName;
			else 
				WHn = set2;
			
			string commmand = FirstCommand.Text;
			string arguments = string.Format("SET_SETTINGS_WATCH,{0},{1},{2}", set1, WHn, set3);
			                                 
			var outputString = SendWHCommand(commmand,arguments);
		}
	
		private void WHUserSettingBox_OnBtnGet_Click(object sender, RoutedEventArgs e)
		{
			string commmand = FirstCommand.Text;
			string arguments = string.Format("GET_SETTINGS_USER");
			var outputString = SendWHCommand(commmand,arguments);
		}
		private void WHUserSettingBox_OnBtnSet_Click(object sender, RoutedEventArgs e)
		{
			// User Box
			var set1 = WatchUserSettings.GetSettings();
			string commmand = FirstCommand.Text;
			string arguments = string.Format("SET_SETTINGS_USER,{0}",set1);
			var outputString = SendWHCommand(commmand,arguments);
			
			// Goal Box
			
		}
		
		private void WatchUserSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName=="User")
				myWH.User = WatchUserSettings.User;
		}
		
		private void WatchGoalSettingsBox_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Contains("Goal"))
			{
				myWH.Goals.Goal = WatchGoalSettingsBox.Goal;
				myWH.Goals.GoalState = WatchGoalSettingsBox.GoalState=="on"?true:false;
			}
		}
		
//		WatchGoalSettingsBox_OnBtnGet_Click
//		WatchGoalSettingsBox_OnBtnSet_Click
		private void WatchGoalSettingsBox_OnBtnGet_Click(object sender, RoutedEventArgs e)
		{
			string commmand = FirstCommand.Text;
			string arguments = string.Format("GET_SETTINGS_GOALS");
			var outputString = SendWHCommand(commmand,arguments);
		}
		private void WatchGoalSettingsBox_OnBtnSet_Click(object sender, RoutedEventArgs e)
		{
			var set1 = WatchGoalSettingsBox.GetSettings();
			string commmand = FirstCommand.Text;
			string arguments = string.Format("SET_SETTINGS_GOALS,{0}",set1);
			var outputString = SendWHCommand(commmand,arguments);
		}		
		private void WatchGoalSettingsBox_OnBtnCal_Click(object sender, RoutedEventArgs e)
		{
				WatchGoalSettingsBox.CalculateKCal(myWH);
		}		
		//WHDClock_OnBtnGet_Click
		private void WHDClock_OnBtnGet_Click(object sender, RoutedEventArgs e)
		{
			string commmand = FirstCommand.Text;
			string arguments = string.Format("GET_SETTINGS_TIME");
			var outputString = SendWHCommand(commmand,arguments);
		}
		/// <summary>
		/// WHDClock_OnBtnSet_Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WHDClock_OnBtnSet_Click(object sender, RoutedEventArgs e)
		{
			var clockControlTime = WHDClock.GetTimeFromDisplay();
			var alarmControlTime = WHDAlarm.GetTimeFromDisplay();
			if (this.myWH.Time.SetClockTime(clockControlTime))
			{
				SendSettingTime();
			}
		}
		
		private void WHDAlarm_OnBtnSet_Click(object sender, RoutedEventArgs e)
		{
			var clockControlTime = WHDClock.GetTimeFromDisplay();
			var alarmControlTime = WHDAlarm.GetTimeFromDisplay();
			if (this.myWH.Time.SetAlarm(alarmControlTime))
			{
				SendSettingTime();
			}
		}
		
		private void SendSettingTime()
		{
			string commmand = FirstCommand.Text;
		
			// SET_SETTINGS_TIME
			string arguments = string.Format("SET_SETTINGS_TIME,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
			//12/24
			myWH.WatchSettings.HourFormat,
			//US/EU
			myWH.WatchSettings.DateTimeCulture,
			// on,on,on,off,on,on,off,off
			myWH.WatchSettings.AlarmRepetitionSettings,
            // alarm status
			myWH.WatchSettings.AlarmStatus,
			// alarm ringtone
            myWH.Time.iAlarmRingtone,
			// alarm beep
            myWH.WatchSettings.AlarmBeep,
            
            
            // date
            myWH.Time.weekday,
            myWH.Time.day,
            myWH.Time.month,
            myWH.Time.year,
            // time
            myWH.Time.hour,
            myWH.Time.minute,
            myWH.Time.second,
            // alarm
            myWH.Time.alarmHour,
            myWH.Time.alarmMinute,
            myWH.WatchSettings.BeatTime
            );
			
			var outputString = SendWHCommand(commmand,arguments);
		}
		
		private void SetNowTime_Click(object sender, EventArgs e)
		{
			var pctime = DateTime.Now;
			myWH.Time.SetDateTime(pctime);
			this.WHDClock.ChangeDateTime(pctime);
			this.WHDClock.SetTimeToDisplay();
			this.Update_WHDatePicker(pctime.Date);
		}
		private void SetNext5MTime_Click(object sender, RoutedEventArgs e)
		{
			SetNextMinutes(5);
		}
		private void SetNextMinutes(int shift)
		{
			var dt = WHDClock.GetTimeFromDisplay();
			dt = dt.AddMinutes(shift); 
			dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 00);
			
			if(shift>0)
				dt = dt.AddSeconds(-this.DelayTime);
			myWH.Time.SetDateTime(dt);
			
			this.WHDClock.ChangeDateTime(dt);
			this.WHDClock.SetTimeToDisplay();
			this.Update_WHDatePicker(dt.Date);
			this.UpdateWatchClockTimes();

//			DoEvents();
		}		
		private void SetNextHour(int shift)
		{
			var dt = WHDClock.GetTimeFromDisplay();
			dt = dt.AddHours(shift); 
			dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 00, 00);
			if(shift>0)
				dt = dt.AddSeconds(-this.DelayTime);
			myWH.Time.SetDateTime(dt);
			this.WHDClock.ChangeDateTime(dt);
			this.WHDClock.SetTimeToDisplay();
			this.Update_WHDatePicker(dt.Date);
			//DoEvents();
			this.UpdateWatchClockTimes();
		}
		private void SetPrevHourTime_Click(object sender, RoutedEventArgs e)
		{
			SetNextHour(-1);
		}
		private void SetNextHourTime_Click(object sender, RoutedEventArgs e)
		{
			SetNextHour(1);
		}
		private void SetNextDay(int shift)
		{
			var dt = WHDClock.GetTimeFromDisplay();
			dt = dt.AddDays(shift);
			dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
			myWH.Time.SetDateTime(dt);
			this.WHDClock.ChangeDateTime(dt);
			this.WHDClock.SetTimeToDisplay();
			this.Update_WHDatePicker(dt.Date);
			this.UpdateWatchClockTimes();
			DoEvents();
		}

		public static void DoEvents()
		{
			// DoEvents()
			//Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { })); 
		    DispatcherFrame frame = new DispatcherFrame();
		    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
		    Dispatcher.PushFrame(frame);
		}
		public static object ExitFrame(object f)
		{
		    ((DispatcherFrame)f).Continue = false;
		    return null;
		}
		private void SetPrevDayTime_Click(object sender, RoutedEventArgs e)
		{
			SetNextDay(-1);
		}		
		private void SetNextDayTime_Click(object sender, RoutedEventArgs e)
		{
			SetNextDay(1);
		}
		private void SetNextMonthTime_Click(object sender, RoutedEventArgs e)
		{
			var dt = WHDClock.GetTimeFromDisplay();
			dt = dt.AddMonths(1);
			dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
			myWH.Time.SetDateTime(dt);
			this.WHDClock.ChangeDateTime(dt);
			this.WHDClock.SetTimeToDisplay();
			this.Update_WHDatePicker(dt.Date);
			DoEvents();
		}
		private void SetEndOfMonth(int shift)
		{
			var dt = WHDClock.GetTimeFromDisplay();
			var dt2 = dt.AddMonths(shift);
			dt = new DateTime(dt2.Year, dt2.Month, 01, 00, 00, 00);
			if(shift>0)
				dt = dt.AddSeconds(-this.DelayTime);
			myWH.Time.SetDateTime(dt);
			this.WHDClock.ChangeDateTime(dt);
			this.WHDClock.SetTimeToDisplay();
			this.Update_WHDatePicker(dt.Date);
			this.UpdateWatchClockTimes();
			DoEvents();
		}
		private void SetEndOfMonthTime_Click(object sender, RoutedEventArgs e)
		{
			SetEndOfMonth(+1);
		}
		private void SetEndOfYear(int shift)
		{
			var dt = WHDClock.GetTimeFromDisplay();
			var dt2 = dt.AddYears(shift);
			dt = new DateTime(dt2.Year, 01, 01, 00, 00, 00);
			if(shift>0)
				dt = dt.AddSeconds(-this.DelayTime);
			myWH.Time.SetDateTime(dt);
			this.WHDClock.ChangeDateTime(dt);
			this.WHDClock.SetTimeToDisplay();
			this.Update_WHDatePicker(dt.Date);
			this.UpdateWatchClockTimes();
			DoEvents();
		}
		private void SetEndOfYearTime_Click(object sender, RoutedEventArgs e)
		{
			SetEndOfYear(+1);
		}		
		private void SetNextYearTime_Click(object sender, RoutedEventArgs e)
		{
			var dt = WHDClock.GetTimeFromDisplay();
			dt = dt.AddYears(1);
			dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
			myWH.Time.SetDateTime(dt);
			this.WHDClock.ChangeDateTime(dt);
			this.WHDClock.SetTimeToDisplay();
			this.Update_WHDatePicker(dt.Date);
			DoEvents();
		}
		private void SetMidnightTime_Click(object sender, RoutedEventArgs e)
		{
			var dt = WHDClock.GetTimeFromDisplay();
			//var dt = WHDClock.DT;
			dt = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
			dt = dt.AddSeconds(-DelayTime);
			myWH.Time.SetDateTime(dt);
			this.WHDClock.ChangeDateTime(dt);
			this.WHDClock.SetTimeToDisplay();
			this.Update_WHDatePicker(dt.Date);
			DoEvents();
		}
		
		private void WHDClock_OnDateChanged(object sender, EventArgs e)
		{
				if (sender.GetType()==typeof(WHClock)){
				var dt = ((WHClock)sender).DT; 	this.Update_WHDatePicker(dt);}
				else if (sender.GetType()==typeof(DateTime)) {
				var dt = (DateTime)sender;  this.Update_WHDatePicker(dt);}
				// TODO: to check or finish
		}
		
		private bool WHDatePickerSelectionSpy = true;
		private	void WHDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			bool changed = false;
			if (WHDatePickerSelectionSpy) 
			{
				if (e!=null) 
					if (e.AddedItems!=null && e.RemovedItems!=null)
						if (e.AddedItems.Count>0 && e.RemovedItems.Count>0) 
						{
							var newDate = (DateTime)e.AddedItems[0];
							var oldDate = (DateTime)e.RemovedItems[0];
							if(oldDate.Date!=newDate.Date)
									this.WHChangeDate(oldDate, newDate);
							
							changed = true;
						} 
				
				if(changed==false){
					var oldDate = myWH.Time.GetDateTime();
					var newDate = WHDatePicker.SelectedDate.Value;
					if(oldDate.Date!=newDate.Date)
						this.WHChangeDate(oldDate, newDate);
				}
			}
		}
		
		private void WHChangeDate(DateTime olddate, DateTime newdate)
		{
			var dt = myWH.Time.ChangeDate(newdate);
			if (WHDClock.HasDateChanged(olddate,newdate))
			{
	     		this.WHDClock.ChangeDateTime(dt);
	     		this.WHDClock.SetTimeToDisplay();
	     		//this.Update_WHDatePicker(dt);
			}
		}
		
		private void Update_WHDatePicker(DateTime dt)
		{
			this.WHDatePickerSelectionSpy =false;
			
			if(dt!=null
			   &&  
			   dt> DateTime.MinValue)
     			this.WHDatePicker.SelectedDate = dt.Date;
			
			this.WHDatePickerSelectionSpy =true;
		}
		
		private void UpdateWatchClockTimes()
		{
     		var dt = myWH.Time.GetDateTime();
     		var bt = myWH.Time.BeatTime;
     		// Digital Clock
     		this.Update_WHDatePicker(dt);
     		
     		this.WHDClock.ChangeDateTime(dt);
     		this.WHDClock.SetTimeToDisplay();
     		//this.WHDClock.RestartTimer();
     		this.WHDAlarm.DT = myWH.Time.GetAlarmTime();
     		this.WHDAlarm.SetTimeToDisplay();
     		this.TimeSettingBox.BeatTime = bt.ToString();
     		this.AlarmSettingsBox.SetAlarmOptions(myWH);
     		// Analog CLock
     		this.WHClock.dt = dt;
		}
		
		private void TimeSettingBox_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Contains("Culture")) 
			{
				myWH.WatchSettings.DateTimeCulture = this.TimeSettingBox.DateTimeCulture;
			}
			else if (e.PropertyName.Contains("Hour")) 
			{
				myWH.WatchSettings.HourFormat = this.TimeSettingBox.HourFormat;
			}
			else if (e.PropertyName.Contains("Beep")) 
			{
				myWH.WatchSettings.AlarmBeep = this.TimeSettingBox.Beep;
			}
			else if (e.PropertyName.Contains("Beat")) 
			{
				myWH.WatchSettings.BeatTime = this.TimeSettingBox.BeatTime;
			}
		}
		private void AlarmSettingsBox_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Contains("Repetition")) 
			{
				myWH.WatchSettings.AlarmRepetitionSettings = this.AlarmSettingsBox.GetAlarmRepetitionSettings();
				myWH.Time.alarmRepeat = myWH.WatchSettings.AlarmRepetitionSettings;
				myWH.Time.alarmRepeat = myWH.WatchSettings.AlarmRepetitionSettings;
			}
			else if (e.PropertyName.Contains("Status")) 
			{
				myWH.WatchSettings.AlarmStatus = this.AlarmSettingsBox.AlarmStatus;
			}
			else if (e.PropertyName.Contains("Ringtone")) 
			{
				myWH.WatchSettings.AlarmRingtone = this.AlarmSettingsBox.GetRingTone();
				myWH.Time.sAlarmRingtone = myWH.WatchSettings.AlarmRingtone;
			}
		}
		
		//PCChrono_OnBtnStart_Click
		private void PCChrono_OnBtnStart_Click(object sender, RoutedEventArgs e)
		{
			PCChrono.DT = new DateTime(0);
			if (!PCChrono.Running) 	{ PCChrono.StartTimer();  }
			else					{ PCChrono.StopTimer(); }
			PCChrono_ButtonToggle();
		}
		private void PCChrono_ButtonToggle()
		{
			if (PCChrono.Running) 	{ PCChrono.BtnGet.Content = "Stop";  }	
			else					{ PCChrono.BtnGet.Content = "Start"; }
		}
		private void PCChrono_OnBtnReset_Click(object sender, RoutedEventArgs e)
		{
			PCChrono.DT = new DateTime(0);
			PCChrono.ResetTimer();
			PCChrono_ButtonToggle();
		}
		
		private void ScreenCapture_Click(object sender, RoutedEventArgs e)
		{
			ScreenShotCapture();	
		}
		
		private void GetTodayLogsPull_Click(object sender, RoutedEventArgs e)
		{
			GetTodaysLogsPull();
		}

		private void GetALLlogsPull_Click(object sender, RoutedEventArgs e)
		{		
			GetALLlogsPull();
		}

		private void PackTestData_Click(object sender, RoutedEventArgs e)
		{
			PackTestData();
		}
		
		private	void OpenSaveFolderBtn_Click(object sender, RoutedEventArgs e)
		{
			OpenSaveFolder();
		}

		#region ADB LogCat Viewer
        private Process LogcatProcess = null;
        private bool LogcatStatus = false;

		private void LogCatStart_Click(object sender, RoutedEventArgs e)
		{
			if (LogcatStatus==false) 
			{
					this.TabBottomControl.SelectedIndex = 2;
					LogcatProcess = StartLogCat();
			}
		}
		private void ADBServerRestart_Click(object sender, RoutedEventArgs e)
		{
			if (LogcatProcess!=null)
				if (LogcatProcess.StartInfo.RedirectStandardInput) 
					if (LogcatProcess.StandardInput!=null) {
						LogcatProcess.StandardInput.WriteLine(ADBPath, "adb kill-server");
						LogcatProcess.StandardInput.WriteLine(ADBPath, "adb start-server");
			}
		}
		private void LogCatStop_Click(object sender, RoutedEventArgs e)
		{
			if (LogcatStatus==true){
				LogCat.StopLogCat(LogcatProcess);
			}
		}       

		private Process StartLogCat()
		{
	      	// register Events
            //reader.BaseStream.BeginRead(buffer, 0, SIZE, new AsyncCallback(ReadAsync), reader);
			
            var startInfo = new ProcessStartInfo(ADBPath, ADBFilter)
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
        	LogcatStatus = true;
    
            //System.Diagnostics.Debug.WriteLine("LogCat Console Encoding format = ",  LogcatPro.StandardOutput.CurrentEncoding.ToString());

            return LogcatPro;
		}
		
		private void LogcatProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			bool flag = this.HandleLogcatMessage(e.Data);
			//if (flag==false) Console.Beep(2000,500);
		}
		
		private bool HandleLogcatMessage(string msg)
		{
			bool flag = false;
			
			// Parse LogCat message
         	var log =  new LogEnty(msg);
         	if (log.IsParsed)
         	{
         		myWH.HandleLogEntry(log);
         		
		        if (Dispatcher.Thread != Thread.CurrentThread)
		        	this.Dispatcher.BeginInvoke(DispatcherPriority.Input,new Action(delegate { UpdateUIfromLogInfo(myWH, log); }));
		        else
		        	UpdateUIfromLogInfo(myWH, log);
         		flag = true;
         	}
			
         	var ms2 = log.ToString();
	        if (Dispatcher.Thread != Thread.CurrentThread)
				this.Dispatcher.BeginInvoke(DispatcherPriority.Input,new Action(delegate { AppendTextAndScrollADBBox(msg, log, flag); }));
	        else
	        	AppendTextAndScrollADBBox(msg, log, flag);
	        
	        if (log.PatternType>0){
	        if (Dispatcher.Thread != Thread.CurrentThread)
				this.Dispatcher.BeginInvoke(DispatcherPriority.Background,new Action(delegate { AppendTextAndScrollInterpretBox(ms2, flag); }));
	        else
	        	AppendTextAndScrollInterpretBox(ms2, flag);}
			return flag;
		}
		
		private void AppendTextAndScrollADBBox(string msg, LogEnty log, bool IsParsed)
		{
			// update ADB Box
         	if (!string.IsNullOrWhiteSpace(msg))
         	{
         		if(IsParsed)
         			if  (log.PatternType==0)
         				this.ADBBox.AddInline(msg, Colors.LightSteelBlue);
         			else if (log.Status=="FAILED")
	         			this.ADBBox.AddInline( msg, Colors.Red);
         			else if (log.Status=="UNEXPECTED")
	         			this.ADBBox.AddInline( msg, Colors.OrangeRed);
         			else
         				if(log.IsFullyParsed)
	         				this.ADBBox.AddInline( msg, Colors.DarkGreen);
         				else
         					this.ADBBox.AddInline( msg, Colors.DarkMagenta);
         		else
         			this.ADBBox.AddInline(msg, Colors.Gray);
         	}
		}
		
		private void AppendTextAndScrollInterpretBox(string msg2, bool IsParsed)
		{
         	// update interpret box
            if (!string.IsNullOrWhiteSpace(msg2))
         	{
            	if (IsParsed)
            		this.InterpretBox.AddInline(msg2, Colors.DarkGreen);
            	else
            		this.InterpretBox.AddInline(msg2, Colors.DarkGray);
         		//this.ScrollToEnd(InterpretBox);
            }			
		}

		private void ScrollToEnd(TextBox tb)
		{
			//if (tb.IsVisible) tb.Focus();
			tb.ScrollToVerticalOffset( double.PositiveInfinity );
			EditingCommands.MoveToLineEnd.Execute( null, tb );
			tb.ScrollToEnd();
		}
		private void ScrollToEnd(UIElement tb)
		{
			//if (tb.IsVisible) tb.Focus();
			//EditingCommands.MoveToLineEnd.Execute( null, tb );
			//LogCatScroller.ScrollToEnd();
		}
		
		private void OnWatchConnectionChanged(object sender, WatchConnectState.ConnectionChangedEventArgs e)
		{
			if (e!=null) {
		        if (Dispatcher.Thread != Thread.CurrentThread)
					this.Dispatcher.BeginInvoke(DispatcherPriority.Input,new Action(delegate 
						{ 
						this.UpdateUIWatchInfo(myWH);
						}));
			}
		}

		private void UpdateUIWatchInfo(WHInfo wh)
		{
			this.MCName.Text = wh.Connection.MacRadical;

			this.WHSettingBox.ConexStatus = wh.Connection.Status;
     		this.WHSettingBox.MacAddress = wh.Connection.MacAddress;

     		this.ConnectionName.Content = wh.Name.ToDisplayString() + " " + wh.Version.ToDisplayString();
			this.ConnectionBox.Content = wh.Connection.ToDisplayString();
			this.ConnectionVersion.Content = wh.Version.ToLine();
		
			// auto select watch
     		if (wh.ConnectState.STATE != CONEXSTATE.UNKNOWN)
     		{
     			var found = WHList.FindWatch(wh.Connection.MacAddress);
     			if (found>-1)
     				CbxWatchHeads.SelectedIndex = found;
     			else
     			{
     				// Add new Watch
     				WHList.Add(new WatchHeadSerie()
     				           {WatchName=wh.Name.WatchName, 
     				           MACaddress= wh.Connection.MacAddress,
     				           MACName=wh.Connection.MacRadical});
     			}
     		}
     		
     		if (wh.ConnectState.STATE==CONEXSTATE.CONNECTED) {
     			ConnectionPanel.Background = new LinearGradientBrush(Colors.LightGreen, Colors.LimeGreen, 0.2);
     		}
     		else if (wh.ConnectState.STATE==CONEXSTATE.DISCONNECTED) {
     			ConnectionPanel.Background = new LinearGradientBrush(Colors.OrangeRed, Colors.Orange, 0.2);
     		}
     		else {
     			ConnectionPanel.Background = null;
     		}
		}
		private void UpdateUIWatchHeadSettings()
		{
			var myWHSerie
				= this.myWH.WHSelected
				= WHList[CbxWatchHeads.SelectedIndex];
			
			// UI
			this.WHName.Text = myWH.Name.WatchName = myWHSerie.WatchName;		
			this.MACaddress.Text = myWH.Connection.MacAddress = myWHSerie.MACaddress;
			
			myWH.Connection.SetMacRadical();
			this.MACRadical = myWHSerie.MACName = myWH.Connection.MacRadical;
			this.MCName.Text = myWHSerie.MACName;
		}		
		private void UpdateUIfromLogInfo(WHInfo wh, LogEnty log)
		{
			//this.Dispatcher.Invoke(new Action(delegate { 
				
			// Set Connected Watch
			if (log.LogComments is LogSettingsWatch)
			{
         		this.WHSettingBox.WatchName = wh.Name.WatchName;
         		this.WHSettingBox.WatchFlags = wh.Name.FlagToString();
         		this.UpdateUIWatchInfo(wh);
			}
			if (log.LogComments is LogSettingsGoals)
			{
				this.ConnectionBox.Content = wh.Connection.ToDisplayString();
         		this.WatchGoalSettingsBox.Goal = wh.Goals.Goal;
         		this.WatchGoalSettingsBox.GoalState = wh.Goals.GoalState?"on":"off";
			}
			if (log.LogComments is LogSettingsUser)
			{
				this.ConnectionBox.Content = wh.Connection.ToDisplayString();
         		//this.WatchUserLabel.Content = wh.User.ToString();
         		this.WatchUserSettings.User = wh.User;
			}
            if (log.LogComments is LogConnect) 
            {
         		this.UpdateUIWatchInfo(wh);
            }
			if (log.LogComments is LogConnectStatus) 
			{
         		this.UpdateUIWatchInfo(wh);
			}
			// Set Watch Time
			if (log.LogComments is LogSettingsTime)
			{
         		this.UpdateUIWatchInfo(wh);
         		this.UpdateWatchClockTimes();
            }
			//}));
		}

		#endregion
			
		#region OpenFileDialogs
		private void OpenTestScenario_Click(object sender, RoutedEventArgs e)
		{
			// Create OpenFileDialog 
    		Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
    		// Set filter for file extension and default file extension 
    		dlg.DefaultExt = ".tst";
    		dlg.Filter = "TEST Files (*.tst)|*.tst|All Files (*.*)|*.*"; 
    		dlg.InitialDirectory = Environment.CurrentDirectory;


		    // Display OpenFileDialog by calling ShowDialog method 
		    Nullable<bool> result = dlg.ShowDialog();

		    // Get the selected file name and display in a TextBox 
		    if (result == true)
		    {
		        // Open document 
		        this.TCFilename = dlg.FileName;
		        this.TCName = Path.GetFileNameWithoutExtension(TCFilename);

		        var sequenceList = Properties.ConfigurationReader.ReadCfgFile(TCFilename);
		        // Wrap and display to the text box
		        string sequence = String.Join("\r\n", sequenceList);
		        this.SequenceTextBox.Text = sequence;

		    }
		}
		
		private void SaveTestScenario_Click(object sender, RoutedEventArgs e)
		{
			// Create SaveFileDialog 
    		Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
    		// Set filter for file extension and default file extension 
    		dlg.Title = "Save an Test File";
    		dlg.DefaultExt = ".tst";
    		dlg.Filter = "TEST Files (*.tst)|*.tst"; 
    		dlg.InitialDirectory = Environment.CurrentDirectory;
    		
    		if (!string.IsNullOrEmpty(TCName))
    		{
    			dlg.FileName = TCName;
    		}
    		
			dlg.ShowDialog();
    		
    		if(dlg.FileName != "")
			   {
		        this.TCFilename = dlg.FileName;
		        this.TCName = Path.GetFileNameWithoutExtension(TCFilename);
    			  // Saves the file via a FileStream created by the OpenFile method.
			      // File type selected in the dialog box.
			      // NOTE that the FilterIndex property is one-based.
			      switch(dlg.FilterIndex)
			      {
			         case 1 :
			      		SaveTestScenario(TCFilename);
			         break;
			      }
			   }
		}

		private void OpenLogFile_Click(object sender, RoutedEventArgs e)
		{
			// Create OpenFileDialog 
    		Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
    		// Set filter for file extension and default file extension 
    		dlg.DefaultExt = ".log";
    		dlg.Filter = "LOG Files (*.log)|*.log|All Files (*.*)|*.*"; 
    		dlg.InitialDirectory = SaveFolder;

		    // Display OpenFileDialog by calling ShowDialog method 
		    Nullable<bool> result = dlg.ShowDialog();

		    // Get the selected file name and display in a TextBox 
		    if (result == true)
		    {
		    	this.CSVSAvePathBox.TextBoxText = WHInfo.ReGenerateCSVfiles(dlg.FileName);
		    }
		}
		private void OpenExcel()
		{
			//			var oExcelApp = (Microsoft.Office.Interop.Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
			//			    try{
			//			        var WB = oExcelApp.ActiveWorkbook;
			//			        var WS = (Worksheet)WB.ActiveSheet;
			//			        ((string)((Range)WS.Cells[1,1]).Value).Dump("Cell Value"); //cel A1 val
			//			        oExcelApp.Run("test_macro_name").Dump("macro");
			//			    }
			//			    finally{
			//			        if(oExcelApp != null)
			//			            System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcelApp);
			//			        oExcelApp = null; 
			//	}
			var argumnentLine = string.Format(" /t {0} /p {1}", this.ExcelTemplatePath, this.SaveFolder);
			System.Diagnostics.Process.Start("Excel.exe", argumnentLine);
			//System.Diagnostics.Process.Start(@"C:\loc\ADB_Commander\Book1.xltm");
		}
		private void OpenExcelReportBtn_Click(object sender, RoutedEventArgs e)
		{
			OpenExcel();
		}

		#endregion
	
		#region Python Console
		private void OnDataRead(string data)
		{
			if (data!=null)
			{
					var message = data.ToString();
					Debug.WriteLine(message);			
					InjectionBox.Text+= message + "\r\n";
			}
		}
		private void OnPythonConsoleDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e!=null)
				if (e.Data!=null)
				{
						var message = e.Data.ToString();
						Debug.WriteLine(message);			
						InjectionBox.Text+= message + "\r\n";
				}
			
		}
		private void OnPythonConsoleInputDataReceived(object sender, ConsoleInputReadEventArgs e)
		{
			if (e!=null)
				if (e.Input!=null)
				{
						var message = e.Input.ToString();
						Debug.WriteLine(message);
						this.Dispatcher.BeginInvoke(new Action(() => {
						InjectionBox.Text+= message + "\r\n";
                       }
                      ));
				}
		}
		#endregion
		
		
		#region FOTA
		private void OnFOTAPushCommand_Click(FOTACore fota)
		{
			this.PushFOTAFile(fota);
		}
		/// <summary>
		/// OnFOTAUploadCommand_Click
		/// </summary>
		/// <param name="fota"></param>
		private void OnFOTAUploadCommand_Click(FOTACore fota)
		{
			this.UploadFOTAFile(myWH, fota);
		}
		
		private void OnFOTALogSaveCommand_Click(FOTACore fota)
		{
			if (myWH!=null) 
				FOTAUserControlBox.SaveFOTALogLive(SaveFolder, myWH.Connection.MacRadical);
		}
		
		private void PushFOTAFile(FOTACore fota)
		{
			// adb push C:/loc/Deliveries/WH_APP_SW_RC05/FOTA/V001B-81-01-03-80-83-F5_FOTA.hex /sdcard/Documents/Swatch/
			
			if (!string.IsNullOrEmpty(fota.HexAppFilepath)) {
				string command2 = string.Format("adb.exe push {0} /sdcard/Documents/Swatch/",	fota.HexAppFilepath);
				SendWHCommand(command2, "");
				FOTAUserControlBox.Trace(command2);
			}
			if (!string.IsNullOrEmpty(fota.HexFotaFilepath)) {
				string command2 = string.Format("adb.exe push {0} /sdcard/Documents/Swatch/",	fota.HexFotaFilepath);
				SendWHCommand(command2, "");
				FOTAUserControlBox.Trace(command2);
			}			
			
			Console.Beep(1200, 500);			
			Console.Beep(2100, 500);			
		}		
		private void UploadFOTAFile(WHInfo wh, FOTACore fota)
		{
			if (ConnectionCheck) 
				CheckConnectionState();
			
			ExecutionDate = DateTime.Now;
			string mac = wh.GetMacAddress();
			
			//
			string command = FirstCommand.Text;
			string arguments = string.Format("FOTA_UPDATE,{0},{1}",mac, fota.GetFotaUpdateCommand());
			var outputString = SendWHCommand(command, arguments);
			
			string trace = string.Format("{0} {1}", command, arguments);
			FOTAUserControlBox.Trace(trace);
			
			Console.Beep(800, 500);			
		}
		#endregion
	}
}