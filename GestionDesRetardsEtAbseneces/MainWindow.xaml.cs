using GestionDesRetardsEtAbseneces.Fenetres;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestionDesRetardsEtAbseneces
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Fonctionnalité de connexion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = NomUser.Text;
            string password = MotDePasse.Password;

            if (string.IsNullOrEmpty(NomUser.Text) || string.IsNullOrEmpty(MotDePasse.Password))
            {
                Txt_ErrorMessage.Text = "Veuillez remplir tous les champs";
                Txt_ErrorMessage.Visibility = Visibility.Visible;
            }

            else if (username == "admin" && password == "123") // Simule une authentification
            {
                FenetreMenu fenetreMenu = new FenetreMenu();
                fenetreMenu.Show();
                this.Close(); // Ferme la fenêtre de connexion
            }
            else
            {
                Txt_ErrorMessage.Text = "Identifiant ou mot de passe incorrect.";
                Txt_ErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pour réinitialiser votre mot de passe, contactez l'assistance.", "Mot de passe oublié", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Quitte l'application
        }
    }
}