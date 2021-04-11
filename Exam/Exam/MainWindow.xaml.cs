﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Threading;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Exam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        public UserAuthentication _ua;
        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.SetData("DataDirectory", @"C:\NetworkProg\Exam");

            _ua = new UserAuthentication();

            RegisterBtn.Click += _ua.RegisterBtn_Click;
            LogInBtn.Click += _ua.LogInBtn_Click;

        }
        public void AfterLogging()
        {
            if (_ua.Authenticated)
            {
                StartMenu sm = new StartMenu(_ua);
                sm.Show();
                this.Close();
            }
        }



        public void Dispose()// implemented just for unsubscribing from events
        {
            RegisterBtn.Click -= _ua.RegisterBtn_Click;
            LogInBtn.Click -= _ua.LogInBtn_Click;

            Process.GetCurrentProcess().Kill();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e) //event handler for password field's watermark
        {
            if (PasswordTb.Password.Length > 0)
            {
                PasswordTBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordTBlock.Visibility = Visibility.Visible;
            }
        }
    }



}
