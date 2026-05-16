using Microsoft.Data.Sqlite;

public class DatabaseHelper
{
    public static void Initialize()
    {
        using var connection = new SqliteConnection("Data Source=sales.db");
        connection.Open();

        var cmd = connection.CreateCommand();

        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Sales (
    Id INTEGER PRIMARY KEY,
    Product TEXT,
    Amount REAL,
    SaleDate TEXT
);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
INSERT INTO Sales (Product, Amount, SaleDate) VALUES
('Apple', 1200, '2026-03-01'),
('Apple', 1500, '2026-03-02'),
('Samsung', 800, '2026-03-03');";
        cmd.ExecuteNonQuery();
    }
}