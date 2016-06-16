/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/23/2016
 * Time: 21:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace WPF_S39_Commander
{
	
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public static class UtilClass
	{
		public static void CheckCreateFolder(string folder)
		{
			if(!Directory.Exists(folder))
			{
				var d = Directory.CreateDirectory(folder);
				System.Diagnostics.Debug.WriteLine("Create folder " + d.FullName);
			}
		}
	
		public static void DeleteOldFiles(string folder, DateTime date)
		{
			var dumpfileList = Directory.GetFiles(folder);
			for (int i = 0; i < dumpfileList.Length; i++) 
			{
				var datePattern = date.ToString("yyyyMMDD");
				var fileItem = dumpfileList[i];
				if (!fileItem.Contains(datePattern)) 
				{
					try {
						File.Delete(fileItem);
					} catch (Exception e) {
						System.Diagnostics.Debug.WriteLine("FAILED deleting {0}\n{1}", fileItem, e.ToString());
					}
				}
			}	
		}
	}
}
