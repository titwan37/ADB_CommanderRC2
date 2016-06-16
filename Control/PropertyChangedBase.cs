/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02.05.2016
 * Time: 13:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Description of PropertyChangedBase.
	/// </summary>
	public class PropertyChangedBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region INotifyPropertyChanged Members
 		
        /// <summary>
 		/// PropertyChanged
 		/// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
 		
        /// <summary>
 		/// NotifyPropertyChanged
 		/// </summary>
 		/// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName)
        {
        	System.Windows.Application.Current.Dispatcher.BeginInvoke((Action) (() => { SendNotifyPropertyChanged(propertyName);}));	       
        }
        
        /// <summary>
        /// SendNotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName"></param>
        public void SendNotifyPropertyChanged(string propertyName)
        {
             PropertyChangedEventHandler handler = PropertyChanged;
             if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));	        	
        }
        
        #endregion
 
        #region INotifyPropertyChanging Members
        
        /// <summary>
        /// PropertyChanging
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;
 
        /// <summary>
        /// NotifyPropertyChanging
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
        #endregion
    }
	
	/*
	public class PropertyChangedBase:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
             {
                 PropertyChangedEventHandler handler = PropertyChanged;
                 if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
             }));
        }
    }
	*/
	
		
	/// <summary> 
	/// Defines an object capable of firing notifications when its properties are changed. 
	/// </summary> 
	public interface IPropertyChangedNotifier : INotifyPropertyChanged 
	{ 
		/// <summary> 
		/// Fires an event which indicates that a property has changed. 
		/// </summary> 
		/// <param name="propertyName">The name of the property that has changed.</param> 
		void SendPropertyChanged(String propertyName); 
	} 
}
