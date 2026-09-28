private void FixProductLayout()
{
    var grid = Controls.OfType<DataGridView>().FirstOrDefault();
    if (grid == null) return;

    // រកគែមក្រោមបំផុតរបស់ប្រអប់បញ្ចូលទាំងអស់ (មិនរាប់ grid)
    int fieldsBottom = Controls.Cast<Control>()
        .Where(c => c != grid)
        .Max(c => c.Bottom);

    grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    grid.Location = new Point(24, fieldsBottom + 20);
    grid.Width = ClientSize.Width - 48;
    grid.Height = Math.Max(200, ClientSize.Height - grid.Top - 20);

    // ជំនួសផ្ទៃពណ៌ប្រផេះខាងក្រោម
    grid.BackgroundColor = Color.White;
    grid.BorderStyle = BorderStyle.None;
    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    grid.MultiSelect = false;
    grid.ReadOnly = true;
    grid.AllowUserToAddRows = false;
    grid.RowHeadersVisible = false;
    grid.EnableHeadersVisualStyles = false;
    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
}