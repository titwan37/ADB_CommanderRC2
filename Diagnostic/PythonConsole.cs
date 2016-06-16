/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 03.03.2016
 * Time: 13:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Diagnostics;
using WPF_S39_Commander.Diagnostic;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace WPF_S39_Commander.Diagnostic
{
	/// <summary>
	/// Description of PythonConsole.
	/// </summary>
	public class PythonConsole
	{
		public event EventHandler<ConsoleInputReadEventArgs> InputDataReceived;
		public event DataReceivedEventHandler DataReceived;
		protected virtual void OnDataReceived(object sender, DataReceivedEventArgs e)
		{
		    if (DataReceived != null) DataReceived(this, e);
		}
				
		public string _speed = "";
		public Process process;
		public ConsoleAutomator automator;
		//private Thread readingThread;
	    const string scriptFileInfoName = "pythonConsoleInit.py";
		
		public PythonConsole()
		{
//	        var ipy = Python.CreateRuntime();
//	        dynamic test = ipy.UseFile("test.py");
//	        test.Simple();
		}
		
		public void Start(string speed)
		{
			_speed = speed;
			
	        //OnDataRead += data => Console.WriteLine(data != null ? data : "Program finished");
	        //readingThread = new System.Threading.Thread(Read);
	        //var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe");
	        var processInfo = new System.Diagnostics.ProcessStartInfo("python.exe", string.Format("-i {0}", scriptFileInfoName));
			{
				//processInfo.CreateNoWindow = false;
				processInfo.UseShellExecute = false;
				
				processInfo.RedirectStandardInput = true;
				processInfo.RedirectStandardError = true;
				processInfo.RedirectStandardOutput = true;
			}
			process = new Process(){StartInfo= processInfo};
	        //readingThread.Start(process);

	        // enable Events
			//process.EnableRaisingEvents = false;
			process.ErrorDataReceived += new DataReceivedEventHandler(OnPythonOutputDataReceived);
			process.OutputDataReceived += new DataReceivedEventHandler(OnPythonOutputDataReceived);
			
			
			process.Start();
			automator= new Diagnostic.ConsoleAutomator(process.StandardInput, process.StandardOutput);
	        System.Threading.Tasks.Task.Factory.StartNew(()=>{Read(process);});
			// AutomatorStandardInputRead is your event handler
			automator.StandardInputRead += AutomatorStandardInputRead;
			automator.StartAutomate();

//			process.BeginErrorReadLine();
//			process.BeginOutputReadLine();
			
			if(processInfo.RedirectStandardInput)
			{
				var myStreamWriter = process.StandardInput;
	            myStreamWriter.WriteLine("aco = acolink.Acolink('--stdout out.txt run --ext9555 --speed "+ _speed +"')");
	            myStreamWriter.Flush();
	            
	            string output = process.StandardOutput.ReadToEnd();
    			process.WaitForExit();

			    Debug.WriteLine("Output:");
			    Debug.WriteLine(output);
	            
	            
	            //Microsoft.Scripting.Hosting.ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();
	            //myStreamWriter.Close();
				//process.StandardInput.WriteLine("python.exe");
				//process.StandardInput.WriteLine("python.exe");
				//process.StandardInput.WriteLine("import time");
				//process.StandardInput.WriteLine("import acolink");
				//process.StandardInput.WriteLine("aco = acolink.Acolink('--stdout out.txt run --ext9555 --speed "+ _speed +"')");
				//process.StandardInput.Flush();
			}
			
			// end of 
			//Stop();
		}
		
		public void Stop()
		{
			// do whatever you want while that process is running
			process.WaitForExit();
	        //readingThread.Join();			
			automator.StandardInputRead -= AutomatorStandardInputRead;
			process.Close();
		}
		
		
		private void AutomatorStandardInputRead(object sender, ConsoleInputReadEventArgs e)
		{
		    if (InputDataReceived != null) InputDataReceived(this, e);
		}
		
		private void OnPythonOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			OnDataReceived(sender, e);
		}
		
		
	    public delegate void DataRead(string data);
	    public event DataRead OnDataRead;
	
	    private void Read(object pythonProc)
	    {
	        Process process = pythonProc as Process;
	        char[] buffer = new char[Console.BufferWidth];
	        int read = 1;
	        while (read > 0)
	        {
//			   string s = process.StandardOutput.ReadLine();
//			   textOutput.Invoke( () => { textOutput.Text += s; } );
	            read = process.StandardOutput.Read(buffer, 0, buffer.Length);
	            string data = read > 0 ? new string(buffer, 0, read) : null;
	            if(data!=null)
	            	if (OnDataRead != null) 
	            		OnDataRead(data);
	        }
	    }
	}
	

	
	
	
}
