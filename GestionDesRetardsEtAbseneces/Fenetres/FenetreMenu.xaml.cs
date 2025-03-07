using GestionDesRetardsEtAbseneces.Model;
using System.Windows;

namespace GestionDesRetardsEtAbseneces.Fenetres
{
    /// <summary>
    /// Interaction logic for FenetreMenu.xaml
    /// </summary>
    public partial class FenetreMenu : Window
    {
        public FenetreMenu()
        {
            InitializeComponent();
            LabelBienvenu.Content = "Bienvenue " + SessionUtilisateur.Nom + " " + SessionUtilisateur.Prenom;
        }
        /// <summary>
        /// Ouvre la fenetre de gestion des rapports
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
    }
}
