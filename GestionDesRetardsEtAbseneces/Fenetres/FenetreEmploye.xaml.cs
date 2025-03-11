using GestionDesRetardsEtAbseneces.Controllers;
using GestionDesRetardsEtAbseneces.Model;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace GestionDesRetardsEtAbseneces.Fenetres
{
    /// <summary>
    /// Logique d'interaction pour FenetreEmploye.xaml
    /// </summary>
    public partial class FenetreEmploye : Window
    {
        private DbGestgrhContext _context;
        private ObservableCollection<Employe> _employes;
        private Employe _selectedEmploye;
        public FenetreEmploye()
        {

            InitializeComponent();

            _context = new DbGestgrhContext();
            _employes = new ObservableCollection<Employe>(_context.Employes.ToList());
            EmployeDataGrid.ItemsSource = _employes;

            if (Utilitaires.timerInactivite.IsEnabled)
            {
                Utilitaires.timerInactivite.Stop();
            }
            Utilitaires.InitialiserTimer(this);
        }

        private bool ValiderChamps()
        {
            bool estValide = true;

            // Réinitialisation des erreurs (on cache toutes les erreurs)
            txtNomErreur.Visibility = Visibility.Collapsed;
            txtPrenomErreur.Visibility = Visibility.Collapsed;
            txtEmailErreur.Visibility = Visibility.Collapsed;
            txtMotDePasseErreur.Visibility = Visibility.Collapsed;
            txtRoleErreur.Visibility = Visibility.Collapsed;
            txtStatutErreur.Visibility = Visibility.Collapsed;

            // Vérification des champs
            if (string.IsNullOrWhiteSpace(txtNom.Text))
            {
                txtNomErreur.Text = "Le nom est obligatoire.";
                txtNomErreur.Visibility = Visibility.Visible;
                estValide = false;
            }

            if (string.IsNullOrWhiteSpace(txtPrenom.Text))
            {
                txtPrenomErreur.Text = "Le prénom est obligatoire.";
                txtPrenomErreur.Visibility = Visibility.Visible;
                estValide = false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtEmailErreur.Text = "L'email est obligatoire.";
                txtEmailErreur.Visibility = Visibility.Visible;
                estValide = false;
            }
            else if (!EmailEstValide(txtEmail.Text))
            {
                txtEmailErreur.Text = "Format d'email invalide.";
                txtEmailErreur.Visibility = Visibility.Visible;
                estValide = false;
            }

            if (string.IsNullOrWhiteSpace(MotDePasse.Password))
            {
                txtMotDePasseErreur.Text = "Le mot de passe est obligatoire.";
                txtMotDePasseErreur.Visibility = Visibility.Visible;
                estValide = false;
            }

            if (RoleComboBox.SelectedItem == null)
            {
                txtRoleErreur.Text = "Veuillez sélectionner un rôle.";
                txtRoleErreur.Visibility = Visibility.Visible;
                estValide = false;
            }

            if (StatutComboBox.SelectedItem == null)
            {
                txtStatutErreur.Text = "Veuillez sélectionner un statut.";
                txtStatutErreur.Visibility = Visibility.Visible;
                estValide = false;
            }

            return estValide;
        }
        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {

            if (!ValiderChamps())
            {
                return;
            }

            string passwordHasher = Hacheur.HacherMotDePasse(MotDePasse.Password);

            var employe = new Employe
            {
                Nom = txtNom.Text,
                Prenom = txtPrenom.Text,
                Email = txtEmail.Text,
                MotDePasse = passwordHasher,
                RoleEmploye = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Statut = (StatutComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()
            };

            _context.Employes.Add(employe);
            _context.SaveChanges();
            _employes.Add(employe);

            MessageBox.Show("Employé ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            ViderFormulaire();

        }

        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmploye != null)
            {
                _selectedEmploye.Nom = txtNom.Text;
                _selectedEmploye.Prenom = txtPrenom.Text;
                _selectedEmploye.Email = txtEmail.Text;
                _selectedEmploye.MotDePasse = MotDePasse.Password;
                _selectedEmploye.RoleEmploye = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                _selectedEmploye.Statut = (StatutComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                _context.SaveChanges();
                EmployeDataGrid.Items.Refresh();
                MessageBox.Show("Employé modifié avec succès !");
                ViderFormulaire();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un employé à modifier.");
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            ViderFormulaire();
        }

        private void EmployeDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (EmployeDataGrid.SelectedItem is Employe employe)
            {
                _selectedEmploye = employe;
                txtNom.Text = employe.Nom;
                txtPrenom.Text = employe.Prenom;
                txtEmail.Text = employe.Email;
                MotDePasse.Password = employe.MotDePasse;
                RoleComboBox.SelectedItem = RoleComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == employe.RoleEmploye);
                StatutComboBox.SelectedItem = StatutComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == employe.Statut);
                btnModifier.IsEnabled = true;
            }
        }
        private bool EmailEstValide(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private void ViderFormulaire()
        {
            txtNom.Text = "";
            txtPrenom.Text = "";
            txtEmail.Text = "";
            MotDePasse.Password = "";
            RoleComboBox.SelectedIndex = -1;
            StatutComboBox.SelectedIndex = -1;
            btnModifier.IsEnabled = false;
            _selectedEmploye = null;
        }

        //Supprimer un employe apres confirmation de l'utilisateur et aprea voir verifier si une ligne est selectionne dans la datagrid et si la ligne selectionner est un employer
        private void Btn_Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmploye == null)
            {
                MessageBox.Show("Veuillez sélectionner un employé à supprimer.");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment supprimer cet employé ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _context.Employes.Remove(_selectedEmploye);
                _context.SaveChanges();
                _employes.Remove(_selectedEmploye);
                MessageBox.Show("Employé supprimé avec succès !");
                ViderFormulaire();
            }
            else
            {
                ViderFormulaire();
                return;
            }

        }
    }
}
