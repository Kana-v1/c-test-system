using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Exam
{

    public enum UserType
    {
        User,
        Administrator
    }
    public class UserAuthentication
    {


        private MainWindow _instance = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        public string Name { get; private set; }
        public string Password { get; private set; }

        public bool IsAdmin { get; private set; }

        public UserType _userType;

        public bool Authenticated { get; private set; } = false;

        public UserAuthentication()
        {
            _userType = UserType.User;
        }

        //REGION
        #region LogIn Button 

        public async void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            _instance.LogInBtn.IsEnabled = false;

            string log = _instance.LogTb.Text;
            string password = _instance.PasswordTb.Password;

            if (await Task.Run(() => CheckUserInfo(log, password)))
            {
                Name = log;
                Password = password;

                if (IsAdmin == true)
                {
                    _userType = UserType.Administrator;
                }

                Authenticated = true;


                _instance.AfterLogging();//continue executing main form
            }
            else
            {
                _instance.LogInBtn.IsEnabled = false;
                MessageBox.Show("Incorrect log or password");
            }

        }
        private bool CheckUserInfo(string checkLogin, string checkPassword)
        {
            bool check = false;

            var provider = new SHA1CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(checkPassword);
            string hashedPasswordFromForm = Convert.ToBase64String(provider.ComputeHash(bytes));

            using (ExamDatabase tsd = new ExamDatabase())
            {
                var user = tsd.Users.FirstOrDefault(u => u.Login == checkLogin);

                bytes = Encoding.UTF8.GetBytes(user.Password);
                string hashedPassworfFromDb = Convert.ToBase64String(provider.ComputeHash(bytes));

                if (user != null && hashedPassworfFromDb == hashedPasswordFromForm)
                {
                    IsAdmin = user.IsAdmin;
                    check = true;
                }
            }

                return check;
        }
        #endregion


        //REGION
        #region Register Button

        public void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {

            //var provider = new SHA1CryptoServiceProvider();
            //byte[] bytes = Encoding.UTF8.GetBytes(_instance.PasswordTb.Password);
            //string hashedPassword = Convert.ToBase64String(provider.ComputeHash(bytes));

            try
            {
                Users user = new Users() { Login = _instance.LogTb.Text, Password = _instance.PasswordTb.Password };
                using (ExamDatabase ud = new ExamDatabase())
                {
                    ud.Users.Add(user);
                    ud.SaveChanges();
                }
                MessageBox.Show("U've been registered");
            }

            catch
            {
                MessageBox.Show("Incorrect log or password");
            }

        }
        #endregion



    }
}
