﻿#pragma checksum "..\..\..\Control\TimeControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "56556C9DEE27769A108F18A718349EE3"
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
    /// TimeControl
    /// </summary>
    public partial class TimeControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\..\Control\TimeControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtHours;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\Control\TimeControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtMinutes;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\Control\TimeControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtAmPm;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\Control\TimeControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnUp;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Control\TimeControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnDown;
        
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
            System.Uri resourceLocater = new System.Uri("/WPF_S39_ADB_Commander;component/control/timecontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Control\TimeControl.xaml"
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
            this.txtHours = ((System.Windows.Controls.TextBox)(target));
            
            #line 17 "..\..\..\Control\TimeControl.xaml"
            this.txtHours.KeyUp += new System.Windows.Input.KeyEventHandler(this.txt_KeyUp);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\Control\TimeControl.xaml"
            this.txtHours.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.txt_MouseWheel);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\Control\TimeControl.xaml"
            this.txtHours.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.txt_PreviewKeyUp);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtMinutes = ((System.Windows.Controls.TextBox)(target));
            
            #line 19 "..\..\..\Control\TimeControl.xaml"
            this.txtMinutes.KeyUp += new System.Windows.Input.KeyEventHandler(this.txt_KeyUp);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\Control\TimeControl.xaml"
            this.txtMinutes.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.txt_MouseWheel);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\Control\TimeControl.xaml"
            this.txtMinutes.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.txt_PreviewKeyUp);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtAmPm = ((System.Windows.Controls.TextBox)(target));
            
            #line 20 "..\..\..\Control\TimeControl.xaml"
            this.txtAmPm.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.txtAmPm_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 20 "..\..\..\Control\TimeControl.xaml"
            this.txtAmPm.KeyUp += new System.Windows.Input.KeyEventHandler(this.txt_KeyUp);
            
            #line default
            #line hidden
            
            #line 20 "..\..\..\Control\TimeControl.xaml"
            this.txtAmPm.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.txt_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnUp = ((System.Windows.Controls.Button)(target));
            
            #line 26 "..\..\..\Control\TimeControl.xaml"
            this.btnUp.Click += new System.Windows.RoutedEventHandler(this.btnUp_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnDown = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\..\Control\TimeControl.xaml"
            this.btnDown.Click += new System.Windows.RoutedEventHandler(this.btnDown_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

