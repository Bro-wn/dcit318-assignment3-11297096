using System;
using System.Collections.Generic;

// Define immutable Transaction record
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Define interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Concrete implementations
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

// Base Account class
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
    }
}

// Sealed SavingsAccount class
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) 
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds for transaction.");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction successful. Remaining balance: {Balance:C}");
        }
    }
}

// FinanceApp to run the flow
public class FinanceApp
{
    private List<Transaction> _transactions = new();
    private SavingsAccount _account;

    public void Run()
    {
        _account = new SavingsAccount("ACC001", 1000m);
        bool continueProgram = true;

        while (continueProgram)
        {
            Console.WriteLine("\n===== Finance Management System =====");
            Console.WriteLine($"Current Balance: {_account.Balance:C}");
            Console.WriteLine("\n1. Mobile Money Transfer");
            Console.WriteLine("2. Bank Transfer");
            Console.WriteLine("3. Crypto Transfer");
            Console.Write("\nSelect transaction type (1-3): ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                ProcessUserTransaction(choice);
            }
            else
            {
                Console.WriteLine("Invalid choice!");
            }

            Console.Write("\nDo you want to make another transaction? (y/n): ");
            continueProgram = Console.ReadLine()?.ToLower() == "y";
        }
        Console.WriteLine("Thank you for using the Finance Management System!");
    }

    private void ProcessUserTransaction(int choice)
    {
        Console.Write("Enter amount: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.Write("Enter category: ");
            string category = Console.ReadLine() ?? "Uncategorized";

            var transaction = new Transaction(_transactions.Count + 1, DateTime.Now, amount, category);
            ITransactionProcessor processor = choice switch
            {
                1 => new MobileMoneyProcessor(),
                2 => new BankTransferProcessor(),
                3 => new CryptoWalletProcessor(),
                _ => throw new ArgumentException("Invalid processor type")
            };

            processor.Process(transaction);
            _account.ApplyTransaction(transaction);
            _transactions.Add(transaction);
        }
        else
        {
            Console.WriteLine("Invalid amount!");
        }
    }
}

class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
