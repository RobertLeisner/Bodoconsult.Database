// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Logging;
using Bodoconsult.App.WinForms.AppStarter.Forms.ViewModel;

namespace DatabaseCleanupTool.App;

/// <summary>
/// ViewModel for alternative main window Form1
/// </summary>
public class Forms1MainWindowViewModel : MainWindowViewModel
{
    public Forms1MainWindowViewModel(AppEventListener listener) : 
        base(listener)
    {
    }


    /// <summary>
    /// Create the main form of the application
    /// </summary>
    /// <returns></returns>
    public override Form CreateForm()
    {
        return new Form1(this)
        {
            Visible = true,
            WindowState = FormWindowState.Normal
        };
    }
}