using GestionDesRetardsEtAbseneces.Model;
using LiveCharts.Wpf;
using LiveCharts;
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
    /// Interaction logic for FenetreDashboard.xaml
    /// </summary>
    public partial class FenetreDashboard : Window
    {
        DbGestgrhContext dbGestgrhContext = new();
        CollectionViewSource employeViewSource;
        public FenetreDashboard()
        {
            InitializeComponent();
            employeViewSource = ((CollectionViewSource)(FindResource("employeViewSource")));
            dbGestgrhContext.Database.EnsureCreated();
            dbGestgrhContext.Employes.Load();
            dbGestgrhContext.Employes.ToList();
            employeViewSource.Source = dbGestgrhContext.Employes.Local.ToObservableCollection();
            ChargerDonneesTableauDeBord();
            AfficherGraphiqueAbsences();
            AfficherGraphiqueRetards();
        }
        private void ChargerDonneesTableauDeBord()
        {
            int totalAbsences = ObtenirTotalAbsences();
            int totalRetards = ObtenirTotalRetards();
            int totalEmployes = ObtenirTotalEmploye();

            // Mise à jour des données affichées
            AbsencesCount.Text = $"Total: {totalAbsences}";
            RetardsCount.Text = $"Total: {totalRetards}";
            TotalEmployeCount.Text = $"Total: {totalEmployes}";
        }

        private int ObtenirTotalEmploye()
        {
            return dbGestgrhContext.Employes.Count();
        }

        private int ObtenirTotalAbsences()
        {
            // Obter le total des absences (à remplacer par l'accès à la base de données).
            return dbGestgrhContext.Absences.Count();
        }

        private int ObtenirTotalRetards()
        {
            // Obter le total des retards (à remplacer par l'accès à la base de données).
            return dbGestgrhContext.Retards.Count();
        }

        //Utiliser livechart pour afficher un graphique a bandeau pour les employés avec le plus de retards
        public void AfficherGraphiqueRetards()
        {
            // Obtenir les employés avec le plus de retards (à remplacer par l'accès à la base de données).
            var employesAvecPlusDeRetards = dbGestgrhContext.Retards
                .GroupBy(r => r.IdEmploye)
                .Select(g => new { IdEmploye = g.Key, TotalRetards = g.Count() })
                .OrderByDescending(g => g.TotalRetards)
                .Take(5)
                .ToList();

            // Créer un graphique en bande
            var series = new LiveCharts.Wpf.ColumnSeries
            {
                Title = "Retards",
                Values = new LiveCharts.ChartValues<int>(employesAvecPlusDeRetards.Select(e => e.TotalRetards))
            };

            // Ajouter la série au graphique
            RetardsChart.Series.Add(series);
        }

        public void AfficherGraphiqueAbsences()
        {
            // Obtenir les employés avec le plus d'absences (à remplacer par l'accès à la base de données).
            var employesAvecPlusDAbsences = dbGestgrhContext.Absences
                .GroupBy(a => a.IdEmploye)
                .Select(g => new { IdEmploye = g.Key, TotalAbsences = g.Count() })
                .OrderByDescending(g => g.TotalAbsences)
                .Take(5)
                .ToList();

            // Créer un graphique en colonnes
            var series = new LiveCharts.Wpf.ColumnSeries
            {
                Title = "Absences",
                Values = new LiveCharts.ChartValues<int>(employesAvecPlusDAbsences.Select(e => e.TotalAbsences))
            };

            // Ajouter la série au graphique
            AbsencesChart.Series.Add(series);
        }

        private void Btn_Voir_Click(object sender, RoutedEventArgs e)
        {
            int idEmploye=ComboBox_Employe.SelectedIndex;
            MessageBox.Show(idEmploye.ToString());
            AfficherGraphiqueEmploye(idEmploye);
        }
        public void AfficherGraphiqueEmploye(int idEmploye)
        {
            // Obtenir les données pour l'employé sélectionné
            int totalRetards = dbGestgrhContext.Retards.Count(r => r.IdEmploye == idEmploye);
            int totalAbsences = dbGestgrhContext.Absences.Count(a => a.IdEmploye == idEmploye);
            int totalConges = dbGestgrhContext.Demandeconges.Count(c => c.IdEmploye == idEmploye);

            // Créer une série de données pour le graphique en secteurs
            var series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Retards",
                    Values = new ChartValues<int> { totalRetards },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Absences",
                    Values = new ChartValues<int> { totalAbsences },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Congés",
                    Values = new ChartValues<int> { totalConges },
                    DataLabels = true
                }
            };

            // Ajouter la série au graphique
            EmployerDetailChart.Series = series;
        }

    }
}
