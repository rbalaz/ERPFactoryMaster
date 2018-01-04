using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPFactoryMaster
{
    public partial class NewInvoicePage : Page
    {
        private string userId;
        private string userName;
        private string text;
        private string authorisation;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["New"] == null)
                Response.Redirect("MainPage.aspx");
            userName = Request.QueryString["User_Name"];
            userId = Request.QueryString["User_Id"];
            text = AmountBox.Text;
            authorisation = Request.QueryString["Authorisation"];
        }

        protected void ConfirmButton_Click(object sender, EventArgs e)
        {
            string product = ProductList.Text;
            string color = "";
            string[] productParts = product.Split();
            if (productParts.Length == 3)
            {
                color = productParts[2].Replace('ž','z').Replace('č','c').Replace('ý','y');
                color = color.ToLower();
                product = productParts[0] + " " + productParts[1];
            }
            else
                color = "biely";
            int amount = int.Parse(AmountBox.Text);
            if (amount == 0)
            {
                Response.Redirect("MainMenuPage.aspx?Success=" + false + "&User_Name=" + userName + "&User_Id=" + userId + "&Authorisation=" + authorisation);
            }
            else
            {
                string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                string userId = getUserId(sqlConnection);
                string productId = getProductId(sqlConnection, product);
                double unitPrice = getProductUnitPrice(sqlConnection, product);
                int priority = calculatePriority(sqlConnection, product);
                bool success = false;
                if (unitPrice != -1)
                {
                    int maxInvoiceId = getMaxInvoiceId(sqlConnection);
                    DateTime date = DateTime.Now;
                    string formattedDate = date.Year + "-" + date.Month.ToString("D2") + "-" + date.Day.ToString("D2");
                    string insertStatement = "INSERT INTO Invoice VALUES(" + maxInvoiceId + "," + userId + "," + productId + ",'" + color + "','" + formattedDate + "'," + amount + "," + (unitPrice * amount) + "," + priority + ",'Pending','No')";
                    SqlCommand insertCommand = new SqlCommand(insertStatement, sqlConnection);
                    insertCommand.ExecuteNonQuery();
                    success = true;
                }
                sqlConnection.Close();
                Response.Redirect("MainMenuPage.aspx?Success=" + success + "&User_Name=" + userName + "&User_Id=" + userId + "&Authorisation=" + authorisation);
            }
        }

        private int getMaxInvoiceId(SqlConnection connection)
        {
            string selectStatement = "SELECT MAX(ID) AS Max FROM Invoice";
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            SqlDataReader reader = selectCommand.ExecuteReader();
            int maxId = 0;
            while (reader.Read())
            {
                int.TryParse(reader["Max"].ToString(), out maxId);
            }
            reader.Close();
            return ++maxId;
        }

        private int calculatePriority(SqlConnection connection, string productName)
        {
            // 2 factors influence Priority
            // a) user priority 
            // b) current status of production lines

            // 1.) User importance factor
            // Values:
            // Very low - 1
            // Low - 2
            // Average - 3
            // High - 4
            // Very High - 5
            string importance = getUserImportance(connection);
            int userImportance;
            switch (importance)
            {
                case "Very low":
                    userImportance = 5;
                    break;
                case "Low":
                    userImportance = 4;
                    break;
                case "Average":
                    userImportance = 3;
                    break;
                case "High":
                    userImportance = 2;
                    break;
                case "Very high":
                    userImportance = 1;
                    break;
                default:
                    userImportance = 0;
                    break;
            }

            // 2.) Current status of production lines
            // a) if lines currently produce the same product - 0 points
            // b) if lines currently produce a similar product - 2 points
            // c) if lines currenlty produce a different product - 5 point
            // d) if lines currently produce a different product, but there is a longer queue of highly
            // prioritised other products - 1 - 5 points
            // e) if lines are not operational - 0 points
            int productionImportance = 0;
            string currentlyProducedProduct = getCurrentlyProducedProduct(connection);
            if (currentlyProducedProduct == "")
            {
                productionImportance = 0;
            }
            else
            {
                if (currentlyProducedProduct == productName)
                    productionImportance = 0;
                else
                {
                    if (validateSimilarity(productName, currentlyProducedProduct))
                    {
                        productionImportance = 2;
                    }
                    else
                        productionImportance = evaluateInvoiceQueue(connection,currentlyProducedProduct,productName);
                }
            }

            return userImportance + productionImportance;
        }

        private int evaluateInvoiceQueue(SqlConnection connection, string currentProduct, string newProduct)
        {
            string product_Id = getProductIdFromProducts(connection, currentProduct);
            string selectStatement = "SELECT COUNT(User_Id) AS Count FROM Invoice WHERE Product_Id = " + product_Id +" AND Priority > 5 AND Status = 'Pending' AND Authorised = 'yes'";

            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            SqlDataReader reader = selectCommand.ExecuteReader();
            int currentProductInvoiceCount = 0;
            while (reader.Read())
            {
                int.TryParse(reader["Count"].ToString(), out currentProductInvoiceCount);
            }
            reader.Close();

            product_Id = getProductIdFromProducts(connection, newProduct);
            selectStatement = "SELECT COUNT(User_Id) AS Count FROM Invoice WHERE Product_Id = " + product_Id + " AND Priority > 5 AND Status = 'Pending' AND Authorised = 'yes'";

            selectCommand = new SqlCommand(selectStatement, connection);
            reader = selectCommand.ExecuteReader();
            int newProductInvoiceCount = 0;
            while (reader.Read())
            {
                int.TryParse(reader["Count"].ToString(), out newProductInvoiceCount);
            }
            reader.Close();

            if (newProductInvoiceCount > currentProductInvoiceCount)
            {
                if (newProductInvoiceCount - currentProductInvoiceCount > 5)
                    return 0;
                else
                    return 5 - newProductInvoiceCount - currentProductInvoiceCount;
            }
            else
                return 0;
        }

        private string getProductIdFromProducts(SqlConnection connection, string currentProduct)
        {
            string selectStatement = "SELECT Id FROM Products WHERE Product_Name='" + currentProduct + "'";
            SqlCommand selectCommand = new SqlCommand(selectStatement,connection);

            SqlDataReader reader = selectCommand.ExecuteReader();
            string productId = "";
            while (reader.Read())
            {
                productId = reader["Id"].ToString();
            }
            reader.Close();

            return productId;
        }

        private bool validateSimilarity(string product1, string product2)
        {
            // Only coloured and regular paper are similar
            if (product1 == "Farebný papier" && product2 == "Klasický papier")
                return true;
            if (product2 == "Klasický papier" && product2 == "Farebný papier")
                return true;

            return false;
        }

        private string getCurrentlyProducedProduct(SqlConnection connection)
        {
            string product_Id = getProductIdFromInvoice(connection);
            string selectStatement = "SELECT Product_Name FROM Products WHERE Id='" + product_Id + "'";
            SqlCommand selectCommand = new SqlCommand(selectStatement,connection);

            SqlDataReader reader = selectCommand.ExecuteReader();
            string productName = "";
            while (reader.Read())
            {
                productName = reader["Product_Name"].ToString();
            }
            reader.Close();

            return productName;
        }

        private string getProductIdFromInvoice(SqlConnection connection)
        {
            string selectStatement = "SELECT Product_Id FROM Invoice WHERE Status = 'In progress'";
            SqlCommand selectCommand = new SqlCommand(selectStatement,connection);

            SqlDataReader reader = selectCommand.ExecuteReader();
            int[] fieldCounter = new int[3];
            for (int i = 0; i < 3; i++)
            {
                fieldCounter[i] = 0;
            }
            string productId = "";
            while (reader.Read())
            {
                productId = reader["Product_Id"].ToString();
                if (productId == "1")
                    fieldCounter[0]++;
                else if (productId == "2")
                    fieldCounter[1]++;
                else if (productId == "3")
                    fieldCounter[2]++;
            }
            reader.Close();

            int maxCount = fieldCounter.Max();
            int maxPosition = fieldCounter.ToList().Find(max => max == maxCount);

            return maxPosition + "";
        }

        private string getUserImportance(SqlConnection connection)
        {
            string selectStatement = "SELECT Importance FROM Users WHERE User_Name = '" + userName + "'";
            SqlCommand selectCommand = new SqlCommand(selectStatement,connection);

            SqlDataReader reader = selectCommand.ExecuteReader();
            string importance = "";
            while (reader.Read())
            {
                importance = reader["Importance"].ToString();
            }
            reader.Close();

            return importance;
        }

        private double getProductUnitPrice(SqlConnection connection, string productName)
        {
            string selectStatement = "SELECT Unit_Price FROM Products WHERE Product_Name='" + productName + "'";
            SqlCommand selectCommand = new SqlCommand(selectStatement,connection);
            SqlDataReader reader = selectCommand.ExecuteReader();
            double price;
            while (reader.Read())
            {
                if (double.TryParse(reader["Unit_Price"].ToString(), out price))
                {
                    reader.Close();
                    return price;
                }
            }
            reader.Close();

            return -1;
        }

        private string getProductId(SqlConnection connection, string productName)
        {
            string selectStatement = "SELECT Id FROM Products WHERE Product_Name='" + productName + "'";
            SqlCommand selectCommand = new SqlCommand(selectStatement,connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            string productId = "";
            while (reader.Read())
            {
                productId = reader["Id"].ToString();
            }
            reader.Close();

            return productId;
        }

        private string getUserId(SqlConnection connection)
        {
            string selectStatement = "SELECT Id FROM Users WHERE User_Name='" + userName + "'";
            SqlCommand selectCommand = new SqlCommand(selectStatement,connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            string userId = "";
            while (reader.Read())
            {
                userId = reader["Id"].ToString();
            }
            reader.Close();

            return userId;
        }

        protected void AmountText_TextChanged(object sender, EventArgs e)
        {
            string newText = AmountBox.Text;
            bool flag = true;
            for (int i = 0; i < newText.Length; i++)
            {
                if (char.IsDigit(newText[i]) == false)
                    flag = false;
            }

            if (flag != true)
                AmountBox.Text = text;
            else
                text = AmountBox.Text;
        }

        protected void ReturnButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("MainMenuPage.aspx?Success=" + false + "&User_Name=" + userName + "&User_Id=" + userId + "&Authorisation=" + authorisation);
        }
    }
}