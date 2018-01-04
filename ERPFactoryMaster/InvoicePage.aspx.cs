using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace ERPFactoryMaster
{
    public partial class InvoicePage : Page
    {
        private string userId;
        private string userName;
        private string authorisation;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["New"] == null)
                Response.Redirect("MainPage.aspx");
            userId = Request.QueryString["User_Id"];
            userName = Request.QueryString["User_Name"];
            authorisation = Request.QueryString["Authorisation"];
            MessageLabel.Text = "";
            LogLabel.Text = "User logged: " + userName + ".";

            if (authorisation == "User")
            {
                MessageLabel.Visible = false;
                AuthorisationLabel.Visible = false;
                IdBox.Visible = false;
                ApplyButton.Visible = false;

                listInvoices(userId);
            }
            else if (authorisation == "low")
            {
                MessageLabel.Visible = false;
                AuthorisationLabel.Visible = false;
                IdBox.Visible = false;
                ApplyButton.Visible = false;
                PayLabel.Visible = false;
                PayButton.Visible = false;
                PayIdBox.Visible = false;

                listInvoices();
            }
            else if (authorisation == "high")
            {
                MessageLabel.Visible = true;
                AuthorisationLabel.Visible = true;
                IdBox.Visible = true;
                ApplyButton.Visible = true;
                PayLabel.Visible = false;
                PayButton.Visible = false;
                PayIdBox.Visible = false;

                listInvoices();
            }
        }

        private void listInvoices()
        {
            string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string selectStatement = "SELECT Id,User_Id,Product_Id,Color,Amount,Price,Priority,Status,Authorised FROM Invoice ORDER BY Priority ASC";
            SqlCommand selectCommand = new SqlCommand(selectStatement, sqlConnection);

            SqlDataReader reader = selectCommand.ExecuteReader();
            List<string> headings = new List<string>();
            headings.Add("Invoice_Id");
            headings.Add("User_Id");
            headings.Add("Product_Id");
            headings.Add("Product");
            headings.Add("Color");
            headings.Add("Amount[t]");
            headings.Add("Price[€]");
            headings.Add("Priority");
            headings.Add("Status");
            headings.Add("Authorised");
            initialiseHeadings(headings);

            while (reader.Read())
            {
                TableRow entry = new TableRow();
                TableCell invoiceId = new TableCell();
                invoiceId.Text = reader["Id"].ToString();
                TableCell userId = new TableCell();
                userId.Text = reader["User_Id"].ToString();
                TableCell productId = new TableCell();
                productId.Text = reader["Product_Id"].ToString();
                TableCell productName = new TableCell();
                productName.Text = getProductNameFromId(new SqlConnection(connectionString), reader["Product_Id"].ToString(), false);
                TableCell color = new TableCell();
                color.Text = reader["Color"].ToString();
                TableCell amount = new TableCell();
                amount.Text = reader["Amount"].ToString();
                TableCell price = new TableCell();
                price.Text = reader["Price"].ToString();
                TableCell priority = new TableCell();
                priority.Text = reader["Priority"].ToString();
                TableCell status = new TableCell();
                status.Text = reader["Status"].ToString();
                TableCell authorised = new TableCell();
                authorised.Text = reader["Authorised"].ToString();

                List<TableCell> cells = new List<TableCell>();
                cells.Add(invoiceId);
                cells.Add(userId);
                cells.Add(productId);
                cells.Add(productName);
                cells.Add(color);
                cells.Add(amount);
                cells.Add(price);
                cells.Add(priority);
                cells.Add(status);
                cells.Add(authorised);

                entry.Cells.AddRange(cells.ToArray());
                InvoiceTable.Rows.Add(entry);
            }
            reader.Close();
            InvoiceTable.GridLines = GridLines.Both;

            sqlConnection.Close();
        }

        private void listInvoices(string userId)
        {
            string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string selectStatement = "SELECT Id,Product_Id,Color,Amount,Price,Status FROM Invoice WHERE User_Id=" + userId;
            SqlCommand selectCommand = new SqlCommand(selectStatement, sqlConnection);
            SqlDataReader reader = selectCommand.ExecuteReader();
            List<string> headings = new List<string>();
            headings.Add("Id");
            headings.Add("Product");
            headings.Add("Color");
            headings.Add("Amount[t]");
            headings.Add("Price[€]");
            headings.Add("Status");
            initialiseHeadings(headings);
            while (reader.Read())
            {
                TableRow entry = new TableRow();
                TableCell invoiceId = new TableCell();
                invoiceId.Text = reader["Id"].ToString();
                TableCell productName = new TableCell();
                productName.Text = getProductNameFromId(new SqlConnection(connectionString), reader["Product_Id"].ToString(), false);
                TableCell color = new TableCell();
                color.Text = reader["Color"].ToString();
                TableCell amount = new TableCell();
                amount.Text = reader["Amount"].ToString();
                TableCell price = new TableCell();
                price.Text = reader["Price"].ToString();
                TableCell status = new TableCell();
                status.Text = reader["Status"].ToString();

                List<TableCell> cells = new List<TableCell>();
                cells.Add(invoiceId);
                cells.Add(productName);
                cells.Add(color);
                cells.Add(amount);
                cells.Add(price);
                cells.Add(status);

                entry.Cells.AddRange(cells.ToArray());
                InvoiceTable.Rows.Add(entry);
            }
            reader.Close();
            InvoiceTable.GridLines = GridLines.Both;

            sqlConnection.Close();
        }

        private void initialiseHeadings(List<string> args)
        {
            TableRow header = new TableRow();
            foreach (string argument in args)
            {
                TableCell cell = new TableCell();
                cell.Font.Bold = true;
                cell.Text = argument;
                header.Cells.Add(cell);
            }
            InvoiceTable.Rows.Add(header);
        }

        private string getProductNameFromId(SqlConnection connection, string productId, bool isConnectionOpen)
        {
            if (isConnectionOpen == false)
                connection.Open();
            string selectStatement = "SELECT Product_Name FROM Products WHERE Id = " + productId;
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);

            SqlDataReader reader = selectCommand.ExecuteReader();
            StringBuilder builder = new StringBuilder();
            while (reader.Read())
            {
                builder.Append(reader["Product_Name"]);
            }
            reader.Close();

            if (isConnectionOpen == false)
                connection.Close();

            return builder.ToString();
        }

        protected void ApplyButton_Click(object sender, EventArgs e)
        {
            string invoiceId = IdBox.Text;
            IdBox.BorderColor = System.Drawing.Color.Black;

            if (invoiceId == "")
            {
                IdBox.BorderColor = System.Drawing.Color.Red;
                return;
            }
            int id;
            if (int.TryParse(invoiceId, out id) == false)
                return;

            string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string userOccupation = getUserOccupation(sqlConnection);
            string selectStatement = "SELECT Occupation FROM Authorisation WHERE Invoice_Id=" + invoiceId;
            SqlCommand selectCommand = new SqlCommand(selectStatement, sqlConnection);

            SqlDataReader reader = selectCommand.ExecuteReader();
            bool occupationFound = false;
            while (reader.Read())
            {
                if (reader["Occupation"].ToString() == userOccupation)
                    occupationFound = true;
            }
            reader.Close();
            if (occupationFound == false)
            {
                int maxId = getMaxAuthorisationId(sqlConnection);
                string insertStatement = "INSERT INTO Authorisation VALUES(" + (++maxId) + "," + invoiceId + ",'" + userOccupation + "')";
                SqlCommand insertCommand = new SqlCommand(insertStatement, sqlConnection);
                insertCommand.ExecuteNonQuery();
                updateInvoiceTable(sqlConnection, IdBox.Text);
            }

            sqlConnection.Close();
        }

        private void updateInvoiceTable(SqlConnection connection, string invoiceId)
        {
            string selectStatement = "SELECT COUNT(Id) as Count FROM Authorisation WHERE Invoice_Id=" + invoiceId;
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            int count = 0;
            while (reader.Read())
            {
                int.TryParse(reader["Count"].ToString(), out count);
            }
            reader.Close();

            int paymentCount = getInvoicePaymentCount(connection, invoiceId);

            if (count >= 2 && paymentCount > 0)
            {
                string updateStatement = "UPDATE Invoice SET Authorised='Yes',Status='Pending' WHERE Id=" + invoiceId;
                SqlCommand updateCommand = new SqlCommand(updateStatement, connection);
                updateCommand.ExecuteNonQuery();
            }
        }

        private int getMaxAuthorisationId(SqlConnection connection)
        {
            string selectStatement = "SELECT MAX(ID) AS Max FROM Authorisation";
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            int max = 0;
            while (reader.Read())
            {
                int.TryParse(reader["Max"].ToString(), out max);
            }
            reader.Close();

            return max;
        }

        private string getUserOccupation(SqlConnection connection)
        {
            string selectStatement = "SELECT Occupation FROM Employees WHERE Id=" + userId;
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            StringBuilder builder = new StringBuilder();
            while (reader.Read())
            {
                builder.Append(reader["Occupation"]);
            }
            reader.Close();

            return builder.ToString();
        }

        protected void ShowButton_Click(object sender, EventArgs e)
        {
            string invoiceId = ShowIdBox.Text;
            ShowIdBox.BorderColor = System.Drawing.Color.Black;

            if (invoiceId == "")
            {
                ShowIdBox.BorderColor = System.Drawing.Color.Red;
                return;
            }

            int id;
            if (int.TryParse(invoiceId, out id) == false)
            {
                ShowIdBox.BorderColor = System.Drawing.Color.Red;
                return;
            }

            string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string selectStatement = "SELECT First_Name,Last_Name,Street,Post_Code,City,Country FROM Users WHERE Id='" + userId + "'";
            SqlCommand selectCommand = new SqlCommand(selectStatement, sqlConnection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            string firstName = "", lastName = "", street = "", postCode = "", country = "", city = "";
            while (reader.Read())
            {
                firstName = reader["First_Name"].ToString();
                lastName = reader["Last_Name"].ToString();
                street = reader["Street"].ToString();
                postCode = reader["Post_Code"].ToString();
                country = reader["Country"].ToString();
                city = reader["City"].ToString();
            }
            reader.Close();

            selectStatement = "SELECT Product_Id,Amount,Price,Created FROM Invoice WHERE Id=" + invoiceId;
            selectCommand = new SqlCommand(selectStatement, sqlConnection);
            reader = selectCommand.ExecuteReader();

            string amount = "", price = "", created = "", productId = "";
            while (reader.Read())
            {
                amount = reader["Amount"].ToString();
                price = reader["Price"].ToString();
                created = reader["Created"].ToString();
                productId = reader["Product_Id"].ToString();
            }
            reader.Close();
            string productName = getProductNameFromId(sqlConnection, productId, true);

            sqlConnection.Close();
            string[] parts = created.Split(' ');
            string date = parts[0].Replace('/', '.');
            parts = date.Split('.');
            date = parts[1] + "." + parts[0] + "." + parts[2];
            GenerateBillPDF(firstName, lastName, street, postCode, city, country, double.Parse(price), int.Parse(amount), date, productName);
        }

        protected void GenerateBillPDF(string firstName, string lastName, string stringStreet, string postCode,
            string stringCity, string stringCountry, double price, int amount, string created, string productName)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10, 10, 10, 10);

                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                Font font10 = FontFactory.GetFont(FontFactory.TIMES, BaseFont.CP1250, BaseFont.EMBEDDED, 10f);
                Font font8 = FontFactory.GetFont(FontFactory.TIMES, BaseFont.CP1250, BaseFont.EMBEDDED, 8f);
                Font bold10 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.EMBEDDED, 10f);
                Font bold8 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.EMBEDDED, 8f);
                Font italic8 = FontFactory.GetFont(FontFactory.TIMES_ITALIC, BaseFont.CP1250, BaseFont.EMBEDDED, 8f);
                Font bold16 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.EMBEDDED, 16f);


                Paragraph bill = new Paragraph();
                bill.SpacingAfter = 15;
                bill.Alignment = 0;
                bill.Font = bold16;
                bill.Add("FAKTÚRA");
                document.Add(bill);

                Paragraph enterprise = new Paragraph();
                enterprise.SpacingAfter = 12;
                enterprise.SpacingBefore = 3;
                enterprise.Alignment = 0;
                enterprise.Font = font10;
                enterprise.Add("Paper factory, Letná 9, 042 00, Košice");
                document.Add(enterprise);

                Paragraph receiver = new Paragraph();
                receiver.SpacingAfter = 1;
                receiver.SpacingBefore = 1;
                receiver.Alignment = 0;
                receiver.Font = bold10;
                receiver.Add("ODBERATEĽ");
                document.Add(receiver);

                Paragraph receiverName = new Paragraph();
                receiverName.SpacingAfter = 1;
                receiverName.SpacingBefore = 1;
                receiverName.Alignment = 0;
                receiverName.Font = font10;
                receiverName.Add(firstName + " " + lastName);
                document.Add(receiverName);

                Paragraph street = new Paragraph();
                street.SpacingAfter = 1;
                street.SpacingBefore = 1;
                street.Alignment = 0;
                street.Font = font10;
                street.Add(stringStreet);
                document.Add(street);

                Paragraph city = new Paragraph();
                city.SpacingAfter = 1;
                city.SpacingBefore = 1;
                city.Alignment = 0;
                city.Font = font10;
                city.Add(postCode + ", " + stringCity);
                document.Add(city);

                Paragraph country = new Paragraph();
                country.SpacingAfter = 15;
                country.SpacingBefore = 1;
                country.Alignment = 0;
                country.Font = font10;
                country.Add(stringCountry);
                document.Add(country);

                Paragraph billId = new Paragraph();
                billId.SpacingBefore = 1;
                billId.SpacingAfter = 1;
                billId.Alignment = 0;
                billId.Font = bold10;
                billId.Add("Číslo faktúry: " + ShowIdBox.Text);
                document.Add(billId);

                Paragraph date1 = new Paragraph();
                date1.SpacingAfter = 1;
                date1.SpacingBefore = 1;
                date1.Alignment = 0;
                date1.Font = bold10;
                date1.Add("Dátum vystavenia: " + created);
                document.Add(date1);

                Paragraph date2 = new Paragraph();
                date2.SpacingAfter = 15;
                date2.SpacingBefore = 1;
                date2.Alignment = 0;
                date2.Font = bold10;
                date2.Add("Dátum splatnosti: " + addWeek(created));
                document.Add(date2);

                Paragraph variable = new Paragraph();
                variable.SpacingAfter = 1;
                variable.SpacingBefore = 1;
                variable.Alignment = 0;
                variable.Font = bold10;
                variable.Add("Variabilný symbol: " + (new Random()).Next(1000000, 9999999));
                document.Add(variable);

                Paragraph receiveDate = new Paragraph();
                receiveDate.SpacingAfter = 1;
                receiveDate.SpacingBefore = 1;
                receiveDate.Alignment = 0;
                receiveDate.Font = bold10;
                receiveDate.Add("Dátum dodania: " + addWeek(addWeek(created)));
                document.Add(receiveDate);

                Paragraph payMethod = new Paragraph();
                payMethod.SpacingAfter = 15;
                payMethod.SpacingBefore = 1;
                payMethod.Alignment = 0;
                payMethod.Font = bold10;
                payMethod.Add("Forma úhrady: Prevod");
                document.Add(payMethod);

                Paragraph notes = new Paragraph();
                notes.SpacingAfter = 3;
                notes.SpacingBefore = 3;
                notes.Alignment = 0;
                notes.Font = italic8;
                notes.Add("Faktúrované sú nasledujúce položky:");
                document.Add(notes);

                PdfPTable table = new PdfPTable(6);
                table.SpacingAfter = 8;
                table.SpacingBefore = 3;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                float[] widths = new float[] { 130f, 130f, 130f, 130f, 130f, 130f };
                table.SetWidths(widths);
                // Row 1
                table.AddCell(createAlignedCell("NÁZOV POLOŽKY", bold8));
                table.AddCell(createAlignedCell("POČET", bold8));
                table.AddCell(createAlignedCell("CENA ZA JEDNOTKU[€]", bold8));
                table.AddCell(createAlignedCell("DPH[%]", bold8));
                table.AddCell(createAlignedCell("DPH[€]", bold8));
                table.AddCell(createAlignedCell("SUMA[€]", bold8));
                // Row 2
                table.AddCell(createAlignedCell(productName, font8));
                table.AddCell(createAlignedCell(amount + "", font8));
                table.AddCell(createAlignedCell(price / amount + "", font8));
                table.AddCell(createAlignedCell("20,00", font8));
                table.AddCell(createAlignedCell(0.2 * price + "", font8));
                table.AddCell(createAlignedCell(price + "", font8));
                document.Add(table);

                PdfPTable finalTable = new PdfPTable(2);
                finalTable.SpacingAfter = 8;
                finalTable.SpacingBefore = 3;
                finalTable.HorizontalAlignment = Element.ALIGN_LEFT;
                float[] finalWidths = new float[] { 80f, 80f };
                finalTable.SetWidths(finalWidths);
                // Row 1
                finalTable.AddCell(createAlignedCell("CELKOM BEZ DPH", bold8));
                finalTable.AddCell(createAlignedCell(0.8 * price + "€", font8));
                // Row 2
                finalTable.AddCell(createAlignedCell("DPH 20%:", bold8));
                finalTable.AddCell(createAlignedCell(0.2 * price + "€", font8));
                // Row 3
                finalTable.AddCell(createAlignedCell("SUMA CELKOM", bold8));
                finalTable.AddCell(createAlignedCell(price + "€", font8));
                // Row 4
                finalTable.AddCell(createAlignedCell("SUMA NA ÚHRADU", bold8));
                finalTable.AddCell(createAlignedCell(price + "€", font8));
                document.Add(finalTable);

                Paragraph presenter = new Paragraph();
                presenter.SpacingAfter = 3;
                presenter.SpacingBefore = 3;
                presenter.Alignment = 0;
                presenter.Font = bold10;
                presenter.Add("Vystavil: Róbert Baláž");
                document.Add(presenter);

                document.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                Response.Clear();
                Response.ContentType = "application/pdf";

                string pdfName = "Bill";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + pdfName + ".pdf");
                Response.ContentType = "application/pdf";
                Response.Buffer = true;
                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.End();
                Response.Close();
            }
        }

        private PdfPCell createAlignedCell(string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            return cell;
        }

        private string addWeek(string date)
        {
            string[] subParts = date.Split('.');
            DateTime realDate = new DateTime(int.Parse(subParts[2]), int.Parse(subParts[1]), int.Parse(subParts[0]));

            realDate = realDate.AddDays(7);

            string newDate = realDate.ToString().Split(' ')[0].Replace('/', '.');
            subParts = newDate.Split('.');
            newDate = subParts[1] + "." + subParts[0] + "." + subParts[2];
            return newDate;
        }

        protected void ReturnButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("MainMenuPage.aspx?User_Id=" + userId + "&Authorisation=" + authorisation + "&User_Name=" + userName);
        }

        protected void PayButton_Click(object sender, EventArgs e)
        {
            PayIdBox.BorderColor = System.Drawing.Color.Black;

            string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string invoiceId = PayIdBox.Text;
            int id;
            if (int.TryParse(invoiceId, out id) == false)
            {
                PayIdBox.BorderColor = System.Drawing.Color.Red;
                return;
            }

            int count = getInvoicePaymentCount(sqlConnection, invoiceId);
            if (count > 0)
            {
                MessageLabel.Text = "You have already paid for this bill.";
                MessageLabel.Visible = true;
            }
            else
            {
                MessageLabel.Visible = true;
                MessageLabel.Text = "Bill successfully paid.";

                int maxPaymentId = getMaxPaymentId(sqlConnection);
                string insertStatement = "INSERT INTO Payment VALUES(" + (++maxPaymentId) + "," + invoiceId + ")";
                SqlCommand insertCommand = new SqlCommand(insertStatement, sqlConnection);
                insertCommand.ExecuteNonQuery();

                updateInvoiceTable(sqlConnection, invoiceId);
            }
            sqlConnection.Close();
        }

        private int getInvoicePaymentCount(SqlConnection connection, string invoiceId)
        {
            string selectStatement = "SELECT COUNT(Id) AS Count FROM Payment WHERE Invoice_Id=" + invoiceId;
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            int count = 0;
            while (reader.Read())
            {
                int.TryParse(reader["Count"].ToString(), out count);
            }
            reader.Close();

            return count;
        }

        private int getMaxPaymentId(SqlConnection connection)
        {
            string selectStatement = "SELECT MAX(Id) AS Max FROM Payment";
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            int maxId = 0;
            while (reader.Read())
            {
                int.TryParse(reader["Max"].ToString(), out maxId);
            }
            reader.Close();

            return maxId;
        }
    }
}