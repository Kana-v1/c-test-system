using Exam.MVVM.ModelView;
using System;
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
using System.Windows.Shapes;

namespace Exam
{
    /// <summary>
    /// Interaction logic for StartMenu.xaml
    /// </summary>
    /// 

    
    public partial class StartMenu : Window
    {
        

        private string _testTitle = string.Empty;
        PassTestView _ptv;
        AddingTestWindow _atw;

        protected UserAuthentication _ua;

        public UserAuthentication UA { get => _ua; private set => _ua = value; }

        public StartMenu(UserAuthentication ua) 
        {
            InitializeComponent();

            UA = ua;

            if (_ua._userType == UserType.User)
            {
                NewTestRb.IsEnabled = false;
            }

            _ptv = new PassTestView() { User = ua.Name };
            _atw = new AddingTestWindow();
            
            
        }

        

        public void LogOut_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void PassTestRb_Checked(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = _ptv;
        }

        private void NewTestRb_Checked(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = _atw;
        }
    }
}
