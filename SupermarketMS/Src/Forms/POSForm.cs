using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using SupermarketManagementSystem.Helpers;
using SupermarketManagementSystem.Models;

namespace SupermarketManagementSystem.Forms
{
    public class POSForm : Form
    {
        private TextBox txtCode;
        private NumericUpDown numQty;
        private Button btnAddToCart, btnCheckout, btnRemove;
        private DataGridView gridCart;
        private ComboBox cboCustomer;
        private Label lblTotal;
        private TextBox txtDiscount, txtPaid;
        private BindingList<CartItem> cart = new();

        public POSForm()
        {
            Text = "Sales (Point of Sale)";
            Size = new Size(950, 650);
            StartPosition = FormStartPosition.CenterScreen;

            Controls.Add(new Label { Text = "Scan / Enter Product Code:", Location = new Point(20, 20), AutoSize = true });
            txtCode = new TextBox { Location = new Point(220, 17), Size = new Size(180, 24) };
            txtCode.KeyDown += TxtCode_KeyDown;
            Controls.Add(txtCode);

            Controls.Add(new Label { Text = "Qty:", Location = new Point(410, 20), AutoSize = true });
            numQty = new NumericUpDown { Location = new Point(450, 17), Size = new Size(60, 24), Minimum = 1, Maximum = 9999, Value = 1 };
            Controls.Add(numQty);

            btnAddToCart = new Button { Text = "Add to Cart", Location = new Point(520, 15), Size = new Size(110, 28) };
            btnAddToCart.Click += (s, e) => AddToCart();
            Controls.Add(btnAddToCart);

            btnRemove = new Button { Text = "Remove Line", Location = new Point(640, 15), Size = new Size(110, 28) };
            btnRemove.Click += (s, e) => RemoveSelected();
            Controls.Add(btnRemove);

            Controls.Add(new Label { Text = "Customer:", Location = new Point(20, 60), AutoSize = true });
            cboCustomer = new ComboBox { Location = new Point(220, 57), Size = new Size(300, 24), DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cboCustomer);

            gridCart = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(890, 350),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            gridCart.DataSource = cart;
            Controls.Add(gridCart);

            Controls.Add(new Label { Text = "Discount:", Location = new Point(20, 470), AutoSize = true });
            txtDiscount = new TextBox { Location = new Point(110, 467), Size = new Size(100, 24), Text = "0" };
            txtDiscount.TextChanged += (s, e) => RefreshTotal();
            Controls.Add(txtDiscount);

            lblTotal = new Label
            {
                Text = "Total: $0.00",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 500),
                AutoSize = true
            };
            Controls.Add(lblTotal);

            Controls.Add(new Label { Text = "Amount Paid:", Location = new Point(20, 545), AutoSize = true });
            txtPaid = new TextBox { Location = new Point(140, 542), Size = new Size(120, 24) };
            Controls.Add(txtPaid);

            btnCheckout = new Button
            {
                Text = "Checkout / Save Sale",
                Location = new Point(650, 530),
                Size = new Size(260, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCheckout.Click += (s, e) => Checkout();
            Controls.Add(btnCheckout);

            Load += (s, e) => LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                var custs = DBHelper.ExecuteQuery("SELECT CustomerID, CustomerName FROM Customers ORDER BY CustomerName");
                cboCustomer.DisplayMember = "CustomerName";
                cboCustomer.ValueMember = "CustomerID";
                cboCustomer.DataSource = custs;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load customers:\n" + ex.Message);
            }
        }

        private void TxtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddToCart();
            }
        }

        private void AddToCart()
        {
            string code = txtCode.Text.Trim();
            if (string.IsNullOrEmpty(code)) return;

            try
            {
                var table = DBHelper.ExecuteQuery(
                    "SELECT ProductID, ProductCode, ProductName, SellPrice, QuantityInStock FROM Products WHERE ProductCode=@c AND IsActive=1",
                    new SqlParameter("@c", code));

                if (table.Rows.Count == 0)
                {
                    MessageBox.Show("Product not found: " + code);
                    return;
                }

                var row = table.Rows[0];
                int productId = Convert.ToInt32(row["ProductID"]);
                int stock = Convert.ToInt32(row["QuantityInStock"]);
                int qty = (int)numQty.Value;

                if (qty > stock)
                {
                    MessageBox.Show($"Not enough stock. Available: {stock}");
                    return;
                }

                var existing = cart.FirstOrDefault(c => c.ProductID == productId);
                if (existing != null)
                {
                    existing.Quantity += qty;
                    cart.ResetBindings();
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductID = productId,
                        ProductCode = row["ProductCode"].ToString(),
                        ProductName = row["ProductName"].ToString(),
                        UnitPrice = Convert.ToDecimal(row["SellPrice"]),
                        Quantity = qty
                    });
                }

                txtCode.Clear();
                numQty.Value = 1;
                txtCode.Focus();
                RefreshTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add product:\n" + ex.Message);
            }
        }

        private void RemoveSelected()
        {
            if (gridCart.CurrentRow == null) return;
            var item = gridCart.CurrentRow.DataBoundItem as CartItem;
            if (item != null)
            {
                cart.Remove(item);
                RefreshTotal();
            }
        }

        private decimal SubTotal() => cart.Sum(c => c.LineTotal);

        private decimal Discount()
        {
            decimal.TryParse(txtDiscount.Text, out var d);
            return d;
        }

        private void RefreshTotal()
        {
            decimal total = SubTotal() - Discount();
            if (total < 0) total = 0;
            lblTotal.Text = $"Total: ${total:0.00}";
        }

        private void Checkout()
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Cart is empty.");
                return;
            }

            decimal subTotal = SubTotal();
            decimal discount = Discount();
            decimal total = Math.Max(0, subTotal - discount);

            if (!decimal.TryParse(txtPaid.Text, out var paid))
            {
                MessageBox.Show("Enter a valid amount paid.");
                return;
            }
            if (paid < total)
            {
                MessageBox.Show("Amount paid is less than the total.");
                return;
            }
            decimal change = paid - total;

            using var conn = DBHelper.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                string invoiceNo = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");

                using var cmdHeader = new SqlCommand(@"
                    INSERT INTO Sales (InvoiceNo, CustomerID, UserID, SubTotal, Discount, Tax, TotalAmount, AmountPaid, ChangeDue, PaymentMethod)
                    OUTPUT INSERTED.SaleID
                    VALUES (@inv, @cust, @user, @sub, @disc, 0, @total, @paid, @change, 'Cash')", conn, tx);
                cmdHeader.Parameters.AddWithValue("@inv", invoiceNo);
                cmdHeader.Parameters.AddWithValue("@cust", (object)cboCustomer.SelectedValue ?? DBNull.Value);
                cmdHeader.Parameters.AddWithValue("@user", CurrentSession.UserID);
                cmdHeader.Parameters.AddWithValue("@sub", subTotal);
                cmdHeader.Parameters.AddWithValue("@disc", discount);
                cmdHeader.Parameters.AddWithValue("@total", total);
                cmdHeader.Parameters.AddWithValue("@paid", paid);
                cmdHeader.Parameters.AddWithValue("@change", change);

                int saleId = (int)cmdHeader.ExecuteScalar();

                foreach (var item in cart)
                {
                    using var cmdDetail = new SqlCommand(@"
                        INSERT INTO SaleDetails (SaleID, ProductID, Quantity, UnitPrice)
                        VALUES (@sale, @prod, @qty, @price)", conn, tx);
                    cmdDetail.Parameters.AddWithValue("@sale", saleId);
                    cmdDetail.Parameters.AddWithValue("@prod", item.ProductID);
                    cmdDetail.Parameters.AddWithValue("@qty", item.Quantity);
                    cmdDetail.Parameters.AddWithValue("@price", item.UnitPrice);
                    cmdDetail.ExecuteNonQuery();

                    using var cmdStock = new SqlCommand(
                        "UPDATE Products SET QuantityInStock = QuantityInStock - @qty WHERE ProductID=@id", conn, tx);
                    cmdStock.Parameters.AddWithValue("@qty", item.Quantity);
                    cmdStock.Parameters.AddWithValue("@id", item.ProductID);
                    cmdStock.ExecuteNonQuery();
                }

                tx.Commit();

                MessageBox.Show($"Sale saved.\nInvoice: {invoiceNo}\nTotal: ${total:0.00}\nChange: ${change:0.00}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                cart.Clear();
                txtDiscount.Text = "0";
                txtPaid.Clear();
                RefreshTotal();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Checkout failed, no changes were saved:\n" + ex.Message);
            }
        }
    }
}
