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

namespace HealthyFood_Shop.GeneralWindows
{
    /// <summary>
    /// Interaction logic for EditValueIDWinfow.xaml
    /// </summary>
    public partial class EditValueIDWinfow : Window
    {
        private SQLite_Interaction.DB_Tables tb;
        private string val;
        private string value_name;
        private long? cid = null;

        public EditValueIDWinfow(SQLite_Interaction.DB_Tables tb_name, in string val_name)
        {
            tb = tb_name;
            value_name = val_name;
            val = "";
            InitializeComponent();
        }

        public EditValueIDWinfow(SQLite_Interaction.DB_Tables tb_name, in long nid, in string curval, in string val_name)
        {
            tb = tb_name;
            value_name = val_name;
            cid = nid;
            val =curval;
            InitializeComponent();
        }

        private void GeneralID_Value_Window_Loaded(object sender, RoutedEventArgs e)
        {
            tb_name.Text = val;
            l_name.Content = value_name;
            this.Title = value_name;
        }

        private void GeneralID_Value_Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && tb_name.Text != "")
            {
                if(cid is null)
                {
                    if(SQLite_Interaction.IsValueInTable(tb_name.Text, tb))
                    {
                        MessageBox.Show("Значення вже існує!",
                            "Помилка", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                    SQLite_Interaction.InsertName(tb, tb_name.Text);
                }
                    
                else
                    SQLite_Interaction.UpdateName(tb, cid.Value, tb_name.Text);
                this.Close();
            }
        }
    }
}
