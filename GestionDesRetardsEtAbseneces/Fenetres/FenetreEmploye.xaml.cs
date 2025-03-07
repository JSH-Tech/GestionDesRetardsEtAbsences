using GestionDesRetardsEtAbseneces.Controllers;
using GestionDesRetardsEtAbseneces.Model;
using System.Collections.ObjectModel;
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

            InitializeComponent();
            if (Utilitaires.timerInactivite.IsEnabled)
            {
                Utilitaires.timerInactivite.Stop();
            }
            Utilitaires.InitialiserTimer(this);
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
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

            MessageBox.Show("Employé ajouté avec succès !");
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


    }
}
