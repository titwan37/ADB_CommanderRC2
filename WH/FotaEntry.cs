/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 15.04.2016
 * Time: 21:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of FotaEntry.
	/// </summary>
	public class FotaEntry
	{
		public enum FOTAMODENUM { Zero, Push, StartUpdate }
		
		bool isParsed = false;
		
		public bool IsParsed {
			get { return isParsed; }
			set { isParsed = value; }
		}
		private FOTAMODENUM fotamode = FOTAMODENUM.Zero;
		public FOTAMODENUM FOTAMode {
			get { return fotamode; }
			set { fotamode = value; }
		}
		public string Message { get; private set; }
			
		
		public FotaEntry()
		{
			
		}
		public FotaEntry(string message)
		{
			this.Parse(message);
		}
		
		void Parse(string message)
		{
			this.Message = message;
			if(!string.IsNullOrEmpty(message))
				if (message.IndexOf("push")>0) {
					this.IsParsed = true;
					this.FOTAMode = FOTAMODENUM.Push;
				}
				else if (message.IndexOf("UPDATE")>0) {
					this.IsParsed = true;
					this.FOTAMode = FOTAMODENUM.StartUpdate;
				}
		}
	}
}
