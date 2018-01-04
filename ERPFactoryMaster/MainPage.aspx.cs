using System;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;

namespace ERPFactoryMaster
{
    public partial class MainPage : Page
    {
        private string authorisation;
        protected void Page_Load(object sender, EventArgs e)
        {
            LogLabel.Text = "No user logged";
            authorisation = "";
        }

        protected void ConfirmButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            if (userTableExists(sqlConnection) == false)
                createUserTable(sqlConnection);

            string username = LoginBox.Text;
            string password = PasswordBox.Text;

            string selectStatement = "SELECT First_Name,Last_Name,Id FROM Users WHERE User_Name='" + username + "' AND Password='" + password + "'";

            SqlCommand selectCommand = new SqlCommand(selectStatement, sqlConnection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            StringBuilder builder = new StringBuilder();
            StringBuilder occupation = new StringBuilder();
            StringBuilder userId = new StringBuilder();
            while (reader.Read())
            {
                builder.Append(reader["First_Name"]);
                builder.Append(reader["Last_Name"]);
                userId.Append(reader["Id"]);
            }
            reader.Close();

            if (userId.ToString() == "")
            {
                selectStatement = "SELECT First_Name,Last_Name,Occupation,Id FROM Employees WHERE User_Name='" + username + "' AND Password='" + password + "'";
                selectCommand = new SqlCommand(selectStatement, sqlConnection);
                reader = selectCommand.ExecuteReader();

                builder.Clear();
                occupation.Clear();
                userId.Clear();

                while (reader.Read())
                {
                    builder.Append(reader["First_Name"]);
                    builder.Append(reader["Last_Name"]);
                    occupation.Append(reader["Occupation"]);
                    userId.Append(reader["Id"]);
                }
                reader.Close();

                selectStatement = "SELECT Authorisation FROM Permissions WHERE Occupation='" + occupation.ToString() + "'";
                selectCommand = new SqlCommand(selectStatement, sqlConnection);
                reader = selectCommand.ExecuteReader();

                StringBuilder authorisation = new StringBuilder();
                while (reader.Read())
                {
                    authorisation.Append(reader["Authorisation"]);
                }
                reader.Close();

                this.authorisation = authorisation.ToString();
            }

            if (authorisation == "")
                authorisation = "User";
            string id = userId.ToString();

            sqlConnection.Close();

            if (id != "")
            {
                Session.Add("New", 1);
                Response.Redirect("MainMenuPage.aspx?Authorisation=" + this.authorisation + "&User_Name=" + LoginBox.Text + "&User_Id=" + id, true);
            }
            else
                LogLabel.Text = "User login failed.";
        }

        private bool userTableExists(SqlConnection connection)
        {
            string selectStatement = "SELECT * FROM Users";

            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            SqlDataReader reader = null;
            try
            {
                reader = selectCommand.ExecuteReader();
            }
            catch (SqlException)
            {
                reader.Close();
                return false;
            }
            reader.Close();
            return true;
        }

        private void createUserTable(SqlConnection connection)
        {
            string createStatement = "CREATE TABLE Users(ID int,FirstName varchar(255),LastName varchar(255),DateOfBirth date,Occupation varchar(255),Payment int,UserName varchar(255),Password varchar(255))";

            SqlCommand createCommand = new SqlCommand(createStatement, connection);
            createCommand.ExecuteNonQuery();
        }

        protected void RegisterButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("RegistrationPage.aspx");
        }

        protected void CreateButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string dropStatement = "DROP TABLE Output_Warehouse;" +
                                   "DROP TABLE Input_Warehouse;" +
                                   "DROP TABLE Authorisation;" +
                                   "DROP TABLE Payment;" +
                                   "DROP TABLE Invoice;" +
                                   "DROP TABLE Materials;" +
                                   "DROP TABLE Products;" +
                                   "DROP TABLE Users;" +
                                   "DROP TABLE Employees;" +
                                   "DROP TABLE Permissions";

            SqlCommand dropCommand = new SqlCommand(dropStatement, sqlConnection);
            try
            {
                dropCommand.ExecuteNonQuery();
            }
            catch (SqlException) { }


            string createStatement = "CREATE TABLE Permissions" +
                                     "(" +
                                        "Occupation VARCHAR(50) PRIMARY KEY," +
                                        "Authorisation VARCHAR(20)," +
                                     ");" +

                                     "CREATE TABLE Users" +
                                     "(" +
                                        "Id INT PRIMARY KEY," +
                                        "First_Name VARCHAR(50)," +
                                        "Last_Name VARCHAR(50)," +
                                        "Date_Of_Birth DATE," +
                                        "Street VARCHAR(20)," +
                                        "Post_Code VARCHAR(20)," +
                                        "City VARCHAR(20)," +
                                        "Country VARCHAR(20)," +
                                        "User_Name VARCHAR(20)," +
                                        "Password VARCHAR(20)," +
                                        "Importance VARCHAR(20)" +
                                     ");" +

                                     "CREATE TABLE Employees" +
                                     "(" +
                                        "Id INT PRIMARY KEY," +
                                        "First_Name VARCHAR(50)," +
                                        "Last_Name VARCHAR(50)," +
                                        "Date_Of_Birth DATE," +
                                        "Occupation VARCHAR(50) REFERENCES Permissions(Occupation)," +
                                        "Days_Worked INT," +
                                        "Salary REAL," +
                                        "User_Name VARCHAR(20)," +
                                        "Password VARCHAR(20)" +
                                     ");" +

                                     "CREATE TABLE Products" +
                                     "(" +
                                        "Id INT PRIMARY KEY," +
                                        "Product_Name VARCHAR(100)," +
                                        "Unit_Price REAL" +
                                     ")" +

                                     "CREATE TABLE Invoice" +
                                     "(" +
                                        "Id INT PRIMARY KEY," +
                                        "User_Id INT REFERENCES Users(Id)," +
                                        "Product_Id INT REFERENCES Products(Id)," +
                                        "Color VARCHAR(20)," +
                                        "Created DATE," +
                                        "Amount INT," +
                                        "Price REAL," +
                                        "Priority INT," +
                                        "Status VARCHAR(20)," +
                                        "Authorised VARCHAR(3)" +
                                     ");" +

                                     "CREATE TABLE Payment" +
                                     "(" +
                                        "Id INT PRIMARY KEY," +
                                        "Invoice_Id INT REFERENCES Invoice(Id)" +
                                     ");" +

                                     "CREATE TABLE Input_Warehouse" +
                                     "(" +
                                        "Material_Name VARCHAR(20) PRIMARY KEY," +
                                        "Amount INT," +
                                        "Quality INT" +
                                     ");" +

                                     "CREATE TABLE Authorisation" +
                                     "(" +
                                        "Id INT PRIMARY KEY," +
                                        "Invoice_Id INT REFERENCES Invoice(Id)," +
                                        "Occupation VARCHAR(50) REFERENCES Permissions(Occupation)" +
                                     ");" +

                                     "CREATE TABLE Output_Warehouse" +
                                     "(" +
                                        "Invoice_Id INT PRIMARY KEY REFERENCES Invoice(Id)," +
                                        "Product_Id INT REFERENCES Products(Id)," +
                                        "Amount REAL" +
                                     ")";
            SqlCommand createCommand = new SqlCommand(createStatement, sqlConnection);
            createCommand.ExecuteNonQuery();

            sqlConnection.Close();
        }

        protected void FillButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string insertString = "INSERT INTO Permissions VALUES ('CEO','high');" +
                                  "INSERT INTO Permissions VALUES('COO','high');" +
                                  "INSERT INTO Permissions VALUES('Team leader','low');" +
                                  "INSERT INTO Permissions VALUES('Operator','low');" +
                                  "INSERT INTO Employees VALUES('1','Róbert','Baláž','1994-08-16','CEO','20','15','rbalaz','rbalaz');" +
                                  "INSERT INTO Employees VALUES('2','Matej','Kvetko','1994-09-11','COO','19','14','mkvetko','mkvetko');" +
                                  "INSERT INTO Employees VALUES('3','Daniel','Nový','1991-01-01','Team leader','21','10','dnovy','dnovy');" +
                                  "INSERT INTO Employees VALUES('4','Martina','Vysoká','1989-12-12','Operator','18','9','mvysoka','mvysoka');" +
                                  "INSERT INTO Products VALUES('1','Farebný papier','5000.00');" +
                                  "INSERT INTO Products VALUES('2','Klasický papier','2500.00');" +
                                  "INSERT INTO Products VALUES('3','Kartón','1250.00'); ";
            SqlCommand insertCommand = new SqlCommand(insertString, sqlConnection);
            insertCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        protected void SimulationButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string insertString = "INSERT INTO Users VALUES ('1','Herbert','Hoover','1944-06-06','Pasteurova 9','040 12','Košice','Slovensko','hhoover','hhoover','Very high');" +
                                  "INSERT INTO Users VALUES('2','Ignác','Prešovský','1975-08-24','Rozvojová 3','040 22','Košice','Slovensko','ipresov','ipresov','High');" +
                                  "INSERT INTO Users VALUES('3','Viktor','Štúr','1995-12-24','Diamantová 5','040 23','Košice','Slovensko','vstur','vstur','Average');" +
                                  "INSERT INTO Invoice VALUES('1','1','1','zlty','2017-12-24',1,5000.00,1,'Pending','Yes');" +
                                  "INSERT INTO Invoice VALUES('2','2','2','biely','2017-12-24',4,10000.00,3,'Pending','Yes');" +
                                  "INSERT INTO Invoice VALUES('3','3','3','hnedy','2017-12-24',6,7500.00,4,'Pending','Yes');" +
                                  "INSERT INTO Invoice VALUES('4','1','1','modry','2017-12-24',7,35000.00,3,'Pending','Yes');" +
                                  "INSERT INTO Invoice VALUES('5','2','2','biely','2017-12-24',4,10000,4,'Pending','Yes');" +
                                  "INSERT INTO Invoice VALUES('6','3','3','hnedy','2017-12-24',1,1250,5,'Pending','Yes');" +
                                  "INSERT INTO Input_Warehouse VALUES('Wood',100,100);" +
                                  "INSERT INTO Input_Warehouse VALUES('Trash',100,100)";
                                  
            SqlCommand insertCommand = new SqlCommand(insertString, sqlConnection);
            insertCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
}