using GestionDesRetardsEtAbseneces.Controllers;
using GestionDesRetardsEtAbseneces.Model;
using System.Windows;

namespace GestionDesRetardsEtAbseneces.Fenetres
{
    /// <summary>
    /// Interaction logic for FenetreMenu.xaml
    /// </summary>
    public partial class FenetreMenu : Window
    {
        Utilitaires utilitaires = new();
        public FenetreMenu()
        {
            InitializeComponent();
            LabelBienvenu.Content = "Bienvenue " + SessionUtilisateur.Nom + " " + SessionUtilisateur.Prenom;
            VerifierAcces();
            if (Utilitaires.timerInactivite.IsEnabled)
            {
                Utilitaires.timerInactivite.Stop();
            }
            Utilitaires.InitialiserTimer(this);
        }
        /// <summary>
        /// Ouvre la fenetre de gestion des rapports
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        //Verifier les acces aux boutton
        public void VerifierAcces()
        {
            if (SessionUtilisateur.Role == "Employé")
            {
                Btn_Employes.IsEnabled = false;
                Btn_Rapports.IsEnabled = false;
                Btn_Dashboard.IsEnabled = false;
                Btn_Notifications.IsEnabled = false;
                Btn_Retard.IsEnabled = false;
                Btn_ValidationConges.IsEnabled = false;

            }
        }
        private void Btn_Rapports_Click(object sender, RoutedEventArgs e)
        {
            FenetreRapport fenetreRapport = new FenetreRapport();
            fenetreRapport.Show();
        }


        private void Btn_Employes_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            FenetreEmploye fenetreEmploye = new FenetreEmploye();
            fenetreEmploye.ShowDialog();

            this.Show();

        }

        private void Btn_Notifications_Click(object sender, RoutedEventArgs e)
        {
            FenetreNotification fenetreNotification = new FenetreNotification();
            fenetreNotification.ShowDialog();
            this.Show();
        }

        private void Btn_NotificationsEmploye_Click(object sender, RoutedEventArgs e)
        {
            
            utilitaires.AfficherNotifications(SessionUtilisateur.IdEmploye);
        }

        private void Btn_Deconnexion_Click(object sender, RoutedEventArgs e)
        {
            Utilitaires.Deconnexion(this);
        }

        private void Btn_Dashboard_Click(object sender, RoutedEventArgs e)
        {
            FenetreDashboard fenetreDashboard = new FenetreDashboard();
            fenetreDashboard.Show();
        }

        private void Btn_Absence_Click(object sender, RoutedEventArgs e)
        {
            FenetreAbsence fenetreAbsence = new FenetreAbsence();
            fenetreAbsence.Show();
        }
    }
}
