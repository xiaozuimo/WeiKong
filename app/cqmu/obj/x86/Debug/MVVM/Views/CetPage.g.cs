﻿#pragma checksum "C:\Users\xiaoz\Documents\Visual Studio 2015\Projects\cqmu\cqmu\MVVM\Views\CetPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C7FC85FBD22A32B41CA033EB5CCB4994"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cqmu.MVVM.Views
{
    partial class CetPage : 
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
                    this.svDetail = (global::Windows.UI.Xaml.Controls.ScrollViewer)(target);
                }
                break;
            case 3:
                {
                    this.SignInGrid = (global::Windows.UI.Xaml.Controls.RelativePanel)(target);
                }
                break;
            case 4:
                {
                    this.TipPanel = (global::Windows.UI.Xaml.Controls.RelativePanel)(target);
                }
                break;
            case 5:
                {
                    this.TipRing = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
                }
                break;
            case 6:
                {
                    this.txtCode = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 7:
                {
                    this.txtName = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 8:
                {
                    this.btnConfirm = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 56 "..\..\..\..\MVVM\Views\CetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnConfirm).Click += this.btnConfirm_Click;
                    #line default
                }
                break;
            case 9:
                {
                    this.detailGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
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

