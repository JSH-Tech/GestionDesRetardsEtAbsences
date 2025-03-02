using GestionDesRetardsEtAbseneces.Model;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace GestionDesRetardsEtAbseneces.Fenetres
{
    /// <summary>
    /// Interaction logic for FenetreRapport.xaml
    /// </summary>
    public partial class FenetreRapport : Window
    {
        readonly DbGestgrhContext gestgrhContext = new();
        CollectionViewSource rapportViewSource;
        CollectionViewSource employeViewSource;
        public FenetreRapport()
        {
            InitializeComponent();
            rapportViewSource = (CollectionViewSource)FindResource(nameof(rapportViewSource));
            employeViewSource = (CollectionViewSource)FindResource(nameof(employeViewSource));
            gestgrhContext.Database.EnsureCreated();

            //Chargement des donnees
            gestgrhContext.Rapportassiduites.Include(r => r.IdEmployeNavigation).Load();
            gestgrhContext.Employes.Load();

            //Affectation des donnés aux sources de donnees correspondant
            rapportViewSource.Source = gestgrhContext.Rapportassiduites.Local.ToObservableCollection();
            employeViewSource.Source = gestgrhContext.Employes.Local.ToObservableCollection();
        }


        //Recuperation des informations du rapport
        public (bool isValide, string? periodeRapport, int idEmploye, DateOnly deteGeneration, string contenuRapport)? RecupererInfos()
        {
            //Declaration des variables et recuperation des valeurs
            ComboBoxItem periodeSelectionnee = (ComboBoxItem)ComboBoxPeriode.SelectedItem;
            if (periodeSelectionnee is null)
            {
                MessageBox.Show("Veuillez selectionner une periode");
                return null;
            }
            string? periodeRapport = periodeSelectionnee.Content.ToString();
            string? idEmployeSelectionne = ComboBoxEmploye.SelectedValue?.ToString();
            if (idEmployeSelectionne is null)
            {
                MessageBox.Show("Veuillez selectionner un employe");
                return null;
            }
            int idEmploye = int.Parse(idEmployeSelectionne);

            if (!DatePickerDateGeneration.SelectedDate.HasValue)
            {
                MessageBox.Show("Veuillez selectionner une date de generation");
                return null;
            }
            DateOnly dateGeneration = DateOnly.FromDateTime(DatePickerDateGeneration.SelectedDate.Value);

            TextRange textRange = new TextRange(RichTextBoxContenuRapport.Document.ContentStart, RichTextBoxContenuRapport.Document.ContentEnd);
            string contenuRapport = textRange.Text;
            if (contenuRapport is null)
            {
                MessageBox.Show("Le contenu du rapport ne peux pas etre null");
                return null;
            }
            return (true, periodeRapport, idEmploye, dateGeneration, contenuRapport);
        }


        //Au chargement de la fenetre
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DatagridRapport.SelectedItem = null;
            Btn_Modifier.IsEnabled = false;
            Btn_Supprimer.IsEnabled = false;
            Btn_Imprimer.IsEnabled = false;
        }


        /// <summary>
        /// A al selection d'un rapport dans le datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatagridRapport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatagridRapport.SelectedItem is null)
            {
                ViderChamps();
                return;
            }
            if (DatagridRapport.SelectedItem is Rapportassiduite rapportassiduite)
            {
                Btn_Modifier.IsEnabled = DatagridRapport.SelectedItem != null;
                Btn_Supprimer.IsEnabled = DatagridRapport.SelectedItem != null;
                Btn_Imprimer.IsEnabled = DatagridRapport.SelectedItem != null;
                ComboBoxPeriode.SelectedItem = rapportassiduite.PeriodeRapport;
                ComboBoxEmploye.SelectedValue = rapportassiduite.IdEmploye;
                DatePickerDateGeneration.SelectedDate = rapportassiduite.DateGeneration;
                //RichTextBoxContenuRapport.Document.Blocks.Clear();
                RichTextBoxContenuRapport.Document.Blocks.Add(new Paragraph(new Run(rapportassiduite.ContenuRapport)));
            }
        }

        //Vider les champs du formulaire
        private void ViderChamps()
        {
            ComboBoxPeriode.SelectedIndex = -1;
            ComboBoxEmploye.SelectedIndex = -1;
            DatePickerDateGeneration.SelectedDate = null;
            RichTextBoxContenuRapport.Document.Blocks.Clear();
            Window_Loaded(this, new RoutedEventArgs());
        }
        //Generation du rapport
        private void Btn_Generer_Click(object sender, RoutedEventArgs e)
        {
            var informations = RecupererInfos();
            if (informations is null)
            {
                return;
            }
            var (isValide, periodeRapport, idEmploye, dateGeneration, contenuRapport) = informations.Value;
            if (!isValide)
            {
                return;
            }

            //Recuperer un employe par son 
            //Creation du rapport
            Rapportassiduite newrapportassiduite = new()
            {
                PeriodeRapport = periodeRapport,
                DateGeneration = dateGeneration.ToDateTime(TimeOnly.MinValue),
                ContenuRapport = contenuRapport,
                IdEmploye = idEmploye
            };

            //Ajout du rapport dans la base de donnees
            try
            {
                gestgrhContext.Rapportassiduites.Add(newrapportassiduite);
                gestgrhContext.SaveChanges();
                MessageBox.Show("Rapport generé avec succes");
                ViderChamps();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Erreur lors de la generation du rapport: " + ex.Message);
                ViderChamps();
            }
        }


        private void Btn_Actualiser_Click(object sender, RoutedEventArgs e)
        {
            ViderChamps();
        }

        /// <summary>
        /// A la modification d'un rapport
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Modifier_Click(object sender, RoutedEventArgs e)
        {
            var informations = RecupererInfos();
            if (informations is null)
            {
                return;
            }
            var (isValide, periodeRapport, idEmploye, dateGeneration, contenuRapport) = informations.Value;
            if (!isValide)
            {
                return;
            }

            //Comparer les informations du rapport avec celles du formulaire pour eviter les doublons lors de la modification
            if (DatagridRapport.SelectedItem is Rapportassiduite rapportassiduite)
            {
                if (rapportassiduite.PeriodeRapport == periodeRapport && rapportassiduite.IdEmploye == idEmploye && rapportassiduite.DateGeneration == dateGeneration.ToDateTime(TimeOnly.MinValue) && rapportassiduite.ContenuRapport == contenuRapport)
                {
                    MessageBox.Show("Aucune modification effectuée");
                    return;
                }
                else
                {
                    //Recuperer le rapport a modifier
                    Rapportassiduite? rapportAModifier = gestgrhContext.Rapportassiduites.Find(rapportassiduite.IdRapport);
                    if (rapportAModifier is null)
                    {
                        MessageBox.Show("Rapport introuvable");
                        return;
                    }
                    rapportAModifier.PeriodeRapport = periodeRapport;
                    rapportAModifier.IdEmploye = idEmploye;
                    rapportAModifier.DateGeneration = dateGeneration.ToDateTime(TimeOnly.MinValue);
                    rapportAModifier.ContenuRapport = contenuRapport;
                    //Modifier le rapport dans la base de donnees
                    try
                    {
                        gestgrhContext.SaveChanges();
                        MessageBox.Show("Rapport modifié avec succes");
                        ViderChamps();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors de la modification du rapport: " + ex.Message);
                        ViderChamps();
                    }
                }
            }

        }

        /// <summary>
        /// A la suppression d'un rapport
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (DatagridRapport.SelectedItem is null)
            {
                MessageBox.Show("Veuillez selectionner un rapport a supprimer");
                return;
            }

            if (DatagridRapport.SelectedItem is Rapportassiduite rapportassiduite)
            {
                //Recuperer le rapport a supprimer
                Rapportassiduite? rapportASupprimer = gestgrhContext.Rapportassiduites.Find(rapportassiduite.IdRapport);
                if (rapportASupprimer is null)
                {
                    MessageBox.Show("Rapport introuvable");
                    return;
                }
                //Supprimer le rapport de la base de donnees
                try
                {
                    gestgrhContext.Rapportassiduites.Remove(rapportASupprimer);
                    gestgrhContext.SaveChanges();
                    MessageBox.Show("Rapport supprimé avec succes");
                    ViderChamps();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la suppression du rapport: " + ex.Message);
                    ViderChamps();
                }
            }
        }

        /// <summary>
        /// A l'impression du rapport
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Imprimer_Click(object sender, RoutedEventArgs e)
        {
            if (DatagridRapport.SelectedItem is null)
            {
                MessageBox.Show("Veuillez selectionner un rapport a imprimer");
                return;
            }
            if (DatagridRapport.SelectedItem is Rapportassiduite rapportassiduite)
            {
                //Recuperer le rapport a imprimer
                Rapportassiduite? rapportAImprimer = gestgrhContext.Rapportassiduites.Find(rapportassiduite.IdRapport);
                if (rapportAImprimer is null)
                {
                    MessageBox.Show("Rapport introuvable");
                    return;
                }
                //Imprimer le rapport
                try
                {
                    string? periodeRapport = rapportAImprimer.PeriodeRapport;
                    string? nomEmploye = rapportAImprimer.IdEmployeNavigation.Nom;
                    string? prenomEmploye = rapportAImprimer.IdEmployeNavigation.Prenom;
                    string? dateGeneration = rapportAImprimer.DateGeneration.ToString();
                    string? contenuRapport = rapportAImprimer.ContenuRapport;

                    //Verication des informations du rapport
                    if (periodeRapport is null || nomEmploye is null || prenomEmploye is null || dateGeneration is null || contenuRapport is null)
                    {
                        MessageBox.Show("Inform ations du rapport manquantes");
                        return;
                    }

                    FlowDocument doc = new()
                    {
                        PageHeight = 1056, // Hauteur en points (8.5 x 11 pouces pour une feuille A4)
                        PageWidth = 816,
                        ColumnWidth = 816, // Largeur totale sans colonnes multiples
                        FontFamily = new System.Windows.Media.FontFamily("Arial"),
                        FontSize = 14,
                        PagePadding = new Thickness(50)
                    };

                    doc.Blocks.Add(new Paragraph(new Run("Rapport d'assiduité"))
                    {
                        FontSize = 24,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center
                    });

                    doc.Blocks.Add(new Paragraph(new Run("Periode du rapport: " + periodeRapport)));
                    doc.Blocks.Add(new Paragraph(new Run("Employé: " + nomEmploye + " " + prenomEmploye)));
                    doc.Blocks.Add(new Paragraph(new Run("Date de generation: " + dateGeneration)));
                    doc.Blocks.Add(new Paragraph(new Run("Contenu du rapport: ")));
                    doc.Blocks.Add(new Paragraph(new Run(contenuRapport))
                    {
                        FontSize = 16,
                        TextAlignment = TextAlignment.Justify,
                        Margin = new Thickness(20, 10, 20, 10)
                    });


                    PrintDialog printDialog = new();
                    if (printDialog.ShowDialog() == true)
                    {
                        MessageBox.Show("Impression du rapport en cours...");
                        printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Rapport d'assiduité");
                        MessageBox.Show("Rapport imprimé avec succès !");

                    }
                    ViderChamps();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de l'impression du rapport: " + ex.Message);
                    ViderChamps();
                }
            }
        }
    }
}
