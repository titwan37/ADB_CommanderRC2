/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 31.03.2016
 * Time: 13:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
using System;
using System.Windows.Data;
 
 
namespace WPF_S39_Commander.Control
{
	public class OnOffToBooleanConverter : IValueConverter
	{
	        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	        {
	        	try 
	        	{
	            	//if (targetType != typeof(bool))
	            		//throw new InvalidOperationException("The target must be a boolean");
	            	if (value==null)
	            		return false;
	            	else if (value is string)
	            		return ((value.ToString()).Equals("on", StringComparison.InvariantCultureIgnoreCase)) ? true : false;
	            	else 
	            		return System.Windows.DependencyProperty.UnsetValue;
	        	}
	            catch
	            {
	                return System.Windows.DependencyProperty.UnsetValue;
	            }
	        }
	
	        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	        {
	        	if (value==null)
	        		return Binding.DoNothing;
	        	else if (value is bool)
	        		return (bool)value ? "on" : "off";
	        	else 
	        		return Binding.DoNothing;
	//            	var para = parameter;
	//		        var isChecked = System.Convert.ToBoolean(value);
	//		        return isChecked ? para : Binding.DoNothing;
	        }
	}

	public class EmptyStringToZeroConverter : IValueConverter
	{
	    #region IValueConverter Members
	
	    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	    {
	        return value;
	    }
	
	    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	    {
	        return value == null || string.IsNullOrEmpty(value.ToString())
	            ? 0
	            : value;
	    }
	    #endregion
	}
	
	
	public class IntToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
		    if  (value is int)
		    {
			    int number = (int)value;
			    return number.ToString();
		    }
		    else if (value is string)
		    {
		        string s = (string)value;
		        int num;
		        if (int.TryParse(s, out num))
		            return num;
		    }
			return 0;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(value is string)
			{
		        string s = (string)value;
		        int num;
		        if (int.TryParse(s, out num))
		            return num;
			}
			else if (value is int) {
				int number = (int)value;
			    return number.ToString();
		    }
			return "";
		}
	}

		//StringToBooleanConverter
	public class StringToBooleanConverter : IValueConverter
    {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
            	if (value==null||parameter==null)
            		return false;
            	else
            	{
            		var par = parameter.ToString();
            		var myChoice = value.ToString();
            		return par.Equals(myChoice, StringComparison.InvariantCultureIgnoreCase);
            		//return ((value as string).Equals(parameter)) ? true : false;
            	}
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
				var para = parameter;
		        var isChecked = System.Convert.ToBoolean(value);
		        return isChecked ? para : Binding.DoNothing;
            }
    }

	public class DebugDummyConverter : IValueConverter
    {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                    //System.Diagnostics.Debugger.Break();
//                    if (value!=null)
//                    	return value.ToString();
				    if (value is String)
				    {
				        string s = (string)value;
				        return s;
				    }
				    else if  (value is int)
				    {
					    int number = (int)value;
					    return number.ToString();
				    }
		    		return 0;            
            }
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
				    if (value is String)
				    {
				        string s = (string)value;
				        int num;
				        if (int.TryParse(s, out num))
				            return num;
				    }
				    else if  (value is int)
				    {
					    int number = (int)value;
					    return number.ToString();
				    }
	                return value;
            }
    }

}