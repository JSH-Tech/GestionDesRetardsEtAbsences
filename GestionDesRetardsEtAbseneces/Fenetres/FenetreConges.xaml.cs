using GestionDesRetardsEtAbseneces.Controllers;
﻿using GestionDesRetardsEtAbseneces.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace GestionDesRetardsEtAbseneces.Fenetres
{
    /// <summary>
    /// Logique d'interaction pour FenetreConges.xaml
    /// </summary>
    public partial class FenetreConges : Window
    {


        
        private DbGestgrhContext _context;
        private ObservableCollection<Demandeconge> _demandes;
        private Demandeconge _selectedDemande;
        private bool _isEditing;
        public FenetreConges()
        {
            InitializeComponent();
            _context = new DbGestgrhContext();
            _demandes = new ObservableCollection<Demandeconge>();
            DemandeCongeDataGrid.ItemsSource = _demandes;
            ChargerDemandes();
            _isEditing = false;

            if (Utilitaires.timerInactivite.IsEnabled)
            {
                Utilitaires.timerInactivite.Stop();
            }
            Utilitaires.InitialiserTimer(this);
        }

        private void Btn_Modifier_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                // Récupérer l'ID de la demande à modifier
                var demandeId = (int)button.Tag;  // ID de la demande dans le Tag du bouton

                // Chercher la demande dans la base de données
                var demandeToModify = _context.Demandeconges.FirstOrDefault(d => d.IdDemande == demandeId);
                if (demandeToModify != null)
                {
                    // Affichage des informations de la demande dans les champs de formulaire
                    myComboBox.SelectedItem = myComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == demandeToModify.TypeConge);
                    DateDebutDatePicker.SelectedDate = demandeToModify.DateDebut;
                    DateFinDatePicker.SelectedDate = demandeToModify.DateFin;
                    JustificatifTextBox.Text = demandeToModify.Justification;

                    // Changer le texte du bouton en "Enregistrer"
                    Btn_Soumettre.Content = "Enregistrer";

                    // Définir une propriété pour savoir si nous sommes en mode "Modification"
                    // Par exemple, tu peux stocker l'ID de la demande en cours de modification.
                    Btn_Soumettre.Tag = demandeToModify.IdDemande;
                }
            }
        }

        private void Btn_Soumettre_Click(object sender, RoutedEventArgs e)
        {
            if (Btn_Soumettre.Content.ToString() == "Enregistrer")
            {
                // Enregistrer les modifications de la demande
                var demandeId = (int)Btn_Soumettre.Tag;  // Récupérer l'ID de la demande à modifier
                var demandeToModify = _context.Demandeconges.FirstOrDefault(d => d.IdDemande == demandeId);

                if (demandeToModify != null)
                {
                    // Mettre à jour les champs de la demande
                    demandeToModify.TypeConge = (myComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    demandeToModify.DateDebut = DateDebutDatePicker.SelectedDate;
                    demandeToModify.DateFin = DateFinDatePicker.SelectedDate;
                    demandeToModify.Justification = JustificatifTextBox.Text;

                    // Sauvegarder les changements dans la base de données
                    _context.SaveChanges();

                    // Maintenant on met à jour la ObservableCollection de manière manuelle
                    var demandeIndex = _demandes.IndexOf(_demandes.First(d => d.IdDemande == demandeId));
                    if (demandeIndex >= 0)
                    {
                        // Remplacer l'élément modifié par le nouvel objet
                        _demandes[demandeIndex] = demandeToModify;  // Mise à jour de l'élément modifié dans la collection
                    }

                    // Rafraîchir le DataGrid en réaffectant l'ItemsSource
                    DemandeCongeDataGrid.ItemsSource = null;
                    DemandeCongeDataGrid.ItemsSource = _demandes;

                    MessageBox.Show("Demande de congé modifiée avec succès !");
                    ViderFormulaire();
                    Btn_Soumettre.Content = "Soumettre"; // Restaurer le texte du bouton
                }
            }
            else
            {
                // Soumettre une nouvelle demande de congé (ancienne fonctionnalité)
                if (DateDebutDatePicker.SelectedDate == null || DateFinDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Veuillez sélectionner une date de début et de fin.");
                    return;
                }

                var demande = new Demandeconge
                {
                    IdEmploye = SessionUtilisateur.IdEmploye,
                    TypeConge = (myComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    DateDebut = DateDebutDatePicker.SelectedDate,
                    DateFin = DateFinDatePicker.SelectedDate,
                    Justification = JustificatifTextBox.Text,
                    Statut = "En attente" // Statut par défaut
                };

                _context.Demandeconges.Add(demande);
                _context.SaveChanges();
                _demandes.Add(demande);

                MessageBox.Show("Demande de congé soumise avec succès !");
                ViderFormulaire();
            }
        }


        private void ChargerDemandes()
        {
            var idEmployeConnecte = SessionUtilisateur.IdEmploye; // Obtenir l'ID de l'employé connecté
            var demandes = _context.Demandeconges
                .Where(d => d.IdEmploye == idEmployeConnecte) // Filtrer par l'ID de l'employé connecté
                .ToList();

            // Actualiser la liste des demandes
            _demandes.Clear();
            foreach (var demande in demandes)
            {
                _demandes.Add(demande);
            }
        }
       





       







        // Fonction pour gérer la suppression d'une demande

        private void Btn_Supprimer_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                // Récupérer l'ID de la demande de congé à supprimer
                var demandeId = (int)button.Tag; // ID de la demande dans le Tag du bouton

                // Afficher une boîte de dialogue de confirmation
                var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cette demande de congé ?",
                                             "Confirmation de suppression",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                // Si l'utilisateur clique sur "Oui"
                if (result == MessageBoxResult.Yes)
                {
                    // Trouver la demande de congé dans la base de données
                    var demandeToDelete = _context.Demandeconges.FirstOrDefault(d => d.IdDemande == demandeId);
                    if (demandeToDelete != null)
                    {
                        // Supprimer la demande de la base de données
                        _context.Demandeconges.Remove(demandeToDelete);
                        _context.SaveChanges();

                        // Supprimer la demande de la liste ObservableCollection
                        _demandes.Remove(demandeToDelete);

                        MessageBox.Show("Demande de congé supprimée avec succès !");
                    }
                    else
                    {
                        MessageBox.Show("La demande de congé n'a pas été trouvée.");
                    }
                }
                else
                {
                    // Si l'utilisateur clique sur "Non", rien n'est fait, la demande ne sera pas supprimée.
                    MessageBox.Show("Suppression annulée.");
                }
            }
        }



        private void ViderFormulaire()
        {
            myComboBox.SelectedIndex = -1;
            DateDebutDatePicker.SelectedDate = null;
            DateFinDatePicker.SelectedDate = null;
            JustificatifTextBox.Clear();
        }
        private void Btn_Annuler_Click(object sender, RoutedEventArgs e)
        {
            ViderFormulaire();
            _isEditing = false;
            Btn_Soumettre.Content = "Soumettre";

        }




    }

}
