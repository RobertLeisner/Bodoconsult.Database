// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.WinForms.Interfaces;
// ReSharper disable LocalizableElement

namespace DatabaseCleanupTool
{
    public sealed partial class Form1 : Form
    {
        private readonly IMainWindowViewModel _mainWindowViewModel;

        public Form1(IMainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            InitializeComponent();

            Text = $"{_mainWindowViewModel.AppBuilder.AppGlobals.AppStartParameter.AppName} {_mainWindowViewModel.AppVersion}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Do something here 1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Do something here 2");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Implement this event to shutdown your application correctly as required by your demands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _mainWindowViewModel.ShutDown();
        }
    }
}
