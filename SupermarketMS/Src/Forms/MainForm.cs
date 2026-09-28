using System;
using System.Drawing;
using System.Windows.Forms;

namespace SupermarketManagementSystem.Forms
{
    public class MainForm : Form
    {
        private readonly Label lblWelcome;

        public MainForm()
        {
            Text = "Supermarket Management System";
            Size = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;

            var menu = new MenuStrip();

            var mnuProducts = new ToolStripMenuItem("&Products");
            mnuProducts.Click += (s, e) => OpenChild(new ProductsForm());

            var mnuCategories = new ToolStripMenuItem("&Categories");
            mnuCategories.Click += (s, e) => OpenChild(new CategoriesForm());

            var mnuSuppliers = new ToolStripMenuItem("&Suppliers");
            mnuSuppliers.Click += (s, e) => OpenChild(new SuppliersForm());

            var mnuCustomers = new ToolStripMenuItem("&Customers");
            mnuCustomers.Click += (s, e) => OpenChild(new CustomersForm());

            var mnuPOS = new ToolStripMenuItem("&Sales (POS)");
            mnuPOS.Click += (s, e) => OpenChild(new POSForm());

            var mnuReports = new ToolStripMenuItem("&Reports");
            mnuReports.Click += (s, e) => OpenChild(new ReportsForm());

            var mnuLogout = new ToolStripMenuItem("Log&out");
            mnuLogout.Click += (s, e) =>
            {
                var login = new LoginForm();
                login.Show();
                Close();
            };

            menu.Items.Add(mnuProducts);
            menu.Items.Add(mnuCategories);
            menu.Items.Add(mnuSuppliers);
            menu.Items.Add(mnuCustomers);
            menu.Items.Add(mnuPOS);
            menu.Items.Add(mnuReports);
            menu.Items.Add(mnuLogout);

            MainMenuStrip = menu;
            Controls.Add(menu);

            lblWelcome = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Text = $"Welcome, {CurrentSession.FullName} ({CurrentSession.Role})"
            };
            Controls.Add(lblWelcome);
            lblWelcome.BringToFront();
        }

        private void OpenChild(Form child)
        {
            child.MdiParent = this;
            child.WindowState = FormWindowState.Maximized;
            child.Show();
        }
    }
}
