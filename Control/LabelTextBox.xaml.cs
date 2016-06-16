/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 25.02.2016
 * Time: 14:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for LabelTextBox.xaml
	/// </summary>
	public partial class LabelTextBox : UserControl, INotifyPropertyChanged
	{
		public LabelTextBox()
		{
			InitializeComponent();
			tb1.DataContext = this;
			//this.DataContext = this;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanging(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		protected virtual void OnPropertyChanged(string propertyName)
		{
		    if (PropertyChanged != null)
		        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public object LabelContent {
			get { return GetValue(LabelContentProperty); }
			set { SetValue(LabelContentProperty, value); }
		}
		public static readonly DependencyProperty LabelContentProperty =
        DependencyProperty.Register("LabelContent", typeof(object), typeof(LabelTextBox),
        //new FrameworkPropertyMetadata(string.Empty, new System.Windows.PropertyChangedCallback(OnLabelContentPropertyChanged)));
		new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		/*
		public static void OnLabelContentPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	    {
		    LabelTextBox myLabelTextBox = dependencyObject as LabelTextBox;  
		    myLabelTextBox.OnPropertyChanging("LabelContent");  
		    myLabelTextBox.OnLabelContentPropertyChanged(e); 
		    myLabelTextBox.OnPropertyChanged("LabelContent");  
	    }
		private void OnLabelContentPropertyChanged(DependencyPropertyChangedEventArgs e) 
		{ 
		    //Label1.Content = LabelContent; 
		    
		}
		*/

		//LabelWidth
		public double LabelWidth {
			get { return (double)GetValue(LabelWidthProperty); }
			set { SetValue(LabelWidthProperty, (double)value); }
		}		
		public static readonly DependencyProperty LabelWidthProperty =
        DependencyProperty.Register("LabelWidth", typeof(double), typeof(LabelTextBox),
		new FrameworkPropertyMetadata(40.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        //new FrameworkPropertyMetadata(20.0, new System.Windows.PropertyChangedCallback(OnLabelWidthPropertyChanged)));
		/*
        public static void OnLabelWidthPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	    {
		    LabelTextBox myLabelTextBox = dependencyObject as LabelTextBox;  
		    myLabelTextBox.OnPropertyChanging("LabelWidth");  
		    myLabelTextBox.OnLabelWidthPropertyChanged(e); 
		    myLabelTextBox.OnPropertyChanged("LabelWidth");  
	    }
		private void OnLabelWidthPropertyChanged(DependencyPropertyChangedEventArgs e) 
		{ 
		    //Label1.Width = LabelWidth; 
		}
		*/		
		
		public String TextBoxText {
			get { return (String)GetValue(TextBoxTextProperty); }
			set { SetValue(TextBoxTextProperty, (value)); }
		}
		public static readonly DependencyProperty TextBoxTextProperty =
	       DependencyProperty.Register("TextBoxText", typeof(String), typeof(LabelTextBox),
		   new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));                            
	       //new FrameworkPropertyMetadata(string.Empty, new System.Windows.PropertyChangedCallback(OnTextBoxTextPropertyChanged)));
		/*
		public static void OnTextBoxTextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	    {
		    LabelTextBox myLabelTextBox = dependencyObject as LabelTextBox;  
		    myLabelTextBox.OnPropertyChanging("TextBoxText");  
			myLabelTextBox.OnTextBoxTextPropertyChanged(e);
		    myLabelTextBox.OnPropertyChanged("TextBoxText");  
	    }
		
		private void OnTextBoxTextPropertyChanged(DependencyPropertyChangedEventArgs e) 
		{ 
			//TextBox1.Text = TextBoxText;
		}
		*/

		public bool TextBoxIsReadOnly {
			get { return (bool)GetValue(TextBoxIsReadOnlyProperty); }
			set { SetValue(TextBoxIsReadOnlyProperty, (bool)value); }
		}		
		public static readonly DependencyProperty TextBoxIsReadOnlyProperty =
        DependencyProperty.Register("TextBoxIsReadOnly", typeof(bool), typeof(LabelTextBox),
		new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		/*
		object labelContent = "Label";
		public object LabelContent2 {
			get { return labelContent = Label1.Content; }
			set { labelContent = Label1.Content = value; OnPropertyChanged("LabelContent2"); }
		}
		string _TextBoxText = "Text";
		public string TextBoxText2
		{
			get { return _TextBoxText = TextBox1.Text; }
			set { _TextBoxText = TextBox1.Text = value; OnPropertyChanged("TextBoxText2"); }
		}		
		bool _TextBoxIsReadOnly = false;
		public bool TextBoxIsReadOnly
		{
			get { return _TextBoxIsReadOnly = TextBox1.IsReadOnly; }
			set { _TextBoxIsReadOnly = TextBox1.IsReadOnly = value; OnPropertyChanged("TextBoxIsReadOnly"); }
		}			
		*/
	}

}