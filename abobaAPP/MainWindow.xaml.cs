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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace abobaAPP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            loginBox.Text = "loginDEabf2018";
            passwordBox.Text = "*Tasm+";
        }

        private int countMisses = 0;
        private bool captchaAccept = true;

        private void captchaGenerator()
        {
            captchaTextBlock.Text = "";
            for (int i = 0; i < 4; i++)
            {
                Random rnd = new Random();
                int value = rnd.Next(97, 123);
                char symbol = Convert.ToChar(value);
                captchaTextBlock.Text += symbol;
            }
        }

        private void checkMisses()
        {
            if (countMisses >= 1)
            {
                stackPanel.Height = 350;
                mainWindow.Height = 450;
                captchaGenerator();
                captchaTextBlock.Visibility = System.Windows.Visibility.Visible;
                captchaTextBox.Visibility = System.Windows.Visibility.Visible;
                captchaButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new user25Entities())
            {
                if (db.User.Any(u => u.UserLogin == loginBox.Text && u.UserPassword == passwordBox.Text))
                {
                    if (!captchaAccept)
                    {
                        MessageBox.Show("Вы не подтвердили каптчу");
                        return;
                    }
                    User user = (from u in db.User where u.UserLogin == loginBox.Text select u).FirstOrDefault();
                    SystemContext.user = user;
                    if (user.RoleID == 1)
                    {
                        SystemContext.isGuest = false;
                        ClientWindow clientWindow = new ClientWindow();
                        this.Close();
                        clientWindow.ShowDialog();
                    }
                    if (user.RoleID == 2)
                    {
                        SystemContext.isGuest = false;
                        AdminWindow adminWindow = new AdminWindow();
                        this.Close();
                        adminWindow.ShowDialog();
                    }
                    SystemContext.isGuest = false;
                    ManagerWindow managerWindow = new ManagerWindow();
                    this.Close();
                    managerWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Неправильно введен логин или пароль!");
                    countMisses += 1;
                    captchaAccept = false;
                    checkMisses();
                }
            }
        }

        private void guestLoginButton_Click(object sender, RoutedEventArgs e)
        {
            SystemContext.isGuest = true;
            ClientWindow clientWindow = new ClientWindow();
            this.Close();
            clientWindow.ShowDialog();
        }

        private void captchaButton_Click(object sender, RoutedEventArgs e)
        {
            if (captchaTextBlock.Text == captchaTextBox.Text)
            {
                captchaAccept = true;
            }
        }
    }
}
