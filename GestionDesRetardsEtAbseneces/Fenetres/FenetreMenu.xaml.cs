using GestionDesRetardsEtAbseneces.Controllers;
using GestionDesRetardsEtAbseneces.Model;
using System.Windows;
using System.Windows.Threading;

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
            UpdateFooterTime();
            LabelBienvenu.Content = " 💛 Bienvenue " + SessionUtilisateur.Nom + " " + SessionUtilisateur.Prenom +" ! 💛";
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
                Btn_Absence.IsEnabled = false;
              

            }
        }
        


        private void Btn_Employes_Click(object sender, RoutedEventArgs e)
        {
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

        private void Btn_Retard_Click(object sender, RoutedEventArgs e)
        {
            fenetreRetards fenetreRetards = new fenetreRetards();
            fenetreRetards.ShowDialog();
        }

        private void Btn_DemandeConges_Click(object sender, RoutedEventArgs e)
        {
            FenetreConges fenetreConges = new();
            fenetreConges.Show();
        }

        private void Btn_Absence_Click(object sender, RoutedEventArgs e)
        {
            FenetreAbsence fenetreAbsence = new FenetreAbsence();   
            fenetreAbsence.Show();
        }

        private void Btn_ValidationConges_Click(object sender, RoutedEventArgs e)
        {

            ValidationConges validationConges = new ValidationConges();
            validationConges.Show();
        }
        private void UpdateFooterTime()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, args) => FooterText.Text = "© 2025 - Heure actuelle : " + DateTime.Now.ToString("HH:mm:ss");
            timer.Start();
        }

        private void Btn_Rapports_Click(object sender, RoutedEventArgs e)
        {
            FenetreRapport fenetreRapport = new FenetreRapport();  
            fenetreRapport.Show();

        }
    }
}
