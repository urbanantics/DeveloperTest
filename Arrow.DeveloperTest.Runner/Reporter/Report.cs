
using System.Collections.Generic;

public record Report
{ 
    public int TotalTransactions { get; set; }
    public List<int> Threads { get; set; }
    public Dictionary<int, List<Transaction>> workers { get; set; }
};

public record Transaction(string account, bool Success);
