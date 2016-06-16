/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 03/03/2016
 * Time: 16:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;
using System.IO;

namespace WPF_S39_Commander.Diagnostic
{
	
	public interface IConsoleAutomator
	{
		event EventHandler<ConsoleInputReadEventArgs> StandardInputRead;
		StreamWriter StandardInput { get; }
	}
	
	/// <summary>
	/// Description of ConsoleAutomator.
	/// </summary>
	public class ConsoleAutomator : ConsoleAutomatorBase, IConsoleAutomator
	{
	    public ConsoleAutomator(StreamWriter standardInput, StreamReader standardOutput)
	    {
	        this.StandardInput = standardInput;
	        this.StandardOutput = standardOutput;
	    }
	
	    public void StartAutomate()
	    {
	        this.stopAutomation = false;
	        this.BeginReadAsync();
	    }
	
	    public void StopAutomation()
	    {
	        this.OnAutomationStopped();
	    }
	}

	
	public class ConsoleInputReadEventArgs : EventArgs
	{
		public string Input { get; private set; }

		public ConsoleInputReadEventArgs(string input)
		{
		    this.Input = input;
		}
	}
	

	
	public abstract class ConsoleAutomatorBase : IConsoleAutomator
	{
		protected readonly StringBuilder inputAccumulator = new StringBuilder();
		
		protected readonly byte[] buffer = new byte[256];
		
		protected volatile bool stopAutomation;
		
		public StreamWriter StandardInput { get; protected set; }
		
		protected StreamReader StandardOutput { get; set; }
		
		protected StreamReader StandardError { get; set; }
		
		public event EventHandler<ConsoleInputReadEventArgs> StandardInputRead;
		
		protected void BeginReadAsync()
		{
		    if (!this.stopAutomation) 
			    if (this.StandardOutput!=null) 
			    {
			        this.StandardOutput.BaseStream.BeginRead(this.buffer, 0, this.buffer.Length, this.ReadHappened, null);
			    }
		}
		
		protected virtual void OnAutomationStopped()
		{
		    this.stopAutomation = true;
		    this.StandardOutput.DiscardBufferedData();
		}
		
		private void ReadHappened(IAsyncResult asyncResult)
		{
			var bytesRead = this.StandardOutput.BaseStream.EndRead(asyncResult);
		    if (bytesRead == 0) {
		        this.OnAutomationStopped();
		        return;
		    }
		
		    var input = this.StandardOutput.CurrentEncoding.GetString(this.buffer, 0, bytesRead);
		    this.inputAccumulator.Append(input);
		
		    // raise Event something on the Input
		    if (bytesRead < this.buffer.Length) {
		        this.OnInputRead(this.inputAccumulator.ToString());
		    }
		
		    this.BeginReadAsync();
		}
		
		private void OnInputRead(string input)
		{
		    var handler = this.StandardInputRead;
		    if (handler == null) {
		        return;
		    }
		
		    handler(this, new ConsoleInputReadEventArgs(input));
		    this.inputAccumulator.Clear();
		}
  }

	
	
	
}
