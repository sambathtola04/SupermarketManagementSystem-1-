using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using SupermarketManagementSystem.Helpers;

namespace SupermarketManagementSystem.Forms
{
    public class CategoriesForm : Form
    {
        private DataGridView grid;
        private TextBox txtName, txtDesc;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private int selectedID = 0;

        public CategoriesForm()
        {
            Text = "Categories";
            Size = new Size(700, 500);
            StartPosition = FormStartPosition.CenterScreen;

            var lblName = new Label { Text = "Category Name:", Location = new Point(20, 20), AutoSize = true };
            txtName = new TextBox { Location = new Point(150, 17), Size = new Size(250, 24) };

            var lblDesc = new Label { Text = "Description:", Location = new Point(20, 55), AutoSize = true };
            txtDesc = new TextBox { Location = new Point(150, 52), Size = new Size(250, 24) };

            btnAdd = new Button { Text = "Add", Location = new Point(420, 17), Size = new Size(80, 28) };
            btnUpdate = new Button { Text = "Update", Location = new Point(420, 52), Size = new Size(80, 28) };
            btnDelete = new Button { Text = "Delete", Location = new Point(510, 17), Size = new Size(80, 28) };
            btnClear = new Button { Text = "Clear", Location = new Point(510, 52), Size = new Size(80, 28) };

            btnAdd.Click += (s, e) => AddCategory();
            btnUpdate.Click += (s, e) => UpdateCategory();
            btnDelete.Click += (s, e) => DeleteCategory();
            btnClear.Click += (s, e) => ClearForm();

            grid = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(640, 350),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.CellClick += Grid_CellClick;

            Controls.Add(lblName);
            Controls.Add(txtName);
            Controls.Add(lblDesc);
            Controls.Add(txtDesc);
            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnClear);
            Controls.Add(grid);

            Load += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                grid.DataSource = DBHelper.ExecuteQuery("SELECT CategoryID, CategoryName, Description FROM Categories ORDER BY CategoryName");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load categories:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            selectedID = Convert.ToInt32(row.Cells["CategoryID"].Value);
            txtName.Text = row.Cells["CategoryName"].Value?.ToString();
            txtDesc.Text = row.Cells["Description"].Value?.ToString();
        }

        private void AddCategory()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Category name is required.");
                return;
            }
            try
            {
                DBHelper.ExecuteNonQuery(
                    "INSERT INTO Categories (CategoryName, Description) VALUES (@n, @d)",
                    new SqlParameter("@n", txtName.Text.Trim()),
                    new SqlParameter("@d", (object)txtDesc.Text.Trim() ?? DBNull.Value));
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add category:\n" + ex.Message);
            }
        }

        private void UpdateCategory()
        {
            if (selectedID == 0)
            {
                MessageBox.Show("Select a category from the grid first.");
                return;
            }
            try
            {
                DBHelper.ExecuteNonQuery(
                    "UPDATE Categories SET CategoryName=@n, Description=@d WHERE CategoryID=@id",
                    new SqlParameter("@n", txtName.Text.Trim()),
                    new SqlParameter("@d", (object)txtDesc.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@id", selectedID));
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update category:\n" + ex.Message);
            }
        }

        private void DeleteCategory()
        {
            if (selectedID == 0)
            {
                MessageBox.Show("Select a category from the grid first.");
                return;
            }
            if (MessageBox.Show("Delete this category?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                DBHelper.ExecuteNonQuery("DELETE FROM Categories WHERE CategoryID=@id", new SqlParameter("@id", selectedID));
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
            txtName.Clear();
            txtDesc.Clear();
        }
    }
}
