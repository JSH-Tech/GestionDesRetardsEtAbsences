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
    /// Logique d'interaction pour ValidationConges.xaml
    /// </summary>
    public partial class ValidationConges : Window
    {
        private DbGestgrhContext _context;
        public ValidationConges()
        {
            InitializeComponent();
            _context = new DbGestgrhContext();
            ChargerDemandes();
        }
        private void ChargerDemandes()
        {
            var demandes = _context.Demandeconges
                .Join(_context.Employes,
                    d => d.IdEmploye,
                    e => e.IdEmploye,
                    (d, e) => new
                    {
                        d.IdDemande,
                        d.TypeConge,
                        d.DateDebut,
                        d.DateFin,
                        d.Justification,
                        d.Statut,
                        e.Nom,
                        e.Prenom
                    })
                .ToList();

            DemandeCongeDataGrid.ItemsSource = demandes;
        }
        private void Btn_Approuver_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var idDemande = (int)button.Tag;

            // Demander confirmation avant d'approuver
            MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment approuver cette demande de congé ?",
                                                      "Confirmer", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var demande = _context.Demandeconges.FirstOrDefault(d => d.IdDemande == idDemande);

                if (demande != null)
                {
                    demande.Statut = "Approuvé";
                    _context.SaveChanges();
                    ChargerDemandes(); // Mettre à jour l'affichage

                    // Désactiver les boutons après l'approbation
                    DesactiverBoutons(demande.Statut);

                    MessageBox.Show("La demande a été approuvée.");
                }
            }
        }

        // Refuser une demande de congé
        private void Btn_Refuser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var idDemande = (int)button.Tag;

            // Demander confirmation avant de refuser
            MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment refuser cette demande de congé ?",
                                                      "Confirmer", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var demande = _context.Demandeconges.FirstOrDefault(d => d.IdDemande == idDemande);

                if (demande != null)
                {
                    demande.Statut = "Refusé";
                    _context.SaveChanges();
                    ChargerDemandes(); // Mettre à jour l'affichage

                    // Désactiver les boutons après le refus
                    DesactiverBoutons(demande.Statut);

                    MessageBox.Show("La demande a été refusée.");
                }
            }
        }

        // Désactiver les boutons "Approuver" et "Refuser"
        private void DesactiverBoutons(string statut)
        {
            // Vérifiez si le statut est "Approuvé" ou "Refusé"
            if (statut == "Approuvé" || statut == "Refusé")
            {
                // Récupérer les boutons en fonction du statut
                foreach (var item in DemandeCongeDataGrid.Items)
                {
                    var dataGridRow = DemandeCongeDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (dataGridRow != null)
                    {
                        // Récupérer le StackPanel qui contient les boutons "Approuver" et "Refuser"
                        var stackPanel = VisualTreeHelper.GetChild(dataGridRow, 0) as StackPanel;
                        foreach (var btn in stackPanel.Children.OfType<Button>())
                        {
                            btn.IsEnabled = false;  // Désactiver les boutons
                        }
                    }
                }
            }

        }

    }
}
