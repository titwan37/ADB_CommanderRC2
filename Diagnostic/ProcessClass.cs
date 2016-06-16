/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/16/2016
 * Time: 10:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;

namespace WPF_S39_Commander.Diagnostic
{
	/// <summary>
	/// Description of ProcessClass.
	/// </summary>
	public class ProcessClass
	{
		public ProcessClass()
		{
		}
		
		
		public static string GetRealADBPath(string input)
		{
			var Fpath = Path.GetDirectoryName(input);
			var filen = Path.GetFileName(input);
			var realpath = Environment.ExpandEnvironmentVariables(Fpath);
			return Path.Combine(realpath, filen);
		}
		
		public static Process Start(bool input, bool error, bool output, bool events, string workingDirectory)
		{
			Process process;
			var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe");
			{
				//processInfo.FileName = "cmd.exe";
				processInfo.CreateNoWindow = true;
				processInfo.UseShellExecute = false;
				processInfo.WorkingDirectory = workingDirectory;
				
				processInfo.RedirectStandardInput = input;
				processInfo.RedirectStandardError = error;
				processInfo.RedirectStandardOutput = output;
			}

			process = System.Diagnostics.Process.Start(processInfo);
			// enable Events
			process.EnableRaisingEvents = events;

			// register Events
			//process.OutputDataReceived += OnLogCatProcessOutputDataReceived;
			//process.ErrorDataReceived += OnLogCatProcessErrorDataReceived;
			//process.Exited += new EventHandler(Process_Exited);
			//process.BeginOutputReadLine();
			//process.BeginErrorReadLine();
			
			//process.WaitForExit(); //realtime output starts on console
			return process;
		}
		
		public static string Stop(Process process, bool exit, int delay, bool ReadToEnd)
		{
			string outputString  = "";

			//ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(WaitForProc), process);
			//System.Threading.Tasks.Task<string> task = System.Threading.Tasks.Task.Factory.StartNew<string>(() => WaitForProc(process, delay));
			//task.Result as string;
			outputString = WaitForProc(process, exit, delay, ReadToEnd);
			return outputString;
		}
		
		public static string WaitForProc(object obj, bool exit, int delay, bool ReadToEnd) 
		{
			string outputString  = "";
			var process = (Process)obj;
			if (process!=null)
				if (process.HasExited==false)
				{
					// line added to stop process from hanging on ReadToEnd
					process.StandardInput.Flush();
					process.StandardInput.Close();

					if (ReadToEnd)
						if (process.StartInfo.RedirectStandardOutput)
					{
								outputString  = process.StandardOutput.ReadToEnd(); // synchronous operation
											    process.StandardOutput.Close();
					}
					
					if (exit) {
						bool exited = process.WaitForExit(delay);
						// Do the file deletion here
						
						if (!exited) 
						{
							process.Close();
							process.Kill();
							process = null;
						}
					}
					
				}
			return outputString;
		}
		
		public static void Process_Exited(object sender, EventArgs e)
		{
			//var proc = (Process)sender;
			Debug.WriteLine("Process "+ e.ToString() +" has exited.");
		}
		
		/*
		public static Task<bool> WaitForExitAsync(Process process, TimeSpan timeout)
		{
		    ManualResetEvent processWaitObject = new ManualResetEvent(false);
		    processWaitObject.SafeWaitHandle = new SafeWaitHandle(process.Handle, false);
		
		    TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		
		    RegisteredWaitHandle registeredProcessWaitHandle = null;
		    registeredProcessWaitHandle = ThreadPool.RegisterWaitForSingleObject(
		        processWaitObject,
		        delegate(object state, bool timedOut)
		        {
		            if (!timedOut)
		            {
		                registeredProcessWaitHandle.Unregister(null);
		            }
		
		            processWaitObject.Dispose();
		            tcs.SetResult(!timedOut);
		        },
		        null // state 
				,
		        timeout,
		        true // executeOnlyOnce 
			);
		
		    return tcs.Task;
		}
		*/
		
	}
}
