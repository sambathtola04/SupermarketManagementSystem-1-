using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using SupermarketManagementSystem.Helpers;

namespace SupermarketManagementSystem.Forms
{
    public class CustomersForm : Form
    {
        private DataGridView grid;
        private TextBox txtName, txtPhone, txtEmail, txtAddress;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private int selectedID = 0;

        public CustomersForm()
        {
            Text = "Customers";
            Size = new Size(780, 560);
            StartPosition = FormStartPosition.CenterScreen;

            int y = 20;
            txtName = AddField("Customer Name:", ref y);
            txtPhone = AddField("Phone:", ref y);
            txtEmail = AddField("Email:", ref y);
            txtAddress = AddField("Address:", ref y);

            btnAdd = new Button { Text = "Add", Location = new Point(460, 20), Size = new Size(90, 28) };
            btnUpdate = new Button { Text = "Update", Location = new Point(460, 55), Size = new Size(90, 28) };
            btnDelete = new Button { Text = "Delete", Location = new Point(560, 20), Size = new Size(90, 28) };
            btnClear = new Button { Text = "Clear", Location = new Point(560, 55), Size = new Size(90, 28) };

            btnAdd.Click += (s, e) => AddCustomer();
            btnUpdate.Click += (s, e) => UpdateCustomer();
            btnDelete.Click += (s, e) => DeleteCustomer();
            btnClear.Click += (s, e) => ClearForm();

            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnClear);

            grid = new DataGridView
            {
                Location = new Point(20, 170),
                Size = new Size(720, 340),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.CellClick += Grid_CellClick;
            Controls.Add(grid);

            Load += (s, e) => LoadData();
        }

        private TextBox AddField(string label, ref int y)
        {
            var lbl = new Label { Text = label, Location = new Point(20, y + 3), AutoSize = true };
            var txt = new TextBox { Location = new Point(160, y), Size = new Size(280, 24) };
            Controls.Add(lbl);
            Controls.Add(txt);
            y += 35;
            return txt;
        }

        private void LoadData()
        {
            try
            {
                grid.DataSource = DBHelper.ExecuteQuery(
                    "SELECT CustomerID, CustomerName, Phone, Email, Address, LoyaltyPoints FROM Customers ORDER BY CustomerName");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load customers:\n" + ex.Message);
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            selectedID = Convert.ToInt32(row.Cells["CustomerID"].Value);
            txtName.Text = row.Cells["CustomerName"].Value?.ToString();
            txtPhone.Text = row.Cells["Phone"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();
            txtAddress.Text = row.Cells["Address"].Value?.ToString();
        }

        private void AddCustomer()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Customer name is required.");
                return;
            }
            try
            {
                DBHelper.ExecuteNonQuery(
                    "INSERT INTO Customers (CustomerName, Phone, Email, Address) VALUES (@n, @p, @e, @a)",
                    new SqlParameter("@n", txtName.Text.Trim()),
                    new SqlParameter("@p", (object)txtPhone.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@e", (object)txtEmail.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@a", (object)txtAddress.Text.Trim() ?? DBNull.Value));
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add customer:\n" + ex.Message);
            }
        }

        private void UpdateCustomer()
        {
            if (selectedID == 0) { MessageBox.Show("Select a customer first."); return; }
            try
            {
                DBHelper.ExecuteNonQuery(
                    "UPDATE Customers SET CustomerName=@n, Phone=@p, Email=@e, Address=@a WHERE CustomerID=@id",
                    new SqlParameter("@n", txtName.Text.Trim()),
                    new SqlParameter("@p", (object)txtPhone.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@e", (object)txtEmail.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@a", (object)txtAddress.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@id", selectedID));
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update customer:\n" + ex.Message);
            }
        }

        private void DeleteCustomer()
        {
            if (selectedID == 0) { MessageBox.Show("Select a customer first."); return; }
            if (MessageBox.Show("Delete this customer?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                DBHelper.ExecuteNonQuery("DELETE FROM Customers WHERE CustomerID=@id", new SqlParameter("@id", selectedID));
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete customer:\n" + ex.Message);
            }
        }

        private void ClearForm()
        {
            selectedID = 0;
            txtName.Clear(); txtPhone.Clear(); txtEmail.Clear(); txtAddress.Clear();
        }
    }
}
