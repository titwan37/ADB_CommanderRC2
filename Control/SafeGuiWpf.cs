/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 22.04.2016
 * Time: 18:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Description of SafeGuiWpf.
	/// </summary>
	public class SafeGuiWpf
	{
	    public static object GetTag(System.Windows.Controls.Control C)
	    {
	        if (C.Dispatcher.CheckAccess()) return C.Tag;
	        else return C.Dispatcher.Invoke(new Func<System.Windows.Controls.Control, object>(GetTag), C);
	    }
	    public static string GetText(TextBox TB)
	    {
	        if (TB.Dispatcher.CheckAccess()) return TB.Text;
	        else return (string)TB.Dispatcher.Invoke(new Func<TextBox,string>(GetText), TB);
	    }
	    public static string GetText(TextBlock TB)
	    {
	        if (TB.Dispatcher.CheckAccess()) return TB.Text;
	        else return (string)TB.Dispatcher.Invoke(new Func<TextBlock,string>(GetText), TB);
	    }
	    public static string GetText(ComboBox TB)
	    {
	        if (TB.Dispatcher.CheckAccess()) return TB.Text;
	        else return (string)TB.Dispatcher.Invoke(new Func<ComboBox,string>(GetText), TB);
	    }
	
	    public static string GetText(PasswordBox TB)
	    {
	        if (TB.Dispatcher.CheckAccess()) return TB.Password;
	        else return (string)TB.Dispatcher.Invoke(new Func<PasswordBox, string>(GetText), TB);
	    }
	
	    public static void SetText(TextBlock TB, string Str)
	    {
	        if (TB.Dispatcher.CheckAccess()) TB.Text = Str;
	        else TB.Dispatcher.Invoke(new Action<TextBlock,string>(SetText), TB, Str);
	    }
	    public static void SetText(TextBox TB, string Str)
	    {
	        if (TB.Dispatcher.CheckAccess()) TB.Text = Str;
	        else TB.Dispatcher.Invoke(new Action<TextBox, string>(SetText), TB, Str);
	    }
	    public static void AppendText(TextBox TB, string Str)
	    {
	        if (TB.Dispatcher.CheckAccess())
	        {
	            TB.AppendText(Str);
	            TB.ScrollToEnd(); // scroll to end?
	        }
	        else TB.Dispatcher.Invoke(new Action<TextBox, string>(AppendText), TB, Str);
	    }
	    public static bool? GetChecked(CheckBox Ck)
	    {
	        if (Ck.Dispatcher.CheckAccess()) return Ck.IsChecked;
	        else return (bool?)Ck.Dispatcher.Invoke(new Func<CheckBox,bool?>(GetChecked), Ck);
	    }
	    public static void SetChecked(CheckBox Ck, bool? V)
	    {
	        if (Ck.Dispatcher.CheckAccess()) Ck.IsChecked = V;
	        else Ck.Dispatcher.Invoke(new Action<CheckBox, bool?>(SetChecked), Ck, V);
	    }
	    public static bool GetChecked(MenuItem Ck)
	    {
	        if (Ck.Dispatcher.CheckAccess()) return Ck.IsChecked;
	        else return (bool)Ck.Dispatcher.Invoke(new Func<MenuItem, bool>(GetChecked), Ck);
	    }
	    public static void SetChecked(MenuItem Ck, bool V)
	    {
	        if (Ck.Dispatcher.CheckAccess()) Ck.IsChecked = V;
	        else Ck.Dispatcher.Invoke(new Action<MenuItem, bool>(SetChecked), Ck, V);
	    }
	    public static bool? GetChecked(RadioButton Ck)
	    {
	        if (Ck.Dispatcher.CheckAccess()) return Ck.IsChecked;
	        else return (bool?)Ck.Dispatcher.Invoke(new Func<RadioButton, bool?>(GetChecked), Ck);
	    }
	    public static void SetChecked(RadioButton Ck, bool? V)
	    {
	        if (Ck.Dispatcher.CheckAccess()) Ck.IsChecked = V;
	        else Ck.Dispatcher.Invoke(new Action<RadioButton, bool?>(SetChecked), Ck, V);
	    }
	
	    public static void SetVisible(UIElement Emt, Visibility V)
	    {
	        if (Emt.Dispatcher.CheckAccess()) Emt.Visibility = V;
	        else Emt.Dispatcher.Invoke(new Action<UIElement, Visibility>(SetVisible), Emt, V);
	    }
	    public static Visibility GetVisible(UIElement Emt)
	    {
	        if (Emt.Dispatcher.CheckAccess()) return Emt.Visibility;
	        else return (Visibility)Emt.Dispatcher.Invoke(new Func<UIElement, Visibility>(GetVisible), Emt);
	    }
	    public static bool GetEnabled(UIElement Emt)
	    {
	        if (Emt.Dispatcher.CheckAccess()) return Emt.IsEnabled;
	        else return (bool)Emt.Dispatcher.Invoke(new Func<UIElement, bool>(GetEnabled), Emt);
	    }
	    public static void SetEnabled(UIElement Emt, bool V)
	    {
	        if (Emt.Dispatcher.CheckAccess()) Emt.IsEnabled = V;
	        else Emt.Dispatcher.Invoke(new Action<UIElement, bool>(SetEnabled), Emt, V);
	    }
	
	    public static void SetSelectedItem(Selector Ic, object Selected)
	    {
	        if (Ic.Dispatcher.CheckAccess()) Ic.SelectedItem = Selected;
	        else Ic.Dispatcher.Invoke(new Action<Selector, object>(SetSelectedItem), Ic, Selected);
	    }
	    public static object GetSelectedItem(Selector Ic)
	    {
	        if (Ic.Dispatcher.CheckAccess()) return Ic.SelectedItem;
	        else return Ic.Dispatcher.Invoke(new Func<Selector, object>(GetSelectedItem), Ic);
	    }
	    public static int GetSelectedIndex(Selector Ic)
	    {
	        if (Ic.Dispatcher.CheckAccess()) return Ic.SelectedIndex;
	        else return (int)Ic.Dispatcher.Invoke(new Func<Selector, int>(GetSelectedIndex), Ic);
	    }
	
	    delegate MessageBoxResult MsgBoxDelegate(Window owner, string text, string caption, MessageBoxButton button, MessageBoxImage icon);
	    public static MessageBoxResult MsgBox(Window owner, string text, string caption, MessageBoxButton button, MessageBoxImage icon)
	    {
	        if (owner.Dispatcher.CheckAccess()) return MessageBox.Show(owner, text, caption, button, icon);
	        else return (MessageBoxResult)owner.Dispatcher.Invoke(new MsgBoxDelegate(MsgBox), owner, text, caption, button, icon);
	    }
	
	    public static double GetRangeValue(RangeBase RngBse)
	    {
	        if (RngBse.Dispatcher.CheckAccess()) return RngBse.Value;
	        else return (double)RngBse.Dispatcher.Invoke(new Func<RangeBase, double>(GetRangeValue), RngBse);
	    }
	    public static void SetRangeValue(RangeBase RngBse, double V)
	    {
	        if (RngBse.Dispatcher.CheckAccess()) RngBse.Value = V;
	        else RngBse.Dispatcher.Invoke(new Action<RangeBase, double>(SetRangeValue), RngBse, V);
	    }
	
	    public static T CreateWindow<T>(Window Owner) where T : Window, new()
	    {
	        if (Owner.Dispatcher.CheckAccess())
	        {
	            var Win = new T(); // Window created on GUI thread
	            Win.Owner = Owner;
	            return Win;
	        }
	        else return (T)Owner.Dispatcher.Invoke(new Func<Window, T>(CreateWindow<T>), Owner);
	    }
	
	    public static bool? ShowDialog(Window Dialog)
	    {
	        if (Dialog.Dispatcher.CheckAccess()) return Dialog.ShowDialog();
	        else return (bool?)Dialog.Dispatcher.Invoke(new Func<Window, bool?>(ShowDialog), Dialog);
	    }
	
	    public static void SetDialogResult(Window Dialog, bool? Result)
	    {
	        if (Dialog.Dispatcher.CheckAccess()) Dialog.DialogResult = Result;
	        else Dialog.Dispatcher.Invoke(new Action<Window, bool?>(SetDialogResult), Dialog, Result);
	    }
	
	    public static Window GetWindowOwner(Window window)
	    {
	        if (window.Dispatcher.CheckAccess()) return window.Owner;
	        else return (Window)window.Dispatcher.Invoke(new Func<Window, Window>(GetWindowOwner), window);
	    }
	
	} // END CLASS: SafeGuiWpf
}

