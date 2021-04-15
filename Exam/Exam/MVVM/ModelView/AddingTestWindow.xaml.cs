using System;
using System.Collections.Generic;
using static System.Linq.Queryable;
using static System.Linq.Enumerable;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

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
        private Questions _question;

        public AddingTestWindow()
        {
            InitializeComponent();

            foreach (var cb in StackPanelWithCB.Children.OfType<CheckBox>())
            {
                cb.IsEnabled = false;
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)//new test
        {
            using (ExamDatabase ed = new ExamDatabase())
            {
                if (ed.TestsInfo.Where(x => x.Title == TestNameTb.Text).FirstOrDefault() != null)
                {
                    MessageBox.Show("Test's title have to be unique", "Incorrect test's title", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (String.IsNullOrEmpty(TestNameTb.Text))
                {
                    MessageBox.Show("Test's title must not be empty", "Incorrect test's title", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                if (!Int32.TryParse(TestAttempts.Text, out _attempts) || _attempts <= 0 || _attempts > 100)
                {
                    MessageBox.Show("Incorrect attempts");
                    return;
                }

                string _testTitle = TestNameTb.Text;

                NewTestBtn.Visibility = Visibility.Hidden;
                TestAttempts.Visibility = Visibility.Hidden;
                TestNameTb.Visibility = Visibility.Hidden;


                OneQOneAns.Visibility = Visibility.Visible;
                OneQRb.Visibility = Visibility.Visible;
                FinishCreatingTestBtn.Visibility = Visibility.Visible;

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
            FirstVariantInChB.IsEnabled = true;
        }

        private void SecondAnswerVariant_TextChanged(object sender, TextChangedEventArgs e)
        {
            SecondVariantInChB.Content = SecondAnswerVariant.Text;
            SecondVariantInChB.IsEnabled = true;

            if (String.IsNullOrEmpty(SecondVariantInChB.Content.ToString()))
            {
                SecondVariantInChB.IsEnabled = false;
            }
        }

        private void ThirdAnswerVariant_TextChanged(object sender, TextChangedEventArgs e)
        {
            ThirdVariantInChB.Content = ThirdAnswerVariant.Text;
            ThirdVariantInChB.IsEnabled = true;

            if (String.IsNullOrEmpty(ThirdVariantInChB.Content.ToString()))
            {
                ThirdVariantInChB.IsEnabled = false;
            }
        }

        private void FourthAnswerVariant_TextChanged(object sender, TextChangedEventArgs e)
        {
            FourthVariantInChB.Content = FourthAnswerVariant.Text;
            FourthVariantInChB.IsEnabled = true;

            if (String.IsNullOrEmpty(FourthVariantInChB.Content.ToString()))
            {
                FourthVariantInChB.IsEnabled = false;
            }
        }

        private void FifthAnswerVariant_TextChanged(object sender, TextChangedEventArgs e)
        {
            FifthVariantInChB.Content = FifthAnswerVariant.Text;
            FifthVariantInChB.IsEnabled = true;

            if (String.IsNullOrEmpty(FifthVariantInChB.Content.ToString()))
            {
                FifthVariantInChB.IsEnabled = false;
            }
        }
        #endregion



        //REGION
        #region Adding picture
        public void AddPictureBtn_Click(object sendet, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "JPEG|*.jpg", ValidateNames = true, Multiselect = false };
            if (ofd.ShowDialog() == true)
            {
                Uri fileUri = new Uri(ofd.FileName);
                imageForQuestion.Source = new BitmapImage(fileUri);
            }


        }
        private byte[] ConvertPicToByte(BitmapImage img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }

        #endregion
        private void NewQuestionBtn(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(QuestionTb.Text))
            {
                MessageBox.Show("Enter question", "Question", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _question = new Questions();

            using (ExamDatabase ed = new ExamDatabase())
            {


                int weight = 0;

                if (!Int32.TryParse(QuestionWeightTb.Text, out weight))
                {
                    MessageBox.Show("Enter question's weight", "Incorrect question's weight", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //REGION
                #region Question field check

                if (String.IsNullOrEmpty(QuestionTb.Text))
                {
                    MessageBox.Show("Question's field must not be empty", "Incorrect question's field", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                #endregion

                _question.Weight = weight;
                _question.Question = QuestionTb.Text;
                _question.TestId = _newTestId;

                if (!(imageForQuestion.Source is null))
                    _question.Image = ConvertPicToByte(imageForQuestion.Source as BitmapImage); //load image to database

                ed.Questions.Add(_question);

                List<AnswerVariants> avList = new List<AnswerVariants>();

                int answersCount = 0;
                foreach (var tb in StackPanelWithCB.Children.OfType<CheckBox>().Where(x => !String.IsNullOrEmpty(x.Content.ToString())))
                {
                    if (tb.IsChecked == true)
                        answersCount++;

                    avList.Add(new AnswerVariants()
                    {
                        Variant = tb.Content.ToString(),
                        IsAnswer = (bool)tb.IsChecked,
                        QuestionId = _question.Id
                    });
                }

                //REGION
                #region Variants check
                if (answersCount == 0)
                {
                    MessageBox.Show("Enter at least 1 answer", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (OneQOneAns.IsChecked == true && !OnlyOneCheckedInCheckBox())
                {
                    MessageBox.Show("2 answers have chosen as right. If you think its OK - chose \"One question → many answers\" mode", "too many answers", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                for (int i = 0; i < avList.Count - 1; i++)
                {
                    for (int j = i + 1; j < avList.Count; j++)
                    {
                        if (avList[i].Variant.Equals(avList[j].Variant))
                            MessageBox.Show($"U can't have two similar answers ({avList[i].Variant}", "Two similar answers", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                #endregion

                foreach (var answers in avList)
                {
                    ed.AnswerVariants.Add(answers);
                }
                ed.SaveChanges();


                MessageBox.Show("Question has been added", "Question", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);

                MakeAllFieldsEmpty();
            }
        }

        private void MakeAllFieldsEmpty()
        {
            foreach (var tb in StackPanelWithTb.Children.OfType<TextBox>())
            {
                tb.Text = String.Empty;
            }

            imageForQuestion.Source = null;
            QuestionTb.Text = String.Empty;
            QuestionWeightTb.Text = String.Empty;

            foreach (var rb in SPWithRb.Children.OfType<RadioButton>())
            {
                rb.IsChecked = false;
            }

            foreach (var tb in StackPanelWithCB.Children.OfType<CheckBox>())
            {
                tb.IsChecked = false;
            }

        }

        private bool OnlyOneCheckedInCheckBox()
        {
            int count = 0;
            foreach (var tb in StackPanelWithCB.Children.OfType<CheckBox>())
            {
                if (tb.IsChecked == true)
                    count++;
            }

            if (count != 1)
                return false;

            return true;
        }

        private void FinishCreatingTestBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Test{_testTitle} has been added", "Test", MessageBoxButton.OK, MessageBoxImage.Information);
            StartMenu instance = Application.Current.Windows.OfType<StartMenu>().FirstOrDefault();
            StartMenu new_instance = new StartMenu(instance.UA);
            instance.Close();
            new_instance.Show();
        }
    }
}
