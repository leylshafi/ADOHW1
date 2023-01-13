using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace ADOHW1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection connection = null;
        public MainWindow()
        {
            InitializeComponent();
            ConnectDatabase();
            fillDatas();

        }

        public void ConnectDatabase()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
.AddJsonFile("jsconfig1.json")
.Build();
            string connectionString = configuration.GetConnectionString("db1");
            MessageBox.Show(connectionString);
            connection = new SqlConnection(connectionString);
        }
        public void fillDatas()
        {
            SqlDataReader? reader = null;
            try
            {
                connection?.Open();

                using SqlCommand cmd = new SqlCommand("SELECT [Name] FROM Categories", connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    category.Items.Add(reader[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connection?.Close();
                reader?.Close();
            }
        }

        private void category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            authors.Items.Clear();
            if (category.SelectedItem != null)
            {
                authors.IsEnabled = true;

                SqlDataReader? reader = null;
                try
                {
                    connection?.Open();

                    using SqlCommand cmd = new SqlCommand("SELECT Authors.[FirstName]+ ' ' +Authors.[LastName] AS Author FROM Books\r\nJOIN Categories ON Id_Category=Categories.Id\r\nJOIN Authors ON Id_Author=Authors.Id\r\nWHERE Categories.[Name]=@p1", connection);
                    cmd.Parameters.AddWithValue("@p1", category.SelectedItem.ToString());
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        authors.Items.Add(reader[0]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection?.Close();
                    reader?.Close();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lst.Items.Clear();
            SqlDataReader? reader = null;
            try
            {
                connection?.Open();
                using SqlCommand cmd = new SqlCommand("SELECT Books.[Name] FROM Books\r\nWHERE Books.[Name] LIKE '%'+@p2+'%'", connection);
                cmd.Parameters.AddWithValue("@p2", srch.Text);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lst.Items.Add(reader[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connection?.Close();
                reader?.Close();
                srch.Text = null;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();

                string insertString = "INSERT INTO Books(Id,Name,Pages,YearPress,Id_Themes,Id_Category, Id_Author,Id_Press,Comment,Quantity) VALUES(@id,@bookname,@pages,@year,@idthm,@idcat,@idaut,@idpress,@comment,@quantity)";

                using SqlCommand cmd = new SqlCommand(insertString, connection);
                cmd.Parameters.AddWithValue("@id", id.Text);
                cmd.Parameters.AddWithValue("@bookname", name.Text);
                cmd.Parameters.AddWithValue("@pages", pages.Text);
                cmd.Parameters.AddWithValue("@year", yearpress.Text);
                cmd.Parameters.AddWithValue("@idthm", idtheme.Text);
                cmd.Parameters.AddWithValue("@idcat", idcategory.Text);
                cmd.Parameters.AddWithValue("@idaut", idauthor.Text);
                cmd.Parameters.AddWithValue("@idpress", idpress.Text);
                cmd.Parameters.AddWithValue("@comment", comment.Text);
                cmd.Parameters.AddWithValue("@quantity", quantity.Text);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                connection?.Close();
                id.Text = null;
                name.Text = null;
                pages.Text = null;
                yearpress.Text = null;
                idtheme.Text = null;
                idcategory.Text = null;
                idauthor.Text = null;
                idpress.Text = null;
                comment.Text = null;
                quantity.Text = null;
            }
        }
        private void lst_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                connection.Open();
                string insertString = "DELETE FROM Books\r\nWHERE [Name]=@p";

                using SqlCommand cmd = new SqlCommand(insertString, connection);
                cmd.Parameters.AddWithValue("@p", lst.SelectedItem.ToString());

                cmd.ExecuteNonQuery();
                lst.Items.Remove(lst.SelectedItem);
                MessageBox.Show("Deleted Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                connection?.Close();

            }
        }
    }
}
