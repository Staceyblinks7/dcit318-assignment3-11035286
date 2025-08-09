using System;
using System.Collections.Generic;

#region Step 1: Define the Transaction Record
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);
#endregion

#region Step 2: Define ITransactionProcessor Interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}
#endregion

#region Step 3: Concrete Implementations of ITransactionProcessor

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed: {transaction.Amount:C} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Processed: {transaction.Amount:C} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Processed: {transaction.Amount:C} for {transaction.Category}");
    }
}
#endregion

#region Step 4: Base Account Class

public class Account
{
    public string AccountNumber { get; private set; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"[Account] Deducted {transaction.Amount:C}. New Balance: {Balance:C}");
    }
}
#endregion

#region Step 5: Sealed SavingsAccount Class

public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine($"[SavingsAccount] Insufficient funds for {transaction.Category} ({transaction.Amount:C}). Balance: {Balance:C}");
        }
        else
        {
            base.ApplyTransaction(transaction);
            Console.WriteLine($"[SavingsAccount] Transaction applied. New Balance: {Balance:C}");
        }
    }
}
#endregion

#region Step 6: FinanceApp Class

public class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        // i. Create SavingsAccount
        var account = new SavingsAccount("ACC12345", 1000m);

        // ii. Create 3 sample transactions
        var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 300m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 200m, "Entertainment");

        // iii. Process with appropriate processors
        ITransactionProcessor processor1 = new MobileMoneyProcessor();
        ITransactionProcessor processor2 = new BankTransferProcessor();
        ITransactionProcessor processor3 = new CryptoWalletProcessor();

        processor1.Process(t1);
        processor2.Process(t2);
        processor3.Process(t3);

        // iv. Apply transactions to account
        account.ApplyTransaction(t1);
        account.ApplyTransaction(t2);
        account.ApplyTransaction(t3);

        // v. Add transactions to list
        _transactions.AddRange(new[] { t1, t2, t3 });

        Console.WriteLine("\n[FinanceApp] All transactions have been processed and recorded.");
    }
}
#endregion

#region Step 7: Main Method

public class Program
{
    public static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
#endregion
