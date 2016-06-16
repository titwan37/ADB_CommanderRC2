﻿/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 19.02.2016
 * Time: 15:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
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
	/// Interaction logic for ClickToEditTextboxControl.xaml
	/// </summary>
	public partial class ClickToEditTextboxControl : UserControl
	{
		public ClickToEditTextboxControl()
		{
			InitializeComponent();
		}

	    public string Text
	    {
	        get { return (string)GetValue(TextProperty); }
	        set { SetValue(TextProperty, value); }
	    }
	
	    // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
	    public static readonly DependencyProperty TextProperty =
	        DependencyProperty.Register("Text", typeof(string), typeof(ClickToEditTextboxControl), new UIPropertyMetadata());
	
	    private void textBoxName_LostFocus(object sender, RoutedEventArgs e)
	    {
	        var txtBlock = (TextBlock)((Grid)((TextBox)sender).Parent).Children[0];
	
	        txtBlock.Visibility = Visibility.Visible;
	        ((TextBox)sender).Visibility = Visibility.Collapsed;
	        
	        // raise event
	        if (OnTextBoxLostFocus!=null) OnTextBoxLostFocus(sender, e);
	    }
	
	    public event RoutedEventHandler OnTextBoxLostFocus;
	    public event MouseButtonEventHandler OnTextBoxMouseDown;
	    
	    private void textBlockName_MouseDown(object sender, MouseButtonEventArgs e)
	    {
	        var txtBox = (TextBox)((Grid)((TextBlock)sender).Parent).Children[1];
	        
	        
	        txtBox.Visibility = Visibility.Visible;
	        ((TextBlock)sender).Visibility = Visibility.Collapsed;
	        // raise event
	        if (OnTextBoxMouseDown!=null) OnTextBoxMouseDown(sender, e);
	    }
	}
}
