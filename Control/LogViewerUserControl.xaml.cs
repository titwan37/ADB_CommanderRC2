/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02.05.2016
 * Time: 09:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPF_S39_Commander.Control
{
		/// <summary>
		/// Interaction logic for LogViewerUserControl.xaml
		/// </summary>
		public partial class LogViewerUserControl : UserControl
		{
			private string TestData = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
	        private System.Threading.Timer Timer;
        	private System.Random random;
	        private List<string> words;
	        private int maxword;
	        private int index;

			public bool AutoScroll { get; set; }

			public ObservableCollection<LogEntry> LogEntries { get; set; }
			
			public LogViewerUserControl()
			{
				InitializeComponent();
				
				random = new Random();
				words = TestData.Split(' ').ToList();
				maxword = words.Count - 1;
				
				DataContext = LogEntries = new ObservableCollection<LogEntry>();
				Enumerable.Range(0, 200000)
				          .ToList()
				          .ForEach(x => LogEntries.Add(GetRandomEntry()));
				
				Timer = new System.Threading.Timer(x => AddRandomEntry(), null, 1000, 10);
			}
			
					private void AddRandomEntry()
        {
            Dispatcher.BeginInvoke((Action) (() => LogEntries.Add(GetRandomEntry())));
        }

        private LogEntry GetRandomEntry()
        {
            if (random.Next(1,10) > 1)
            {
                return new LogEntry()
                {
                    Index = index++,
                    DateTime = DateTime.Now,
                    Message = string.Join(" ", Enumerable.Range(5, random.Next(10, 50))
                                                         .Select(x => words[random.Next(0, maxword)])),
                };
            }

            return new CollapsibleLogEntry()
           	{
               Index = index++,
               DateTime = DateTime.Now,
               Message = string.Join(" ", Enumerable.Range(5, random.Next(10, 50))
                                            .Select(x => words[random.Next(0, maxword)])),
               Contents = Enumerable.Range(5, random.Next(5, 10))
                                    .Select(i => GetRandomEntry())
                                    .ToList()
      	     };
	        }

			
			
			private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
			{
				if (e.Source is ScrollViewer)
				{
					var sc = (ScrollViewer)e.Source as ScrollViewer;
					if (sc!=null)
					{
					    // User scroll event : set or unset autoscroll mode
					    if (e.ExtentHeightChange == 0)
					    {   // Content unchanged : user scroll event
					        if (sc.VerticalOffset == sc.ScrollableHeight)
					        {   // Scroll bar is in bottom
					            // Set autoscroll mode
					            AutoScroll = true;
					        }
					        else
					        {   // Scroll bar isn't in bottom
					            // Unset autoscroll mode
					            AutoScroll = false;
					        }
					    }
					
					    // Content scroll event : autoscroll eventually
					    if (AutoScroll && e.ExtentHeightChange != 0)
					    {   // Content changed and autoscroll mode set
					        // Autoscroll
					        sc.ScrollToVerticalOffset(sc.ExtentHeight);
					    }
					}
				}
			}
		}
	

		public class CollapsibleLogEntry: LogEntry
		{
		    public List<LogEntry> Contents { get; set; }
		}
		
		public class LogEntry: PropertyChangedBase
		{
		    public DateTime DateTime { get; set; }
		    public int Index { get; set; }
		    public System.Windows.Media.Brush Foreground { get; set; }
		    
		    string message ="";
	        public string Message
	        {
	            get
	            {
	                return this.message;
	            }
	
	            set
	            {
	                if (value != this.message)
	                {
	                    this.message = value;
	                    NotifyPropertyChanged("Message");
	                }
	            }
	        }
		}
}