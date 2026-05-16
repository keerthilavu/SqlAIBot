using Microsoft.SemanticKernel;
using System.ComponentModel;

public class SqlPlugin
{
    [KernelFunction, Description("Database schema")]
    public string GetSchema()
    {
        return @"Table: Sales 
Columns: Id, Product, Amount, SaleDate";
    }
}