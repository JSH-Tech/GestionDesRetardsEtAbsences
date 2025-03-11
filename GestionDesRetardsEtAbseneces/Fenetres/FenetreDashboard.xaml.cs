using GestionDesRetardsEtAbseneces.Model;
using LiveCharts.Wpf;
using LiveCharts;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Data;
using GestionDesRetardsEtAbseneces.Controllers;

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
            Utilitaires.InitialiserTimer(this);

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
            RetardsChart.Series.Clear();
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

            // Créer un graphique en bande
            var series = new LiveCharts.Wpf.ColumnSeries
            {
                Title = "Absences",
                Values = new LiveCharts.ChartValues<int>(employesAvecPlusDAbsences.Select(e => e.TotalAbsences))
            };

            // Ajouter la série au graphique
            AbsencesChart.Series.Clear();
            AbsencesChart.Series.Add(series);
        }

        private void Btn_Voir_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_Employe.SelectedItem is Employe employe)
            {
                AfficherGraphiqueEmploye(employe.IdEmploye);
            }
            else
            {
                MessageBox.Show("Veillez selectionner un employé");
            }
        }
        public void AfficherGraphiqueEmploye(int idEmploye)
        {
            try
            {
                // Regrouper les comptages dans une seule requête pour optimiser la performance
                var result = dbGestgrhContext.Employes
                    .Where(e => e.IdEmploye == idEmploye)
                    .Select(e => new
                    {
                        Retards = dbGestgrhContext.Retards.Count(r => r.IdEmploye == idEmploye),
                        Absences = dbGestgrhContext.Absences.Count(a => a.IdEmploye == idEmploye),
                        Conges = dbGestgrhContext.Demandeconges.Count(c => c.IdEmploye == idEmploye)
                    })
                    .FirstOrDefault();

                if (result == null)
                {
                    // Gérer le cas où l'employé n'existe pas
                    MessageBox.Show("Aucun employé trouvé.");
                    return;
                }
                if (result.Retards == 0 && result.Absences == 0 && result.Conges == 0)
                {
                    MessageBox.Show("Aucune donnée disponible pour cet employé.");
                    return;
                }

                // Créer les séries de données pour le graphique
                var series = new SeriesCollection
                {
                    new PieSeries { Title = "Retards", Values = new ChartValues<int> { result.Retards }, DataLabels = true },
                    new PieSeries { Title = "Absences", Values = new ChartValues<int> { result.Absences }, DataLabels = true },
                    new PieSeries { Title = "Congés", Values = new ChartValues<int> { result.Conges }, DataLabels = true }
                };

                // Mettre à jour le graphique
                EmployerDetailChart.Series.Clear();
                EmployerDetailChart.Series = series;

                EmployerDetailChart.AnimationsSpeed = TimeSpan.FromMilliseconds(500);
                EmployerDetailChart.HoverPushOut = 10;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


    }
}
