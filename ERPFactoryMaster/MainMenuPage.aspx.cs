using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace ERPFactoryMaster
{
    public partial class MainMenuPage : Page
    {
        private string userName;
        private string userId;
        private string authorisation;
        private string success;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["New"] == null)
                Response.Redirect("MainPage.aspx");
            authorisation = Request.QueryString["Authorisation"];
            userId = Request.QueryString["User_Id"];
            userName = Request.QueryString["User_Name"];
            if (authorisation == "User")
                PayslipButton.Visible = false;
            if (authorisation != "User")
                NewOrderButton.Visible = false;
            success = Request.QueryString["Success"];
            if (success == "True")
            {
                OrderLabel.Visible = true;
                OrderLabel.Text = "Your order was successfully registered. You can view its current status by clicking View orders button.";
            }
            else if (success == "False")
            {
                OrderLabel.Visible = true;
                OrderLabel.Text = "Your order failed. Please re-enter your order again.";
            }
            else
            {
                OrderLabel.Visible = false;
            }

            LogLabel.Text = "User logged: " + userName;
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            Session.Remove("New");
            Response.Redirect("MainPage.aspx");
        }

        protected void NewOrderButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("NewInvoicePage.aspx?User_Name=" + userName + "&User_Id=" + userId + "&Authorisation=" + authorisation);
        }

        protected void InvoiceButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("InvoicePage.aspx?User_Id=" + userId + "&Authorisation=" + authorisation + "&User_Name=" + userName);
        }

        protected void GeneratePDF(int days, int hourlySalary, string firstName, string lastName)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10, 10, 10, 10);

                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                Paragraph para = new Paragraph("Obdobie " + calculatePeriod() + " " + firstName + " " + lastName);
                para.SpacingBefore = 3;
                para.SpacingAfter = 3;
                para.Font = FontFactory.GetFont(FontFactory.TIMES, 9, BaseColor.BLACK);
                para.Alignment = 0;
                document.Add(para);
                para = new Paragraph("Paper Factory s.r.o.                                   ");
                para.Alignment = 2;
                para.SpacingBefore = 3;
                para.SpacingAfter = 3;
                para.Font = FontFactory.GetFont(FontFactory.TIMES, 9, BaseColor.BLACK);
                document.Add(para);


                PdfPTable table = new PdfPTable(11);
                table.SpacingBefore = 3;
                table.SpacingAfter = 3;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                Font font = FontFactory.GetFont(FontFactory.TIMES, BaseFont.CP1250, BaseFont.EMBEDDED, 7f);
                Font font6 = FontFactory.GetFont(FontFactory.TIMES, BaseFont.CP1250, BaseFont.EMBEDDED, 6f);
                float[] widths = new float[] { 130f, 40f, 60f, 60f, 100f, 70f, 110f, 130f, 130f, 110f, 70f };
                table.SetWidths(widths);
                // Row 1
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("dní", font));
                table.AddCell(createAlignedCell("hod.", font));
                table.AddCell(createAlignedCell("čiastka", font));
                PdfPCell cell7 = createAlignedCell("", font);
                cell7.Colspan = 2;
                table.AddCell(cell7);
                table.AddCell(createAlignedCell("VZ", font));
                table.AddCell(createAlignedCell("zamestnávateľ", font));
                table.AddCell(createAlignedCell("zamestnanec", font));
                PdfPCell cell8 = createAlignedCell("", font);
                cell8.Colspan = 2;
                table.AddCell(cell8);

                // Row 2
                table.AddCell(createAlignedCell("Fond prac. č.", font));
                table.AddCell(createAlignedCell(days + ".0", font));
                table.AddCell(createAlignedCell(days * 8 + ".00", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("Hrubá mzda", font));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 8 * 1.1745 * 100) / 100 + "", font));
                table.AddCell(createAlignedCell("Zdrav.poist.", font));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 0.32 * 100) / 100 + "", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("Týžd. prac. čas", font6));
                table.AddCell(createAlignedCell("", font));

                // Row 3
                table.AddCell(createAlignedCell("Odpracované", font));
                table.AddCell(createAlignedCell(days + ".0", font));
                table.AddCell(createAlignedCell(days * 8 + ".00", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("Nezdaňované", font));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 8 * 1.5847 * 100) / 100 + "", font));
                table.AddCell(createAlignedCell("Nemoc. poist.", font));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 0.112 * 100) / 100 + "", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("Tarif", font6));
                table.AddCell(createAlignedCell(hourlySalary * days * 8 + "/mes.", font6));

                // Row 4
                table.AddCell(createAlignedCell("Dohoda o BPŠ", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell(hourlySalary * days * 8 + ".00", font));
                table.AddCell(createAlignedCell("Čistá mzda", font));
                table.AddCell(createAlignedCell(hourlySalary * days * 8 + ".00", font));
                table.AddCell(createAlignedCell("Starob. poist.", font));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 0.32 * 100) / 100 + "", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("", font));

                // Row 5
                PdfPCell cell1 = createAlignedCell("", font);
                cell1.Rowspan = 5;
                cell1.Colspan = 4;
                table.AddCell(cell1);
                table.AddCell(createAlignedCell("Na účet", font));
                table.AddCell(createAlignedCell(hourlySalary * days * 8 + ".00", font));
                table.AddCell(createAlignedCell("Inval. poist.", font));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 0.24 * 100) / 100 + "", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("Cena práce", font6));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 8 * 1.1745 * 100) / 100 + "", font));

                // Row 6
                table.AddCell(createAlignedCell("K výplate", font));
                table.AddCell(createAlignedCell("0", font));
                table.AddCell(createAlignedCell("Poist. v nezam.", font));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 0.32 * 100) / 100 + "", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("Odvody zamestnávateľ", font6));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 8 * 0.1745 * 100) / 100 + "", font));

                // Row 7
                PdfPCell cell5 = createAlignedCell("", font);
                cell5.Rowspan = 3;
                cell5.Colspan = 2;
                table.AddCell(cell5);
                table.AddCell(createAlignedCell("Úraz. poist.", font));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 8 * 0.008 * 100) / 100 + "", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("Odv. + daň zamestnanec", font6));
                table.AddCell(createAlignedCell("0.00", font));

                // Row 8
                table.AddCell(createAlignedCell("Garan. poist.", font));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 8 * 0.0025 * 100) / 100 + "", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("Odvody + daň spolu", font6));
                table.AddCell(createAlignedCell(Math.Round(hourlySalary * days * 8 * 0.1745 * 100) / 100 + "", font));

                // Row 9
                table.AddCell(createAlignedCell("Rezer. fond", font));
                table.AddCell(createAlignedCell("", font));
                table.AddCell(createAlignedCell("", font));
                PdfPCell cell2 = createAlignedCell("", font);
                cell2.Colspan = 2;
                table.AddCell(cell2);
                document.Add(table);

                document.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                Response.Clear();
                Response.ContentType = "application/pdf";

                string pdfName = "Payslip";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + pdfName + ".pdf");
                Response.ContentType = "application/pdf";
                Response.Buffer = true;
                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.End();
                Response.Close();
            }
        }

        private string calculatePeriod()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month - 1;

            if (month == 0)
            {
                year -= 1;
                month = 12;
            }

            return month + "/" + year;
        }

        private PdfPCell createAlignedCell(string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            return cell;
        }

        protected void PayslipButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=tcp:paperfactoryserver.database.windows.net,1433;Initial Catalog=PaperFactory;Persist Security Info=False;User ID=paperadmin;Password=Factoryadmin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string selectStatement = "SELECT Days_Worked,Salary,First_Name,Last_Name FROM Employees WHERE User_Name='" + userName + "'";
            SqlCommand selectCommand = new SqlCommand(selectStatement, sqlConnection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            int daysWorked = 0;
            int salary = 0;
            string firstName = "", lastName = "";
            while (reader.Read())
            {
                daysWorked = int.Parse(reader["Days_Worked"].ToString());
                salary = int.Parse(reader["Salary"].ToString());
                firstName = reader["First_Name"].ToString();
                lastName = reader["Last_Name"].ToString();
            }
            reader.Close();

            sqlConnection.Close();
            GeneratePDF(daysWorked, salary, firstName, lastName);
        }
    }
}
