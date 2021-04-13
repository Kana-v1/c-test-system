using System;
using System.Collections.Generic;
using static System.Linq.Queryable;
using static System.Linq.Enumerable;
using System.Windows;
using System.Windows.Controls;

namespace Exam.MVVM.ModelView
{
    /// <summary>
    /// Interaction logic for AddingTestWindow.xaml
    /// </summary>
    public partial class AddingTestWindow : UserControl
    {
        private int _attempts = 0;
        private string _testTitle = string.Empty;
        private int _newTestId;

        public AddingTestWindow()
        {
            InitializeComponent();
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

            using (TestSystemDb ed = new TestSystemDb())
            {
                TestsInfo ti = new TestsInfo() { Title = _testTitle, Attempts = _attempts };
                ed.TestsInfo.Add(ti);
                ed.SaveChanges();

                _newTestId = ed.TestsInfo.FirstOrDefault(x => x.Title == _testTitle).Id;
            }

            AddQuestionBtn.IsEnabled = true;

        }


        //REGION
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
            using (TestSystemDb ed = new TestSystemDb())
            {

                Questions question = new Questions() { Question = QuestionTb.Text, TestId = _newTestId };
                ed.Questions.Add(question);

                List<AnswerVariants> avList = new List<AnswerVariants>();

              
                foreach (var tb in StackPanelWithCB.Children.OfType<CheckBox>().Where(x => !String.IsNullOrEmpty(x.Content.ToString())))
                {
                    avList.Add(new AnswerVariants() { Variant = tb.Content.ToString(),
                        IsAnswer = (bool)tb.IsChecked,
                        TestId = _newTestId
                    });                   
                }

                for (int i = 0; i < avList.Count - 1; i++)
                {
                    for (int j = i + 1; j < avList.Count; j++)
                    {
                        if (avList[i].Variant.Equals(avList[j].Variant))
                            MessageBox.Show($"U can't have two similar answers ({avList[i].Variant}","Two similar answers", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                foreach (var answers in avList)
                {
                    ed.AnswerVariants.Add(answers);
                }
                    ed.SaveChanges();
            }
        }
    }
}
