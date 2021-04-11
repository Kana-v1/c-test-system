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
    public partial class StartMenu : Window
    {

        private int _attempts = 0;
        private string _testTitle = string.Empty;
        private int _newTestId;

        private UserAuthentication _ua;
        public StartMenu()
        {
            InitializeComponent();
        }

        public StartMenu(UserAuthentication ua) 
        {
            InitializeComponent();

            _ua = ua;

            if (_ua._userType == UserType.User)
            {
                NewTestRb.IsEnabled = false;
            }
            
        }

        public void NewTestRb_Checked (object sender, RoutedEventArgs e)
        {
            OneQRb.Visibility = Visibility.Visible;
            ManyQRb.Visibility = Visibility.Visible;

           
        }

        public void LogOut_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)//new test
        {

            if (!Int32.TryParse(TestAttempts.Text, out _attempts) || _attempts <= 0 || _attempts > 100)
            {
                MessageBox.Show("Incorrect attempts");
                return;
            }

            string _testTitle = TestNameTb.Text;

            NewTestBtn.Visibility = Visibility.Hidden;
            TestAttempts.Visibility = Visibility.Hidden;
            TestNameTb.Visibility = Visibility.Hidden;


            ManyQRb.Visibility = Visibility.Visible;            
            OneQRb.Visibility = Visibility.Visible;

            using (ExamDatabase ed = new ExamDatabase())
            {
                TestsInfo ti = new TestsInfo() { Title = _testTitle, Attempts = _attempts };
                ed.TestsInfo.Add(ti);
                ed.SaveChanges();

                _newTestId = ed.TestsInfo.FirstOrDefault(x => x.Title == _testTitle).Id;                
            }
           
        }

       

        #region move answers to checkbox dynamically
        private void FirstAnswerVariant_TextChanged(object sender, TextChangedEventArgs e)
        {
            FirstVariantInChB.Content = FirstAnswerVariant.Text;
        }

        private void SecondAnswerVariant_TextChanged(object sender, TextChangedEventArgs e)
        {
            SecondVariantInChB.Content = SecondAnswerVariant.Text;
        }

        private void ThirdAnswerVariant_TextChanged(object sender, TextChangedEventArgs e)
        {
            ThirdVariantInChB.Content = ThirdAnswerVariant.Text;
        }

        private void FourthAnswerVariant_TextChanged(object sender, TextChangedEventArgs e)
        {
            FourthVariantInChB.Content = FourthAnswerVariant.Text;
        }

        private void FifthAnswerVariant_TextChanged(object sender, TextChangedEventArgs e)
        {
            FifthVariantInChB.Content = FifthAnswerVariant.Text;
        }
        #endregion

        private void NewQuestionBtn(object sender, RoutedEventArgs e)
        {
            using (ExamDatabase ed = new ExamDatabase())
            {

                Questions question = new Questions() { Question = QuestionTb.Text, TestId = _newTestId };
                ed.Questions.Add(question);
                ed.SaveChanges();

                List<AnswerVariants> avList = new List<AnswerVariants>();

                foreach (var tb in StackPanelWithTb.Children.OfType<TextBox>())
                {
                    if (!String.IsNullOrEmpty(tb.Text))
                        avList.Add(new AnswerVariants() { Variant = tb.Text });
                }

                int i = 0;
                foreach (var tb in StackPanelWithCB.Children.OfType<CheckBox>().Where(x => !String.IsNullOrEmpty(x.Content.ToString())))
                {
                    avList[i].IsAnswer = (bool)tb.IsChecked;
                    avList[i].TestId = _newTestId;
                    i++;
                }

               foreach (var answers in avList)
                {
                    ed.AnswerVariants.Add(answers);
                    ed.SaveChanges();
                }
            }
        }
    }
}
