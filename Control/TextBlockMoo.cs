/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02.05.2016
 * Time: 16:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Description of TextBlockMoo.
	/// </summary>
	/// <summary>
	/// TextBlockMoo
	/// </summary>
	public class TextBlockMoo : TextBlock 
	{
	    public String SelectedText = "";
	
	    public delegate void TextSelectedHandler(string SelectedText);
	    public event TextSelectedHandler OnTextSelected;
	    public event TextSelectedHandler OnMouseDoubleClick;
	    
	    protected void RaiseEvent_OnTextSelected()
	    {
	        if (OnTextSelected != null){OnTextSelected(SelectedText);}
	    }
	    protected void RaiseEvent_OnMouseDoubleClick()
	    {
	        if (OnMouseDoubleClick != null){OnMouseDoubleClick(SelectedText);}
	    }
	    
	    TextPointer StartSelectPosition;
	    TextPointer EndSelectPosition;
	    object _saveForeGroundBrush;
	    object _saveBackGroundBrush;
	    	
	    TextRange _ntr = null;
	    
	    protected override void OnMouseDown(MouseButtonEventArgs e)
	    {
	        base.OnMouseDown(e);
	        
	        if (e.ClickCount>=2)
	        {
	        	//SelectedText = this.Text;
	        	RaiseEvent_OnMouseDoubleClick();
	        }
	        
	        if (_ntr!=null) {
	        	try {
	        		if (_saveForeGroundBrush!=null)
	        			_ntr.ApplyPropertyValue(TextElement.ForegroundProperty, _saveForeGroundBrush);
	        		if (_saveBackGroundBrush!=null)
		        		_ntr.ApplyPropertyValue(TextElement.BackgroundProperty, _saveBackGroundBrush);
	        	} catch (Exception ex) {
	        		System.Diagnostics.Debug.WriteLine(ex.ToString());
	        	}
	        }
	        
	        Point mouseDownPoint = e.GetPosition(this);
	        StartSelectPosition = this.GetPositionFromPoint(mouseDownPoint, true);            
	    }
	    	
	    protected override void OnMouseUp(MouseButtonEventArgs e)
	    {
	    	if(e==null) return;
	    	
	        base.OnMouseUp(e);
	        Point mouseUpPoint = e.GetPosition(this);
	    	if(mouseUpPoint==null) return;
		        EndSelectPosition = this.GetPositionFromPoint(mouseUpPoint, true);
	
	    	if(StartSelectPosition==null) return;
	    	if(EndSelectPosition==null) return;
	        _ntr = new TextRange(StartSelectPosition, EndSelectPosition);
	        
	        // keep saved
	        _saveForeGroundBrush = _ntr.GetPropertyValue(TextElement.ForegroundProperty);
	        _saveBackGroundBrush = _ntr.GetPropertyValue(TextElement.BackgroundProperty);
	        //System.Diagnostics.Debug.WriteLine(_saveForeGroundBrush.GetType().ToString());
	        // change style
	        _ntr.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Yellow));
	        _ntr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.DarkBlue));
	        
	        SelectedText = _ntr.Text;
	        if (!string.IsNullOrEmpty(SelectedText)) RaiseEvent_OnTextSelected();
	    }
	
	    public string GetTextValue()
	    {
	    	TextRange textRange = new TextRange(this.ContentStart,this.ContentEnd);
		    return textRange.Text;
	    	//return this.Text;
	    }
	    
		public TextBlockMoo()
		{
		   //Edit_Command=new RelayCommand(()=>execute_me());
		}
		
		public void execute_me()
		{
		   //write your code here
			System.IO.File.WriteAllText("SelectedText.txt", this.Text);
			System.Diagnostics.Process.Start("explorer.exe", "SelectedText.txt" );
		}
		
		/*
		RelayCommand _editCommand; 
		public RelayCommand Edit_Command
		{
			//get {if (_editCommand== null) _editCommand = new RelayCommand();}
			get;
			private set;
		}
		*/
		
		/*
		RelayCommand _saveCommand; 
		public ICommand SaveCommand 
		{ 
			get { if (_saveCommand == null) 
				{ _saveCommand = new RelayCommand(param => this.Save(), param => this.CanSave ); }
				return _saveCommand; }
		}
		*/
		
		/*
		public static readonly DependencyProperty MaxLinesProperty =
        DependencyProperty.RegisterAttached(
            "MaxLines",
            typeof(int),
            typeof(TextBlockMoo),
            new PropertyMetadata(default(int), OnMaxLinesPropertyChangedCallback));

	    public static void SetMaxLines(DependencyObject element, int value)
	    {
	        element.SetValue(MaxLinesProperty, value);
	    }
	
	    public static int GetMaxLines(DependencyObject element)
	    {
	        return (int)element.GetValue(MaxLinesProperty);
	    }
	
	    private static void OnMaxLinesPropertyChangedCallback(
	        DependencyObject d,
	        DependencyPropertyChangedEventArgs e)
	    {
	        var element = d as TextBlock;
	        if (element != null)
	        {
	            //element.MaxHeight = element.LineHeight * GetMaxLines(element);
	        }
	    }
*/
	}
	
	
	/// <summary>
	/// RelayCommand
	/// </summary>
	public class RelayCommand : ICommand { 
		#region Fields 
		readonly Action<object> _execute; 
		readonly Predicate<object> _canExecute; 
	    //private Func<object, bool> canExecute;     
		#endregion // Fields 
		#region Constructors 
		public RelayCommand(Action<object> execute) : this(execute, null) { } 
		public RelayCommand(Action<object> execute, Predicate<object> canExecute) 
		{ if (execute == null) throw new ArgumentNullException("execute"); 
			_execute = execute; 
			_canExecute = canExecute; 
		}
//	    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)     
//	    {     
//	        this._execute = execute;     
//	        this.canExecute = canExecute;     
//	    }     
		#endregion // Constructors 
		#region ICommand Members 
		[System.Diagnostics.DebuggerStepThrough]
	    public bool CanExecute(object parameter)     
	    {     
	        return this._canExecute == null || this._canExecute(parameter);     
	    }     
		//public bool CanExecute(object parameter) { return _canExecute == null ? true : _canExecute(parameter); } 
		public event EventHandler CanExecuteChanged 
		{ 	
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; } 
		}
		#endregion // ICommand Members 
	    public void Execute(object parameter)     
	    {     
	    	if (_execute!=null) _execute(parameter);
	    }     
	}  

}
