using GestionDesRetardsEtAbseneces.Controllers;
using GestionDesRetardsEtAbseneces.Model;
using System.Windows;
using System.Windows.Controls;

namespace GestionDesRetardsEtAbseneces.Fenetres
{
    /// <summary>
    /// Logique d'interaction pour fenetreRetards.xaml
    /// </summary>
    public partial class fenetreRetards : Window
    {
        private DbGestgrhContext _context;

        public fenetreRetards()
        {
            InitializeComponent();
            // Connexion à la base de données
            _context = new DbGestgrhContext(); 
            ChargerEmployes();
            if (Utilitaires.timerInactivite.IsEnabled)
            {
                Utilitaires.timerInactivite.Stop();
            }
            this.MouseMove += ResetInactivityTimer;
            this.KeyDown += ResetInactivityTimer;

        }
        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            Utilitaires.InitialiserTimer(this);
        }
        private void ChargerEmployes()
        {
            cmbEmploye.ItemsSource = _context.Employes.ToList();
            cmbEmploye.DisplayMemberPath = "Nom";
            cmbEmploye.SelectedValuePath = "IdEmploye";
            DataGrid_Retard.ItemsSource=_context.Retards.ToList();
        }

     
        /// Ajoute un nouveau retard à la base de données
      
        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEmploye.SelectedValue == null || string.IsNullOrWhiteSpace(txtHeureDebut.Text) || string.IsNullOrWhiteSpace(txtHeureFin.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs requis.");
                return;
            }

            var retard = new Retard
            {
                IdEmploye = (int)cmbEmploye.SelectedValue,
                DateRetard = dpDateRetard.SelectedDate,
                HeureDebut = TimeSpan.Parse(txtHeureDebut.Text),
                HeureFin = TimeSpan.Parse(txtHeureFin.Text),
                Justification = txtJustification.Text,
                Valide = chkValide.IsChecked ?? false
            };

            _context.Retards.Add(retard);
            _context.SaveChanges();
            MessageBox.Show("Retard ajouté avec succès !");
            ViderChamps();
        }

          /// Modifie un retard existant dans la base de données
        
        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEmploye.SelectedValue == null || dpDateRetard.SelectedDate == null)
            {
                MessageBox.Show("Veuillez sélectionner un employé et une date.");
                return;
            }

            var retard = _context.Retards.FirstOrDefault(r => r.IdEmploye == (int)cmbEmploye.SelectedValue && r.DateRetard == dpDateRetard.SelectedDate);
            if (retard == null)
            {
                MessageBox.Show("Aucun retard trouvé pour cet employé à cette date.");
                return;
            }

            retard.HeureDebut = TimeSpan.Parse(txtHeureDebut.Text);
            retard.HeureFin = TimeSpan.Parse(txtHeureFin.Text);
            retard.Justification = txtJustification.Text;
            retard.Valide = chkValide.IsChecked ?? false;

            _context.SaveChanges();
            MessageBox.Show("Retard modifié avec succès !");
            ViderChamps();
        }

        
        /// Suppression d'un retard de la base de données
        
        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEmploye.SelectedValue == null || dpDateRetard.SelectedDate == null)
            {
                MessageBox.Show("Veuillez sélectionner un employé et une date.");
                return;
            }

            var retard = _context.Retards.FirstOrDefault(r => r.IdEmploye == (int)cmbEmploye.SelectedValue && r.DateRetard == dpDateRetard.SelectedDate);
            if (retard == null)
            {
                MessageBox.Show("Aucun retard trouvé pour cet employé à cette date.");
                return;
            }

            _context.Retards.Remove(retard);
            _context.SaveChanges();
            MessageBox.Show("Retard supprimé avec succès !");
            ViderChamps();
        }

        private void DataGrid_Retard_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            if (DataGrid_Retard.SelectedItem is Retard retard)
            {
                cmbEmploye.SelectedValue = retard.IdEmploye;
                dpDateRetard.SelectedDate = retard.DateRetard;
                txtHeureDebut.Text=retard.HeureDebut.ToString();
                txtHeureFin.Text=retard.HeureFin.ToString();
                txtJustification.Text = retard.Justification;
            }
            else
            {
                ViderChamps();
                return;
            }
        }

        //Vider les champs de saisies
        public void ViderChamps()
        {
            cmbEmploye.SelectedValue = null;
            dpDateRetard.SelectedDate = null;
            txtHeureDebut.Clear();
            txtHeureFin.Clear();
            txtJustification.Clear();
            chkValide.IsChecked = false;
        }

        private void Btn_Actualiser_Click(object sender, RoutedEventArgs e)
        {
            ViderChamps();
        }
    }
}
