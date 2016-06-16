/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 17.02.2016
 * Time: 12:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace WPF_S39_Commander.Diagnostic
{
	/// <summary>
	/// Description of AuditTrail.
	/// </summary>
	public static class AuditTrail
	{
        public static string Filename = "AuditTrail.log";
        public static string Filepath = "";
		
        private static Mutex mut = new Mutex();
        
        public static string SetFilepath(string exeFolder)
        {
        	var intFolder = Path.Combine(exeFolder, "AuditTrails");
        	UtilClass.CheckCreateFolder(intFolder);
        	return AuditTrail.Filepath = Path.Combine(intFolder, AuditTrail.Filename);
        }
        
		public static string Log(string message, string WHname, string MACaddress)
		{
			string trails = "";
			try 
			{
		        //Debug.WriteLine("{0} is requesting the mutex", Thread.CurrentThread.Name);
		      
		        // Wait until it is safe to enter.
		        mut.WaitOne(100);
		
		        //Debug.WriteLine("{0} has entered the mutex protected area", Thread.CurrentThread.Name);				
				//using (StreamWriter sw = File.AppendText(AuditTrail.Filepath))
				using (var sw = new StreamWriter(new FileStream(AuditTrail.Filepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
			    {
		         	string trail = string.Format("{0}_|_{1}|{2}|{3}", 
					                DateTime.Now.ToString("yyyyMMddHHmmss"), MACaddress, WHname, message);
					             	sw.WriteLine(trail);
					             	trails += trail;
					    sw.Close();
				}
				
				//Debug.WriteLine("{0} is leaving the protected area", Thread.CurrentThread.Name);

		        // Release the Mutex.
		        mut.ReleaseMutex();
		
		        //Debug.WriteLine("{0} has released the mutex", Thread.CurrentThread.Name);
				
				
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine("Error Log ");
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}
			return trails;
		}

		public static string Log(List<string> messages, string WHname, string MACaddress)
		{
			string trails = "";
			
			try {
		        //Debug.WriteLine("{0} is requesting the mutex", Thread.CurrentThread.Name);
		      
		        // Wait until it is safe to enter.
		        mut.WaitOne(1000);
		
		        //Debug.WriteLine("{0} has entered the protected area", Thread.CurrentThread.Name);				

		        //using (StreamWriter sw = File.AppendText(AuditTrail.Filepath))
				using (var sw = new StreamWriter(new FileStream(AuditTrail.Filepath, FileMode.Append, FileAccess.Write, FileShare.Write)))
			    {
					messages.ForEach( x =>  
					                 {
				                 	string trail = string.Format("{0}_|_{1}|{2}|{3}", 
				                 	    DateTime.Now.ToString("yyyyMMddHHmmss"), MACaddress, WHname, x);
					                 	sw.WriteLine(trail);
					                 	trails += trail;
					                 });
				    sw.Close();
			    }
				
				//Debug.WriteLine("{0} is leaving the protected area", Thread.CurrentThread.Name);

		        // Release the Mutex.
		        mut.ReleaseMutex();
		
		        //Debug.WriteLine("{0} has released the mutex", Thread.CurrentThread.Name);
				
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine("Error Log ");
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}
			
			return trails;
		}
		
	}
}
