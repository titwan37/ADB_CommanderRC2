/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 03/10/2016
 * Time: 12:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Net.Mime;
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
	/// Interaction logic for ProgressButton.xaml
	/// </summary>
	[TemplatePart(Name="TextBlock1", Type=typeof(TextBlock))]
	public partial class ProgressButton : UserControl
	{
		public ProgressButton()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
		public event RoutedEventHandler Click;
		protected virtual void OnClick(object sender, RoutedEventArgs e)
		{
		    RoutedEventHandler rt = Click;
		    if (rt != null)	{ rt(this, e);}
		}	
		
		public void NotifyInputValue(int value)
		{
			if (PbMaximum!=0)
			{
				double perc = ((double)value/(double)PbMaximum);
				this.Percentage = perc;
			}
			else {	this.Percentage = 0; }
		}
		
		public string Text1
        {
            get { return (string)GetValue(Text1Property); }
            set { SetValue(Text1Property, value); }
        }
		public string Text2
        {
            get { return (string)GetValue(Text2Property); }
            set { SetValue(Text2Property, value); }
        }
		public Brush Background1
        {
            get { return (Brush)GetValue(BackGround1Property); }
            set { SetValue(BackGround1Property, value); }
        }
		public int PbValue
        {
            get { return (int)GetValue(PbValueProperty); }
            set { SetValue(PbValueProperty, value); NotifyInputValue(value); }
        }
		public int PbMinimum
        {
            get { return (int)GetValue(PbMinimumProperty); }
            set { SetValue(PbMinimumProperty, value); }
        }
		public int PbMaximum
        {
            get { return (int)GetValue(PbMaximumProperty); }
            set { SetValue(PbMaximumProperty, value); }
        }
		public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }
		
		private static void OnPercentageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ProgressButton circle = sender as ProgressButton;
            circle.Text2 = circle.Percentage.ToString("P1");
        }
        // Using a DependencyProperty as the backing store for Percentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(ProgressButton), 
        	                            new PropertyMetadata(00d, new PropertyChangedCallback(OnPercentageChanged)));
		// Using a DependencyProperty as the backing store for Percentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Text1Property =
            DependencyProperty.Register("Text1", typeof(string), typeof(ProgressButton), 
        	                            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty Text2Property =
            DependencyProperty.Register("Text2", typeof(string), typeof(ProgressButton), 
        	                            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        public static readonly DependencyProperty PbValueProperty =
            DependencyProperty.Register("PbValue", typeof(int), typeof(ProgressButton), 
        	                            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty PbMinimumProperty =
            DependencyProperty.Register("PbMinimum", typeof(int), typeof(ProgressButton), 
        	                            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty PbMaximumProperty =
            DependencyProperty.Register("PbMaximum", typeof(int), typeof(ProgressButton), 
        	                            new FrameworkPropertyMetadata(100, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        public static readonly DependencyProperty BackGround1Property =
            DependencyProperty.Register("Background1", typeof(Brush), typeof(ProgressButton), 
        	                            new FrameworkPropertyMetadata(new SolidColorBrush(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
	}
}