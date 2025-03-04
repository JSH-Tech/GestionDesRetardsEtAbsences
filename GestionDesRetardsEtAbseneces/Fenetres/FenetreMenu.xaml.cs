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
    /// Interaction logic for FenetreMenu.xaml
    /// </summary>
    public partial class FenetreMenu : Window
    {
        public FenetreMenu()
        {
            InitializeComponent();
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
    }
}
