using FirebaseRepos.WPFSimple.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace FirebaseRepos.WPFSimple
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //lock object for synchronization;
        private static object _syncLock = new object();
        public MainWindow()
        {
            InitializeComponent();
            //Enable the cross acces to this collection elsewhere
            BindingOperations.EnableCollectionSynchronization(Context.Users.Local, _syncLock);
            gridd.ItemsSource = Context.Users.Local;
        }


        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            await Context.Users.AddAsync(new User { Email = txtEmail.Text, Name = txtName.Text });
            txtEmail.Text = txtName.Text = "";
        }
    }
}
