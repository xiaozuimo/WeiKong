﻿#pragma checksum "C:\Users\xiaoz\Documents\Visual Studio 2015\Projects\cqmu\cqmu\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DB78C8DCEB26A61D0C64F5EDB9105780"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cqmu
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.RootGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 2:
                {
                    this.MyVisualStateGroup = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                }
                break;
            case 3:
                {
                    this.Phone = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 4:
                {
                    this.Tablet = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 5:
                {
                    this.PC = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 6:
                {
                    this.MySplitView = (global::Windows.UI.Xaml.Controls.SplitView)(target);
                }
                break;
            case 7:
                {
                    this.btnBurgerMenu = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 243 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnBurgerMenu).Click += this.btnBurgerMenu_Click;
                    #line default
                }
                break;
            case 8:
                {
                    this.rdoHelp = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 194 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.rdoHelp).Checked += this.rdoOption_OnChecked;
                    #line default
                }
                break;
            case 9:
                {
                    this.rdoAbout = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 201 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.rdoAbout).Checked += this.rdoOption_OnChecked;
                    #line default
                }
                break;
            case 10:
                {
                    this.rdoSetting = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 219 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.rdoSetting).Checked += this.rdoOption_OnChecked;
                    #line default
                }
                break;
            case 11:
                {
                    this.rdoCourse = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 171 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.rdoCourse).Checked += this.rdoOption_OnChecked;
                    #line default
                }
                break;
            case 12:
                {
                    this.rdoGrade = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 178 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.rdoGrade).Checked += this.rdoOption_OnChecked;
                    #line default
                }
                break;
            case 13:
                {
                    this.rdoCet = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 185 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.rdoCet).Checked += this.rdoOption_OnChecked;
                    #line default
                }
                break;
            case 14:
                {
                    this.MainFrame = (global::Windows.UI.Xaml.Controls.Frame)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

