/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/16/2016
 * Time: 10:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;

namespace WPF_S39_Commander.Properties
{
	/// <summary>
	/// Description of ConfigurationReader.
	/// </summary>
	public class ConfigurationReader
	{
		public ConfigurationReader()
		{
		}
		
		public static List<string> ReadCfgFile(string filename)
		{
        	List<string> LogList = new List<string>();
        	// TXT file
			if (File.Exists(filename)) { var logFile = File.ReadAllLines(filename); LogList = new List<string>(logFile); }
			return LogList;
		}	
	}
}
