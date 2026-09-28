using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using SupermarketManagementSystem.Helpers;

namespace SupermarketManagementSystem.Forms
{
    public class SuppliersForm : Form
    {
        private DataGridView grid;
        private TextBox txtName, txtContact, txtPhone, txtEmail, txtAddress;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private int selectedID = 0;

        public SuppliersForm()
        {
            Text = "Suppliers";
            Size = new Size(820, 560);
            StartPosition = FormStartPosition.CenterScreen;

            int y = 20;
            txtName = AddField("Supplier Name:", ref y);
            txtContact = AddField("Contact Person:", ref y);
            txtPhone = AddField("Phone:", ref y);
            txtEmail = AddField("Email:", ref y);
            txtAddress = AddField("Address:", ref y);

            btnAdd = new Button { Text = "Add", Location = new Point(500, 20), Size = new Size(90, 28) };
            btnUpdate = new Button { Text = "Update", Location = new Point(500, 55), Size = new Size(90, 28) };
            btnDelete = new Button { Text = "Delete", Location = new Point(600, 20), Size = new Size(90, 28) };
            btnClear = new Button { Text = "Clear", Location = new Point(600, 55), Size = new Size(90, 28) };

            btnAdd.Click += (s, e) => AddSupplier();
            btnUpdate.Click += (s, e) => UpdateSupplier();
            btnDelete.Click += (s, e) => DeleteSupplier();
            btnClear.Click += (s, e) => ClearForm();

            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnClear);

            grid = new DataGridView
            {
                Location = new Point(20, 210),
                Size = new Size(760, 300),
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
            var txt = new TextBox { Location = new Point(160, y), Size = new Size(300, 24) };
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
                    "SELECT SupplierID, SupplierName, ContactName, Phone, Email, Address FROM Suppliers ORDER BY SupplierName");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load suppliers:\n" + ex.Message);
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            selectedID = Convert.ToInt32(row.Cells["SupplierID"].Value);
            txtName.Text = row.Cells["SupplierName"].Value?.ToString();
            txtContact.Text = row.Cells["ContactName"].Value?.ToString();
            txtPhone.Text = row.Cells["Phone"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();
            txtAddress.Text = row.Cells["Address"].Value?.ToString();
        }

        private SqlParameter[] BuildParams() => new[]
        {
            new SqlParameter("@name", txtName.Text.Trim()),
            new SqlParameter("@contact", (object)txtContact.Text.Trim() ?? DBNull.Value),
            new SqlParameter("@phone", (object)txtPhone.Text.Trim() ?? DBNull.Value),
            new SqlParameter("@email", (object)txtEmail.Text.Trim() ?? DBNull.Value),
            new SqlParameter("@address", (object)txtAddress.Text.Trim() ?? DBNull.Value)
        };

        private void AddSupplier()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Supplier name is required.");
                return;
            }
            try
            {
                DBHelper.ExecuteNonQuery(
                    "INSERT INTO Suppliers (SupplierName, ContactName, Phone, Email, Address) VALUES (@name, @contact, @phone, @email, @address)",
                    BuildParams());
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add supplier:\n" + ex.Message);
            }
        }

        private void UpdateSupplier()
        {
            if (selectedID == 0) { MessageBox.Show("Select a supplier first."); return; }
            try
            {
                var pars = new System.Collections.Generic.List<SqlParameter>(BuildParams())
                {
                    new SqlParameter("@id", selectedID)
                };
                DBHelper.ExecuteNonQuery(
                    "UPDATE Suppliers SET SupplierName=@name, ContactName=@contact, Phone=@phone, Email=@email, Address=@address WHERE SupplierID=@id",
                    pars.ToArray());
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update supplier:\n" + ex.Message);
            }
        }

        private void DeleteSupplier()
        {
            if (selectedID == 0) { MessageBox.Show("Select a supplier first."); return; }
            if (MessageBox.Show("Delete this supplier?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                DBHelper.ExecuteNonQuery("DELETE FROM Suppliers WHERE SupplierID=@id", new SqlParameter("@id", selectedID));
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete (it may be used by existing products):\n" + ex.Message);
            }
        }

        private void ClearForm()
        {
            selectedID = 0;
            txtName.Clear(); txtContact.Clear(); txtPhone.Clear(); txtEmail.Clear(); txtAddress.Clear();
        }
    }
}
