using GestionDesRetardsEtAbseneces.Controllers;
using GestionDesRetardsEtAbseneces.Fenetres;
using GestionDesRetardsEtAbseneces.Model;
using System.Windows;

namespace GestionDesRetardsEtAbseneces
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbGestgrhContext dbGestGrhContext = new DbGestgrhContext();
        SessionEtAuthentification sessionEtAuthentification = new SessionEtAuthentification();
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
            Employe? employeConnecter = AuthentifierUtilisateur(username, password);
            if (employeConnecter is not null)
            {
                try
                {

                     sessionEtAuthentification.MettreAJourAuthentification(employeConnecter.IdEmploye);

                    FenetreMenu fenetreMenu = new();
                    fenetreMenu.Show();
                    this.Close(); // Ferme la fenêtre de connexion
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
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

        private Employe? AuthentifierUtilisateur(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            {
                string passwordHasher = Hacheur.HacherMotDePasse(password);
                Employe? employe = dbGestGrhContext.Employes.FirstOrDefault(e => e.Email == username && e.MotDePasse == passwordHasher);

                if (employe is not null)
                {
                    return employe;
                }
                Txt_ErrorMessage.Text = "Identifiant ou mot de passe incorrect.";
                Txt_ErrorMessage.Visibility = Visibility.Visible;
                return null;
            }
            Txt_ErrorMessage.Text = "Veuillez remplir tous les champs";
            Txt_ErrorMessage.Visibility = Visibility.Visible;
            return null;
        }

    }
}