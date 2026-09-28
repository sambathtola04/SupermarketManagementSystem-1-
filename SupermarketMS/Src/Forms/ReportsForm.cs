using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using SupermarketManagementSystem.Helpers;

namespace SupermarketManagementSystem.Forms
{
    public class ReportsForm : Form
    {
        private DateTimePicker dtFrom, dtTo;
        private Button btnRun;
        private DataGridView gridSales;
        private DataGridView gridLowStock;
        private Label lblRevenue;
        private TabControl tabs;

        public ReportsForm()
        {
            Text = "Reports";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;

            tabs = new TabControl { Dock = DockStyle.Fill };

            // --- Sales report tab ---
            var tabSales = new TabPage("Sales Report");

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 60 };
            panelTop.Controls.Add(new Label { Text = "From:", Location = new Point(10, 20), AutoSize = true });
            dtFrom = new DateTimePicker { Location = new Point(60, 17), Size = new Size(150, 24), Value = DateTime.Today.AddDays(-7) };
            panelTop.Controls.Add(dtFrom);

            panelTop.Controls.Add(new Label { Text = "To:", Location = new Point(230, 20), AutoSize = true });
            dtTo = new DateTimePicker { Location = new Point(270, 17), Size = new Size(150, 24), Value = DateTime.Today };
            panelTop.Controls.Add(dtTo);

            btnRun = new Button { Text = "Run Report", Location = new Point(440, 15), Size = new Size(110, 28) };
            btnRun.Click += (s, e) => RunSalesReport();
            panelTop.Controls.Add(btnRun);

            lblRevenue = new Label
            {
                Text = "Total Revenue: $0.00",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(580, 20),
                AutoSize = true
            };
            panelTop.Controls.Add(lblRevenue);

            gridSales = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            tabSales.Controls.Add(gridSales);
            tabSales.Controls.Add(panelTop);

            // --- Low stock tab ---
            var tabLowStock = new TabPage("Low Stock Alert");
            gridLowStock = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tabLowStock.Controls.Add(gridLowStock);

            tabs.TabPages.Add(tabSales);
            tabs.TabPages.Add(tabLowStock);
            Controls.Add(tabs);

            Load += (s, e) =>
            {
                RunSalesReport();
                LoadLowStock();
            };
        }

        private void RunSalesReport()
        {
            try
            {
                var table = DBHelper.ExecuteQuery(@"
                    SELECT s.InvoiceNo, s.SaleDate, c.CustomerName, u.FullName AS CashierName,
                           s.SubTotal, s.Discount, s.TotalAmount, s.PaymentMethod
                    FROM Sales s
                    LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                    LEFT JOIN Users u ON s.UserID = u.UserID
                    WHERE s.SaleDate >= @from AND s.SaleDate < @to
                    ORDER BY s.SaleDate DESC",
                    new SqlParameter("@from", dtFrom.Value.Date),
                    new SqlParameter("@to", dtTo.Value.Date.AddDays(1)));

                gridSales.DataSource = table;

                decimal totalRevenue = 0;
                foreach (System.Data.DataRow row in table.Rows)
                    totalRevenue += Convert.ToDecimal(row["TotalAmount"]);

                lblRevenue.Text = $"Total Revenue: ${totalRevenue:0.00}  ({table.Rows.Count} sales)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to run report:\n" + ex.Message);
            }
        }

        private void LoadLowStock()
        {
            try
            {
                gridLowStock.DataSource = DBHelper.ExecuteQuery(@"
                    SELECT ProductCode, ProductName, QuantityInStock, ReorderLevel, Unit
                    FROM Products
                    WHERE QuantityInStock <= ReorderLevel AND IsActive = 1
                    ORDER BY QuantityInStock ASC");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load low stock report:\n" + ex.Message);
            }
        }
    }
}
