using System;
using System.Collections.Generic;
using System.IO;
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

namespace Exam.MVVM.ModelView
{
    /// <summary>
    /// Interaction logic for PassTestView.xaml
    /// </summary>
    public partial class PassTestView : UserControl
    {
        long _maxMark = 0;
        long _currentMark = 0;
        List<Questions> _questions;

        public PassTestView()
        {
            InitializeComponent();

            using (ExamDatabase ed = new ExamDatabase())
            {
                foreach (var test in ed.TestsInfo)
                {
                    TestsTitle.Items.Add(test.Title.ToString());
                }
            }
        }

        private void TestsTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TestTitleTb.Text = TestsTitle.SelectedItem.ToString();
        }

        private void StartTestBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TestTitleTb.Text))
            {
                MessageBox.Show("Enter test", "Incorrect test name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TestsTitle.Visibility = Visibility.Hidden;
            TestTitleTb.IsEnabled = false;
            StartTestBtn.Visibility = Visibility.Hidden;

            FillCheckBoxByVariants();

            QuestionTb.Text = _questions[0].Question;

            try
            {
                QuestionImage.Source = ConvertByteToPicture(_questions[0].Image);
            }
            catch  { }

        }

        private void FillCheckBoxByVariants()
        {
            using (ExamDatabase ed = new ExamDatabase())
            {
                List<AnswerVariants> variants = ed.AnswerVariants.Where(x => x.TestId == ed.TestsInfo.Where(ti => ti.Title == TestTitleTb.Text).FirstOrDefault().Id).ToList();

                _questions = ed.Questions.Where(x => x.TestId == ed.TestsInfo.Where(ti => ti.Title == TestTitleTb.Text).FirstOrDefault().Id).ToList();
                int i = 0;
                foreach (var checkBox in StackPanelWithCB.Children.OfType<CheckBox>())
                {
                    if (variants[i] != null)
                    {
                        checkBox.Content = variants[i].Variant;
                    }
                    else
                    {
                        checkBox.IsEnabled = false;
                    }
                }
            }
        }

        private BitmapImage ConvertByteToPicture(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
