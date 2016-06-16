/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/23/2016
 * Time: 19:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;
using IronPython.Modules;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogList.
	/// </summary>
	public class LogList<T> : System.Collections.Generic.List<T> where T : ILogBody, IScribe
	{
		public LogList(){}
		public LogList(string savepath, bool autoOpen){ SaveFolder = savepath; AutoOpenCSV = autoOpen;}
		
		internal bool AutoOpenCSV { get; set; }
		internal string SaveFolder { get; set; }
		public bool IsPublished { get; set; }
		public bool LineExtEnabled = false;
		
		internal static string GetMyDateString (DateTime dt)
		{
			return dt.ToString("yyyyMMddHHmm");
		}		
		internal static string GetExcelDateString (DateTime dt)
		{
			return dt.ToString("dd/MM/yyyy");
		}	
		internal static string GetExcelTimeString (DateTime dt)
		{
			return dt.ToString("HH:mm:ss");
		}	
		private ILogBody LogPrevious = null;
		private ILogBody logCurrent = null;
		
		public ILogBody GetToday()
		{
			return logCurrent;
		}
		
		public ILogBody GetToday(DateTime date)
		{
			if (this.Count>0) 	
			{
				return this.Find(x => 
				                 { var iday = (IDate)x; 
				                 	var xDate = new DateTime(int.Parse(iday.year_0), int.Parse(iday.month_0), int.Parse(iday.day_0));
				                 	return DateTime.Equals(xDate,date);});
			}
			else return (ILogBody)null;
		}
		
		public bool HandleLogEntry(WHInfo wh, ILogBody log)
		{
			bool ret = false;
			var sav = SaveFolder;
			if(!string.IsNullOrWhiteSpace(sav))
			if (log !=null)
			{
				logCurrent = log;
				var typeCur = log.GetType().Name;
				var typeOfClass = typeof(T).Name;
				var typeOfStack = typeof(LogCommandOnStack).Name;
				if (typeCur==typeOfStack) 
					return true; // skip that type

				var typePrev = "";
				if (LogPrevious!=null) 
					typePrev = LogPrevious.GetType().Name;
				
				try {
		
						if(!(typePrev.Equals(typeCur)) && typeCur.Equals(typeOfClass)) // first time
						{
							var tt = (T)log;
							tt.HandleType(wh);
							this.Add(tt);
							//System.Diagnostics.Debug.WriteLine(tt.ToString());
						}
						else if(typePrev.Equals(typeOfClass) && typeCur.Equals(typeOfClass)) // following
						{
							var tt = (T)log;
							tt.HandleType(wh);
							this.Add(tt);
							//System.Diagnostics.Debug.WriteLine(tt.ToString());
						}
						else if(!typePrev.Equals(typeCur) && !typeCur.Equals(typeOfClass) && typePrev.Equals(typeOfClass))
						{
							// Publish
								
							System.Diagnostics.Debug.WriteLine("Start publication");
							
							IsPublished = this.PublishLogList(wh);
							if (LineExtEnabled)
								if (LogPrevious is IScribeExt)
									this.PublishLogListExt(wh);
							
							System.Diagnostics.Debug.WriteLine("End publication");
							ret = true;
						}
					} catch (Exception e) 
					{
						System.Diagnostics.Debug.WriteLine(e.ToString());
						
					} finally 
					{
						LogPrevious = log;
					}
			}
			else if (log == null && this.Count>0)
			{
				IsPublished = this.PublishLogList(wh);
				ret = true;
			}
			return ret;
		}
		
		public bool PublishLogList(WHInfo wh)
		{
			var dt = DateTime.Now;
			var published = false;
			try {
				if (this.Count>0)
				{
					string csv_filename = string.Format("{0}_{1}_{2}_{3}.csv", GetMyDateString(dt), wh.Connection.MacRadical, typeof(T).Name, DateTime.Now.Millisecond);
					string csv_filepath = System.IO.Path.Combine(SaveFolder, csv_filename);
					UtilClass.CheckCreateFolder(SaveFolder);
					published = PublishLogList(wh, csv_filepath);
					// auto - open
					if (System.IO.File.Exists(csv_filepath))
						System.Diagnostics.Process.Start("explorer.exe", csv_filepath);
				}
			} catch (Exception ex) 
			{
				System.Diagnostics.Debug.WriteLine("Error PublishLogList ");
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}

			return published;
		}
		
		private static void WriteTableHeader(System.IO.StreamWriter w, WHInfo wh)
		{
			var dt = DateTime.Now;
			if (w!=null)
			{
				w.WriteLine("sep=;"); // force the separator to ; whatever the PC culture
				w.WriteLine(typeof(T).Name);
				w.WriteLine("Report; Date;{0};Time;{1};",GetExcelDateString(dt), GetExcelTimeString(dt)); // Set that line as text
				w.WriteLine("Watch; Date;{0};Time;{1};", wh.Time.stringDate, wh.Time.stringTime);
				w.WriteLine("Watch; Version;{0};Info;{1};", wh.Version.ToHeader(), wh.Version.ToString());
				w.WriteLine("Watch; ID;'{0};MAC address;{1};", wh.Connection.MacRadical, wh.Connection.MacAddress );
				w.WriteLine("Device;'{0};Model;'{1};OS;'{2};", wh.Connection.Device, wh.Connection.Model, wh.Connection.OS);
				w.WriteLine("RemoteApp;'{0}", wh.Connection.AppVersion);
				w.WriteLine("");			
			}
		}
		
		public bool PublishLogList(WHInfo wh, string path)
		{
			bool  published = false;
			try {
					System.Diagnostics.Debug.WriteLine("Start Publishing ... LogList = " + path);
					
					using (var fsAppend = new System.IO.FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
					{
						using(var w = new System.IO.StreamWriter(fsAppend))
						{
							if (this.Count>0)
							{
								WriteTableHeader(w, wh);
								
								var item = (T)(this[0]);
								if (item!=null)
									w.WriteLine(item.ToHeader());
							}
							for (int i = 0; i < this.Count; i++) 
							{
								var item = (T)(this[i]);
								if (item!=null)
								{
									var line = ((IScribe)item).ToLine();
							        w.WriteLine(line);
							        w.Flush();
								}
								published = true;
						    }
							this.Clear();
						}
					}
					System.Diagnostics.Debug.WriteLine("Stop Publishing ... LogList = " + path);
					
				} 
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error PublishLogList " + path);
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}
				
				
			return published;
		}
		
		public void PublishLogListExt(WHInfo wh)
		{
			var dt = DateTime.Now;
			string csv_filename = string.Format("{0}_{1}_{2}-line_{3}.csv", GetMyDateString(dt), wh.Connection.MacRadical, typeof(T).Name, DateTime.Now.Millisecond);
			string csv_filepath = System.IO.Path.Combine(SaveFolder, csv_filename);
			UtilClass.CheckCreateFolder(SaveFolder);
			PublishLogListExt(wh, csv_filepath);
			// auto - open
			if (AutoOpenCSV)
				if (System.IO.File.Exists(csv_filepath))
					System.Diagnostics.Process.Start("explorer.exe", csv_filepath);
		}
		
		public void PublishLogListExt(WHInfo wh, string path)
		{
			try {
				using (var fsAppend = new System.IO.FileStream(path, FileMode.Append, FileAccess.Write))
				using(var w = new System.IO.StreamWriter(fsAppend))
				{
					if (this.Count>0)
					{
						WriteTableHeader(w,wh);
						
						var item = (IScribeExt)((T)this[0]);
						if (item!=null)
							w.WriteLine(item.ToHeaderExt());
					}
					for (int i = 0; i < this.Count; i++) 
					{
						var item = (T)(this[i]);
						if (item!=null)
						{
							var line = ((IScribeExt)item).ToLineExt();
					        w.WriteLine(line);
					        w.Flush();
						}
				    }
					this.Clear();
				}	
			} catch (Exception ex) 
			{
				System.Diagnostics.Debug.WriteLine("Error PublishLogList " + path);
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}			
		}		

	}
}
