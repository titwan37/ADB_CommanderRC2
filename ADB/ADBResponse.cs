/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/15/2016
 * Time: 18:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Text;
using Utilities.Endianness;

namespace WPF_S39_Commander.ADB
{
	/// <summary>
	/// Description of ADBResponse.
	/// </summary>
	public class ADBResponse
	{
		public bool IsOkay
		{
			get;
			private set;
		}

		public string Error
		{
			get;
			private set;
		}

		private ADBResponse(bool okay, string error)
		{
			this.IsOkay = okay;
			this.Error = error;
		}

		internal static ADBResponse GetResponse(EndianBinaryReader reader)
		{
			byte[] bytes = reader.ReadBytes(4);
			string @string = Encoding.ASCII.GetString(bytes);
			if (string.IsNullOrEmpty(@string))
			{
				return new ADBResponse(false, "Empty response");
			}
			if (@string != "OKAY")
			{
				string string2 = Encoding.ASCII.GetString(reader.ReadBytes(4));
				int count = int.Parse(string2, NumberStyles.HexNumber);
				string string3 = Encoding.ASCII.GetString(reader.ReadBytes(count));
				return new ADBResponse(false, string3);
			}
			return new ADBResponse(true, null);
		}
	}
}
