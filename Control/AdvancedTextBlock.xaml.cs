/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 24.03.2016
 * Time: 18:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Interaction logic for AdvancedTextBlock.xaml
	/// </summary>
	public partial class AdvancedTextBlock : UserControl
	{
			
		public AdvancedTextBlock()
		{
			InitializeComponent();
			
			DataContext = LogEntries = new ObservableCollection<LogEntry>();
			
			this.textBlock1.OnTextSelected += textBlock1_OnTextSelected;
			this.textBlock1.OnMouseDoubleClick += textBlock1_OnMouseDoubleClick;
		}

		public bool AutoScroll { get; set; }
		
		public void AddInline(Run run)
		{
			this.Inlines.Add(run);
		}

		
		public InlineCollection Inlines {
			get { return textBlock1.Inlines; }
			//set { SetValue(InlinesProperty, value); }
		}
		
		public static readonly DependencyProperty InlinesProperty =
		DependencyProperty.Register("Inlines", typeof(InlineCollection), typeof(AdvancedTextBlock),
		new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


		public ObservableCollection<LogEntry> LogEntries { get; set; }
		
		#region Bindeable Control
		public ObservableCollection<Run> InlineList
		{
		    get { return (ObservableCollection<Run>)GetValue(InlineListProperty); }
		    set { SetValue(InlineListProperty, value); }
		}
		
		public static readonly DependencyProperty InlineListProperty =
		    DependencyProperty.Register("InlineList",typeof(ObservableCollection<Run>), typeof(AdvancedTextBlock), new UIPropertyMetadata(null, OnPropertyChanged));
		
		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
		    AdvancedTextBlock textBlock = sender as AdvancedTextBlock;
		    ObservableCollection<Run> list = e.NewValue as ObservableCollection<Run>;
		    list.CollectionChanged += new NotifyCollectionChangedEventHandler(textBlock.InlineCollectionChanged);
		}
		
		private void InlineCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
		    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
		    {
		        int idx = e.NewItems.Count -1;
		        var inline = e.NewItems[idx] as UIElement;
		        this.textBlock1.Inlines.Add(inline);
		    }
		}
		#endregion

		#region QuickAccess
		private void textBlock1_OnTextSelected(string SelectedText)
	    {
	        System.Windows.Clipboard.SetText(SelectedText, TextDataFormat.Text); 
	        //Clipboard.SetDataObject(SelectedText, true); 
	    }

		private void textBlock1_OnMouseDoubleClick(string SelectedText)
		{
			System.IO.File.WriteAllText("SelectedText.txt", GetText());
			System.Diagnostics.Process.Start("explorer.exe", "SelectedText.txt" );
		}
		
		public string GetText()
		{
			string returnText="";
			//returnText = SafeGuiWpf.GetText((TextBlock)this.textBlock1);
			if (textBlock1.Dispatcher.Thread == System.Threading.Thread.CurrentThread)  
			{	
				//textBlock1.Focus();
				returnText = textBlock1.GetTextValue();
			}
			else
			{
				textBlock1.Dispatcher.Invoke(new Action(()=>{ returnText = GetText(); }),System.Windows.Threading.DispatcherPriority.Normal);
				//returnText = (string)textBlock1.Dispatcher.Invoke(new Func<TextBox,string>(GetText), textBlock1);
				//Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,(ThreadStart)delegate { returnText = GetText(); });
			}
			return returnText;
		}		
		#endregion

		#region HighlightText
		
		private void Mouse_Click(object sender, RoutedEventArgs e)
		{
		    Drawing textBlockDrawing = VisualTreeHelper.GetDrawing(textBlock1);
		    var sb = new StringBuilder();
		    WalkDrawingForText(sb, textBlockDrawing);
			// TODO: what you want
		    //System.Diagnostics.Debug.WriteLine(sb.ToString());
		}
		
		private static void WalkDrawingForText(StringBuilder sb, Drawing d)
		{
		    var glyphs = d as GlyphRunDrawing;
		    if (glyphs != null)
		    {
		        sb.Append(glyphs.GlyphRun.Characters.ToArray());
		    }
		    else
		    {
		        var g = d as DrawingGroup;
		        if (g != null)
		        {
		            foreach (Drawing child in g.Children)
		            {
		                WalkDrawingForText(sb, child);
		            }
		        }
		    }
		}
		
		private string GetTextFromVisual(Visual v)
		{
		    Drawing textBlockDrawing = VisualTreeHelper.GetDrawing(v);
		    var glyphs = new List<PositionedGlyphs>();
		
		    WalkDrawingForGlyphRuns(glyphs, Transform.Identity, textBlockDrawing);
		
		    // Round vertical position, to provide some tolerance for rounding errors
		    // in position calculation. Not totally robust - would be better to
		    // identify lines, but that would complicate the example...
		    var glyphsOrderedByPosition = from glyph in glyphs
		                                    let roundedBaselineY = Math.Round(glyph.Position.Y, 1)
		                                    orderby roundedBaselineY ascending, glyph.Position.X ascending
		                                    select new string(glyph.Glyphs.GlyphRun.Characters.ToArray());
		
		    return string.Concat(glyphsOrderedByPosition);
		}
		
		[System.Diagnostics.DebuggerDisplay("{Position}")]
		public struct PositionedGlyphs
		{
		    public PositionedGlyphs(Point position, GlyphRunDrawing grd)
		    {
		        this.Position = position;
		        this.Glyphs = grd;
		    }
		    public readonly Point Position;
		    public readonly GlyphRunDrawing Glyphs;
		}
		
		private static void WalkDrawingForGlyphRuns(List<PositionedGlyphs> glyphList, Transform tx, Drawing d)
		{
		    var glyphs = d as GlyphRunDrawing;
		    if (glyphs != null)
		    {
		        var textOrigin = glyphs.GlyphRun.BaselineOrigin;
		        Point glyphPosition = tx.Transform(textOrigin);
		        glyphList.Add(new PositionedGlyphs(glyphPosition, glyphs));
		    }
		    else
		    {
		        var g = d as DrawingGroup;
		        if (g != null)
		        {
		            // Drawing groups are allowed to transform their children, so we need to
		            // keep a running accumulated transform for where we are in the tree.
		            Matrix current = tx.Value;
		            if (g.Transform != null)
		            {
		                // Note, Matrix is a struct, so this modifies our local copy without
		                // affecting the one in the 'tx' Transforms.
		                current.Append(g.Transform.Value);
		            }
		            var accumulatedTransform = new MatrixTransform(current);
		            foreach (Drawing child in g.Children)
		            {
		                WalkDrawingForGlyphRuns(glyphList, accumulatedTransform, child);
		            }
		        }
		    }
		}
		#endregion
		
		
		#region ScrollChanged
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
	}
}