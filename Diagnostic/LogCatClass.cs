/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/22/2016
 * Time: 11:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace WPF_S39_Commander.Diagnostic
{
	/// <summary>
	/// Description of LogCat.
	/// </summary>
	public class LogCat
	{
		public LogCat(){}
		
		
		const int SIZE = 4096;
		static byte[] buffer = new byte[SIZE];
        static string textString = string.Empty;
        //static AutoResetEvent autoEvent = new AutoResetEvent(false);
		
		public static void ReadAsync(IAsyncResult asynResult)
        {
                var reader = asynResult.AsyncState as StreamReader;
                int bytesAvail = reader.BaseStream.Read(buffer, 0, SIZE);
                textString += Encoding.UTF8.GetString(buffer, 0, bytesAvail);
        }
		
		public static void StopLogCat(Process LogCatPro)
		{
			if (LogCatPro==null) return;
		
			var PTrId = (UInt32)LogCatPro.Id;

			if (LogCatPro!=null)
				if (LogCatPro.StartInfo.RedirectStandardInput) 
					if (LogCatPro.StandardInput!=null) 
						if (LogCatPro.StandardInput.BaseStream!=null) 
						{
							LogCatPro.StandardInput.WriteLine("{0} {1}", "adb.exe logcat ", "-c"); 
							//LogCatPro.StandardInput.WriteLine("^C");
							LogCatPro.StandardInput.Flush();
							LogCatPro.StandardInput.Close();
						}
			
			if (LogCatPro!=null)
			if (LogCatPro.HasExited==false)
				LogCatPro.WaitForExit(1500);
			
			if (LogCatPro!=null)
				if(LogCatPro.HasExited==false)
				{
					KillAllProcessesSpawnedBy(PTrId);
					//LogCatPro.Dispose();
					//KillADBTasks();
					if (LogCatPro!=null)
						if(LogCatPro.HasExited==false)
							LogCatPro.Kill();
				}
			//Console.Beep(800, 1000);
		}
		
		public static void KillADBTasks()
		{
			try
			{
				var allproc = Process.GetProcesses();
				var adbproc = new System.Collections.Generic.List<Process>();
				foreach (var pro in allproc) {
					if (pro.ProcessName.Contains("adb")){
						adbproc.Add(pro);
						Console.WriteLine(pro.ProcessName + pro.Id);
					}
				}

				Process[] proc = adbproc.ToArray();
				//Process[] proc = Process.GetProcessesByName("adb");
				
				for (int i = proc.Length - 1; i >= 0; i--)
				{
					proc[i].Kill();
					//Console.Beep(1600, 500);
				}
				
			}catch{}
		}
		
		public static void KillAllProcessesSpawnedBy(UInt32 parentProcessId)
	    {
	        //logger.Debug("Finding processes spawned by process with Id [" + parentProcessId + "]");
	
	        // NOTE: Process Ids are reused!
	        System.Management.
	        ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(
	            "SELECT * " +
	            "FROM Win32_Process " +
	            "WHERE ParentProcessId=" + parentProcessId);
	        System.Management.ManagementObjectCollection collection = searcher.Get();
	        if (collection.Count > 0)
	        {
	            //logger.Debug("Killing [" + collection.Count + "] processes spawned by process with Id [" + parentProcessId + "]");
	            foreach (var item in collection)
	            {
	                var childProcessId = (UInt32)item["ProcessId"];
	                if ((int)childProcessId != Process.GetCurrentProcess().Id)
	                {
	                    KillAllProcessesSpawnedBy(childProcessId);
	                    System.Threading.Thread.Sleep(400);
	                    Process childProcess = Process.GetProcessById((int)childProcessId);
	                    //logger.Debug("Killing child process [" + childProcess.ProcessName + "] with Id [" + childProcessId + "]");
	                    childProcess.Kill();
	                }
	            }
	        }
	    }
		
	}
}
