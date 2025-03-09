using GestionDesRetardsEtAbseneces.Model;
using Microsoft.EntityFrameworkCore;
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
using System.Windows.Shapes;

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
        }
        private void ChargerEmployes()
        {
            cmbEmploye.ItemsSource = _context.Employes.ToList();
            cmbEmploye.DisplayMemberPath = "Nom";
            cmbEmploye.SelectedValuePath = "IdEmploye";
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
        }
    
}
}
