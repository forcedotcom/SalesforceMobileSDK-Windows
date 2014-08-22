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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.PlatformUI;
using System.Xaml;

namespace TemplateWizard
{
    /// <summary>
    /// Interaction logic for TemplateForm.xaml
    /// </summary>
    public partial class TemplateForm : DialogWindow
    {
        public TemplateForm()
        {
            InitializeComponent();
        }

        private void finishButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
