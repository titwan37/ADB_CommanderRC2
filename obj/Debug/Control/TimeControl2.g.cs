﻿#pragma checksum "..\..\..\Control\TimeControl2.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A24C0CF5066FBD7B99C0298F1BF1465B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace WPF_S39_Commander.Control {
    
    
    /// <summary>
    /// TimeControl2
    /// </summary>
    public partial class TimeControl2 : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\..\Control\TimeControl2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WPF_S39_Commander.Control.TimeControl2 UserControl;
        
        #line default
        #line hidden
        
        
        #line 7 "..\..\..\Control\TimeControl2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\Control\TimeControl2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid hour;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Control\TimeControl2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock mmTxt;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\Control\TimeControl2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock sep1;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\Control\TimeControl2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid min;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Control\TimeControl2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ddTxt;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Control\TimeControl2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock sep2;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Control\TimeControl2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid sec;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\Control\TimeControl2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock yyTxt;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WPF_S39_ADB_Commander;component/control/timecontrol2.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Control\TimeControl2.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.UserControl = ((WPF_S39_Commander.Control.TimeControl2)(target));
            return;
            case 2:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.hour = ((System.Windows.Controls.Grid)(target));
            
            #line 16 "..\..\..\Control\TimeControl2.xaml"
            this.hour.KeyDown += new System.Windows.Input.KeyEventHandler(this.Down);
            
            #line default
            #line hidden
            return;
            case 4:
            this.mmTxt = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.sep1 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.min = ((System.Windows.Controls.Grid)(target));
            
            #line 32 "..\..\..\Control\TimeControl2.xaml"
            this.min.KeyDown += new System.Windows.Input.KeyEventHandler(this.Down);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ddTxt = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.sep2 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.sec = ((System.Windows.Controls.Grid)(target));
            
            #line 48 "..\..\..\Control\TimeControl2.xaml"
            this.sec.KeyDown += new System.Windows.Input.KeyEventHandler(this.Down);
            
            #line default
            #line hidden
            return;
            case 10:
            this.yyTxt = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

