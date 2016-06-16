/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 31.03.2016
 * Time: 12:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace WPF_S39_Commander
{
	/// <summary>
	/// Description of UtilEnum.
	/// </summary>
	public static class UtilEnum<T>
	{
		/// <summary>
		/// GetNames
		/// </summary>
		/// <returns></returns>
	    public static IList<string> GetNames()
	    {
	        Type type = typeof(T);
	        if (!type.IsEnum)
	            throw new ArgumentException("Type '" + type.Name + "' is not an enum");
	
	        return (
	          from field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
	          where field.IsLiteral
	          select field.Name).ToList<string>();
	    }
	    
	    //public static T ParseEnum<T>(string value)
	    public static T ParseEnum(string value)
		{
		    return (T) Enum.Parse(typeof(T), value, true);
		}
		//public static T ParseEnum<T>(int value)
		public static T ParseEnum(int value)
		{
		    return (T) Enum.ToObject(typeof(T), value);
		}		
	}
	
	/*
	public static class UtilEnum
	{
		//public static T ToEnum<T>(this string value, T defaultValue) 
		public static T ToEnum<T>(this string value, T defaultValue) 
		{
		    if (string.IsNullOrEmpty(value))
		    {
		        return defaultValue;
		    }
		
		    T result;
		    return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
		}
	}*/
	
	
}
