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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ZooNew
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["ZooNew.Properties.Settings.ProductsDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            ShowZoos();
            ShowAllAnimals();


        }

        private void ShowZoos()
        {

            try
            {
                string query = "select * from Zoo";
                // the SqlDataAdapter can be imagined like an interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();

                    sqlDataAdapter.Fill(zooTable);

                    //Which information of the Table in DataTable should be shown in our ListBox?
                    listZoos.DisplayMemberPath = "Location";
                    //Which Value should be delivered, when an Item from our ListBox is selected?
                    listZoos.SelectedValuePath = "Id";
                    //The Reference to the Data the ListBox should populate
                    listZoos.ItemsSource = zooTable.DefaultView;
                }

            }
            catch (Exception e)
            {
                Console.Write(e);
            }

        }
        private void ShowAssociatedAnimals()
        {

            try
            {
                string query = "select * from Animal a inner join ZooAnimal " +
                    "za on a.Id = za.AnimalId where za.ZooId = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // the SqlDataAdapter can be imagined like an interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                    DataTable animalTable = new DataTable();

                    sqlDataAdapter.Fill(animalTable);

                    //Which information of the Table in DataTable should be shown in our ListBox?
                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    //Which Value should be delivered, when an Item from our ListBox is selected?
                    listAssociatedAnimals.SelectedValuePath = "Id";
                    //The Reference to the Data the ListBox should populate
                    listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
                    
                }

            }
            catch (Exception)
            {
                Console.Write("Something happened in the Associated Animals Function!");
            }

        }
        private void ShowSelectedZooInTextBox()
        {
            try
            {
                string query = "select Location from Zoo where Id = @zooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // the SqlDataAdapter can be imagined like an interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                    DataTable zooDataTable = new DataTable();

                    sqlDataAdapter.Fill(zooDataTable);

                    myTextBox.Text = zooDataTable.Rows[0]["Location"].ToString();

                }

            }
            catch (Exception)
            {
                Console.Write("Something happened in the Showing Selected Zoon in the Text Box");
            }
        }
        private void ShowSelectedAssociatedInTextBox()
        {
            try
            {
                string query = "select name from Animal where Id = @AnimalID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // the SqlDataAdapter can be imagined like an interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    //MessageBox.Show("flagged for Associated to Text Box");
                    //sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@AnimalID", listAssociatedAnimals.SelectedValue);
                    DataTable zooDataTable = new DataTable();

                    sqlDataAdapter.Fill(zooDataTable);

                    myTextBox.Text = zooDataTable.Rows[0]["Name"].ToString();
                    //AssociatedAnimalDataTable.Rows[0]["AnimalId"].ToString()
                }

            }
            catch (Exception)
            {
                Console.Write("Something happened in the Showing Selected Zoon in the Text Box");
            }
        }
        private void ShowSelectedAnimalInTextBox()
        {
            try
            {
                string query = "select Name from Animal where Id = @Name";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // the SqlDataAdapter can be imagined like an interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@Name", listAnimals.SelectedValue);
                    DataTable AnimalDataTable = new DataTable();

                    sqlDataAdapter.Fill(AnimalDataTable);

                    myTextBox.Text = AnimalDataTable.Rows[0]["Name"].ToString();

                }

            }
            catch (Exception)
            {
                Console.Write("Something happened in the Showing Selected Zoon in the Text Box");
            }
        }

        private void ShowAllAnimals()
        {
            try
            {
                string query = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();

                    sqlDataAdapter.Fill(animalTable);
                    listAnimals.DisplayMemberPath = "Name";
                    listAnimals.SelectedValuePath = "Id";
                    listAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch
            {
                Console.Write("All  Animal List issue");
            }


        }
            

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                
                ShowAssociatedAnimals();
                ShowSelectedZooInTextBox();
            } 
            catch (Exception)
            {
                Console.Write("there was an error listing zoos Selection");
            }
  
           
        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where id = @zooid";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch
            {
                Console.Write("Deleted Zoo Issue");
            }
            finally
            {
                
                ShowZoos();
                ShowAllAnimals();
                ShowAssociatedAnimals();
                sqlConnection.Close();
            }

            //MessageBox.Show("Delete was clicked");
        }

        private void Add_Zoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "Insert into Zoo values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                sqlCommand.ExecuteScalar();

            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                
                ShowZoos();
                sqlConnection.Close();
            }
        }
        
        private void Add_Animal_To_Zoo(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "Insert into ZooAnimal values (@ZooId, @AnimalId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();

            }
        }
        private void Add_Animal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "Insert into Animal values (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                
                ShowAllAnimals();
                sqlConnection.Close();
            }
        }
        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "Update Zoo Set Location = @Location where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooID", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {

                
                sqlConnection.Close();
                ShowZoos();
            }

        }
        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "Update Animal Set Name = @Name where id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                
                ShowAllAnimals();
                sqlConnection.Close();
            }

        }
        private void Remove_Animal(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch
            {
                Console.Write("Removed Animal Issue");
            }
            finally
            {
                
                ShowZoos();
                ShowAllAnimals();
                ShowAssociatedAnimals();
                sqlConnection.Close();
            }

            //MessageBox.Show("Delete was clicked");
        }
        private void myTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
       
        private void listAssociated_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                
                ShowSelectedAssociatedInTextBox();
            }
            catch (Exception)
            {
                Console.Write("there was an error listing zoos Selection");
            }
            
        }

        private void Remove_Associated_Animal(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from ZooAnimal where AnimalId = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAssociatedAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch
            {
                Console.Write("Removed Animal Issue");
            }
            finally
            {
                
               
           
                
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }

        private void listAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                ShowSelectedAnimalInTextBox();
                
            }
            catch (Exception)
            {
                Console.Write("there was an error listing zoos Selection");
            }
           
        }
    }
}
