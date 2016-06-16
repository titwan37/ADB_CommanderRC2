/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02.05.2016
 * Time: 12:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for AdvancedSuperTextBlock.xaml
	/// </summary>
	public partial class AdvancedSuperTextBlock : UserControl
	{
	    private int index;
		public bool AutoScroll { get; set; }
		public ObservableCollection<TextBlockEntry> TextEntries { get; set; }
		public void AddTextEntries(List<string> myHistory)
		{
			myHistory.ForEach((x) => AddInline(x, Colors.Gray));
		}

		private string GetTextValue()
		{
			var clone = TextEntries.ToList();
			var sb = new StringBuilder();

			lock(clone) // lock on the list
			{
			    foreach (var item in clone)
			    {
			        sb.AppendLine(item.ToString());
			    } 
			}
		    return sb.ToString();
		}
		public string GetText()
		{
			string returnText="";
			if (Dispatcher.Thread == System.Threading.Thread.CurrentThread)  
				returnText = GetTextValue();
			else
				Dispatcher.Invoke(new Action(()=>{ returnText = GetTextValue(); }), System.Windows.Threading.DispatcherPriority.Normal);
			return returnText;
		}
		
		public AdvancedSuperTextBlock()
		{
			InitializeComponent();
			
			this.Loaded+= AdvancedSuperTextBlock_Loaded;
			
			DataContext 
				= TextEntries
				= new ObservableCollection<TextBlockEntry>();
		}

		private void AdvancedSuperTextBlock_Loaded(object sender, RoutedEventArgs e)
		{
			this.LogViewer1.MouseLeftButtonDown += LogViewer1_MouseDown;
			this.LogViewer1.MouseRightButtonDown += LogViewer1_MouseDown;
			this.OnTextBlockMouseDoubleClick += LogViewer1_OnMouseDoubleClick;
		}
		
		public void AddInline(Run run)
		{
			AddInline(TextBlockEntry.SetTextBlockEntry(++index,run));
		}
		public void AddInline(string message, Color color)
		{
			AddInline(TextBlockEntry.SetTextBlockEntry(++index,message, color));
		}		
		public void AddInline(TextBlockEntry entry)
		{
			this.TextEntries.Add(entry);
		}		
		
		#region ScrollToEnd
		/// <summary>
		/// ScrollViewer_ScrollChanged
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		#endregion

		#region QuickAccess
	    public String SelectedText = "";
	    public delegate void TextSelectedHandler(string SelectedText);
	    public event TextSelectedHandler OnTextBlockMouseDoubleClick;
	    protected void RaiseEvent_OnMouseDoubleClick() { if (OnTextBlockMouseDoubleClick != null){ OnTextBlockMouseDoubleClick(SelectedText);}	 }
		protected void LogViewer1_MouseDown(object sender, MouseButtonEventArgs e)
		{
	        if (e.ClickCount>=2)
	        {
	        	RaiseEvent_OnMouseDoubleClick();
	        }
		}
		private void LogViewer1_OnMouseDoubleClick(string selectedText)
		{
			System.IO.File.WriteAllText("SelectedText.txt", GetText());
			System.Diagnostics.Process.Start("explorer.exe", "SelectedText.txt" );
		}
		private void textBlock1_OnTextSelected(string selectedText)
	    {
			//selectedText = this.SelectedText;
	        System.Windows.Clipboard.SetText(selectedText, TextDataFormat.Text);
	        //Clipboard.SetDataObject(SelectedText, true); 
	    }
		#endregion
	}
	
	/// <summary>
	/// TextBlockEntry
	/// </summary>
	public class TextBlockEntry: PropertyChangedBase
	{
	    public DateTime DateTime { get; set; }
	    public int Index { get; set; }
	    public System.Windows.Media.Brush Foreground { get; set; }
	    
	    string message ="";
        public string Message
        {
            get { return this.message;}
            set
            {
                if (value != this.message)
                {
                    this.message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }
        
        public static TextBlockEntry SetTextBlockEntry(int index, Run run)
		{
            return new TextBlockEntry()
            {
                Index = index,
                DateTime = DateTime.Now,
                Message = run.Text,
                Foreground = run.Foreground
            };
		}		
		public static TextBlockEntry SetTextBlockEntry(int index, string message, Color color)
		{
            return new TextBlockEntry()
            {
                Index = index,
                DateTime = DateTime.Now,
                Message = message,
                Foreground = new SolidColorBrush(color)
            };
		}

		public override string ToString()
		{
			return string.Format("{2}", Index, DateTime, message);
		}
		public string ToAdvancedString()
		{
			return string.Format("{0};{1:u};{2}", Index, DateTime, message);
		}
	}
}