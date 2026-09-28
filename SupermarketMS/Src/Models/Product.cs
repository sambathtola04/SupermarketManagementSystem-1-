namespace SupermarketManagementSystem.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public int? CategoryID { get; set; }
        public int? SupplierID { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public string Unit { get; set; } = "pcs";
    }

    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
