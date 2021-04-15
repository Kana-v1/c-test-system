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
        private long _maxMark = 0;
        private double _currentMark = 0;
        private List<Questions> _questions; //LINQ can't recognize _question[num] 
        private Questions _question;
        private int _countForQuestion = 0;
        private int _trueVariantsCount = 0;
        private int _userId = 0;
        private int _testId = 0;

        private string _user;

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }


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

            foreach (var cb in StackPanelWithCB.Children.OfType<CheckBox>())
            {
                cb.IsEnabled = false;
            }

        }

        private void TestsTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TestTitleTb.Text = TestsTitle.SelectedItem.ToString();
        }

        private async void StartTestBtn_Click(object sender, RoutedEventArgs e)
        {
            TestTitleTb.IsEnabled = false;

            using (ExamDatabase db = new ExamDatabase())
            {
                int _userId = db.Users.Where(x => x.Login.Equals(_user)).FirstOrDefault().Id;
                int _testId = db.TestsInfo.Where(x => x.Title.Equals(TestTitleTb.Text)).FirstOrDefault().Id;

                int attemptsLeft;
                try
                {
                    attemptsLeft = (int)db.Results.Where(x => x.UserId == _userId).ToList().Where(x => x.TestId == _testId).First().Attemptsleft;
                }
                catch
                {
                    attemptsLeft = db.TestsInfo.Where(x => x.Id == _testId).FirstOrDefault().Attempts;
                }

                MessageBox.Show($"You have {attemptsLeft} attempts", "Attempts", MessageBoxButton.OK, MessageBoxImage.Information);
                if (attemptsLeft == 0)
                    return;
            }

            string testTitle = TestTitleTb.Text;

            if (String.IsNullOrEmpty(testTitle))
            {
                MessageBox.Show("Choose the test", "Incorrect test name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TestsTitle.Visibility = Visibility.Hidden;
            TestTitleTb.IsEnabled = false;
            StartTestBtn.Visibility = Visibility.Hidden;
            QuestionTb.Visibility = Visibility.Visible;
            FinishTestBtn.Visibility = Visibility.Visible;

            _question = new Questions();
            await Task.Run(() => MaxMarkCalculating(testTitle));
            FillCheckBoxByVariants();


            foreach (var cb in StackPanelWithCB.Children.OfType<CheckBox>())
            {
                cb.Checked += CheckAnswerForMultipleVariants;
            }

            if (_questions.Count == 1)
            {
                NextQuestionBtn.Visibility = Visibility.Hidden;
            }

        }

        private void CheckAnswerForMultipleVariants(object sender, RoutedEventArgs e)
        {
            if (_trueVariantsCount > 1)
            {
                return;
            }

            foreach (var cb in StackPanelWithCB.Children.OfType<CheckBox>())
            {
                if (cb != sender && cb.IsChecked == true)
                {
                    cb.IsChecked = false;
                }
            }

        }

        private void MaxMarkCalculating(string testTitle)
        {
            using (ExamDatabase ed = new ExamDatabase())
            {
                TestsInfo test = ed.TestsInfo.Where(x => x.Title.Equals(testTitle)).FirstOrDefault();

                _questions = ed.Questions.Where(x => x.TestId == test.Id).ToList().OrderBy(x => Guid.NewGuid()).ToList();

                foreach (var question in _questions)
                {
                    _maxMark += question.Weight;
                }
            }
        }

        private void FillCheckBoxByVariants()
        {
            using (ExamDatabase ed = new ExamDatabase())
            {

                _question = _questions[_countForQuestion];
                QuestionTb.Text = _questions[_countForQuestion].Question;

                List<AnswerVariants> variants = ed.AnswerVariants.Where(x => x.QuestionId == _question.Id).ToList();

                int i = 0;
                foreach (var checkBox in StackPanelWithCB.Children.OfType<CheckBox>())
                {
                    try
                    {
                        checkBox.Content = variants[i++].Variant;
                        checkBox.IsEnabled = true;
                    }
                    catch
                    {
                        break;
                    }

                }

                foreach (var variant in variants)
                {
                    if (variant.IsAnswer == true)
                        _trueVariantsCount++;
                }
                try
                {
                    QuestionImage.Source = ConvertByteToPicture(_questions[_countForQuestion++].Image);
                    QuestionImage.Visibility = Visibility.Visible;
                }
                catch 
                {
                    QuestionImage.Visibility = Visibility.Hidden;
                }



                QuestiobVariantsInfoTb.Visibility = Visibility.Visible;
                QuestiobVariantsInfoTb.IsEnabled = false;

                if (_trueVariantsCount > 1)
                {
                    QuestiobVariantsInfoTb.Text = "Multiple answers";
                }
                else
                {
                    QuestiobVariantsInfoTb.Text = "One answer";
                }

            }


        }

        private BitmapImage ConvertByteToPicture(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        private void NextQuestionBtn_Click(object sender, RoutedEventArgs e)
        {
            CalculateCurrentMark();
            FillCheckBoxByVariants();

            if (_countForQuestion == _questions.Count)
            {
                NextQuestionBtn.Visibility = Visibility.Hidden;
            }

            foreach (var cb in StackPanelWithCB.Children.OfType<CheckBox>())
            {
                cb.IsChecked = false;
            }



        }

        private void CalculateCurrentMark()
        {

            using (ExamDatabase ed = new ExamDatabase())
            {
                int answerCount = ed.AnswerVariants.Where(x => x.IsAnswer == true).ToList().Where(x => x.QuestionId == _question.Id).ToList().Count;

                double oneRightAnswerWeight = (double)_question.Weight / answerCount;
                bool answerIsTrue;
                foreach (var cb in StackPanelWithCB.Children.OfType<CheckBox>())
                {
                    try
                    {
                        answerIsTrue = ed.AnswerVariants.Where(x => x.Variant.Equals(cb.Content.ToString())).ToList().Where(x => x.QuestionId == _question.Id).First().IsAnswer;
                    }
                    catch { return; }

                    if (cb.IsChecked == true &&
                        (answerIsTrue == true))
                    {
                        _currentMark += oneRightAnswerWeight;
                    }
                }
            }
        }

        private void FinishTestBtn_Click(object sender, RoutedEventArgs e)
        {
            CalculateCurrentMark();
            using (ExamDatabase db = new ExamDatabase())
            {
                int mark = (int)_currentMark;


                //both of these variables are 0 when this method perform, conundrum for me 
                _testId = db.TestsInfo.Where(x => x.Title.Equals(TestTitleTb.Text)).FirstOrDefault().Id;
                _userId = db.Users.Where(x => x.Login.Equals(_user)).FirstOrDefault().Id;

                //if this user hasn't already passed this test
                int lastMark = -1;

                if (db.Results.Where(x => x.UserId == _userId).FirstOrDefault() != null)
                {
                    if (db.Results.Where(x => x.UserId == _userId).ToList().Where(x => x.TestId == _testId).FirstOrDefault() != null)
                        lastMark = db.Results.Where(x => x.UserId == _userId).ToList().Where(x => x.TestId == _testId).FirstOrDefault().Mark;
                }

                if (lastMark == -1)
                {
                    int attemptsLeft = db.TestsInfo.Where(x => x.Id == _testId).FirstOrDefault().Attempts;

                    Results result = new Results() { UserId = _userId, TestId = _testId, Mark = mark, Attemptsleft = attemptsLeft };
                    db.Results.Add(result);
                    db.SaveChanges();
                }

                else
                {
                    int attemptsLeft = (int)db.Results.Where(x => x.UserId == _userId).ToList().Where(x => x.TestId == _testId).First().Attemptsleft;
                    db.Results.Where(x => x.UserId == _userId).ToList().Where(x => x.TestId == _testId).First().Attemptsleft = --attemptsLeft;

                    if (mark > lastMark)
                    {
                        db.Results.Where(x => x.UserId == _userId).ToList().Where(x => x.TestId == _testId).FirstOrDefault().Mark = mark;
                    }
                    db.SaveChanges();
                }

                MessageBox.Show($"U have passed test with mark = {mark}  ({String.Format("{0:0.0}", (double)mark / _maxMark * 100)} %)", "Result", MessageBoxButton.OK, MessageBoxImage.Information);

                TestsTitle.Visibility = Visibility.Visible;
                FinishTestBtn.Visibility = Visibility.Hidden;

                foreach (var checkBox in StackPanelWithCB.Children.OfType<CheckBox>())
                {
                    checkBox.Content = String.Empty;
                    checkBox.IsEnabled = false;
                    checkBox.IsChecked = false;
                }
                QuestionTb.Visibility = Visibility.Hidden;
                StartTestBtn.Visibility = Visibility.Visible;
                QuestiobVariantsInfoTb.Visibility = Visibility.Hidden;
                QuestionImage.Visibility = Visibility.Hidden;
                _countForQuestion = 0;



            }

        }
    }
}
