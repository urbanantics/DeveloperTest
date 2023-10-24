using System.Collections.Generic;
using System.Threading;
using System;

public class MockAccountDataStore : IAccountDataStore
{
    private readonly int _partition;
    private Random _rand = new Random();

    private Dictionary<string, Account> _accounts = new Dictionary<string, Account>()
        {
            { "1001", new Account(){ AccountNumber = "1001", Balance=100000, Status=AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
            { "1002", new Account(){ AccountNumber = "1002", Balance=600000, Status=AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments} },
            { "1003", new Account(){ AccountNumber = "1003", Balance=9000000, Status=AccountStatus.Live, AllowedPaymentSchemes =AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
            { "1004", new Account(){ AccountNumber = "1004", Balance=3400000, Status=AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments} },
            { "1005", new Account(){ AccountNumber = "1005", Balance=3520000, Status=AccountStatus.Disabled, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs } },

            { "2001", new Account(){ AccountNumber = "2001", Balance=400000, Status=AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
            { "2002", new Account(){ AccountNumber = "2002", Balance=5600, Status=AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
            { "2003", new Account(){ AccountNumber = "2003", Balance=2430000, Status=AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
            { "2004", new Account(){ AccountNumber = "2004", Balance=320000, Status=AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
            { "2005", new Account(){ AccountNumber = "2005", Balance=76340000, Status=AccountStatus.Disabled, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },

            { "3001", new Account(){ AccountNumber = "3001", Balance=540000, Status=AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
            { "3002", new Account(){ AccountNumber = "3002", Balance=7600000, Status=AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
            { "3003", new Account(){ AccountNumber = "3003", Balance=320000, Status=AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
            { "3004", new Account(){ AccountNumber = "3004", Balance=65300000, Status=AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
            { "3005", new Account(){ AccountNumber = "3005", Balance=1200000, Status=AccountStatus.Disabled, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments } },
        };


    public MockAccountDataStore(int partition)
    {
        this._partition = partition;
    }
    public Account GetAccount(string accountNumber)
    {
        // simulate db hit
        Thread.Sleep(_rand.Next(2, 5));

        return _accounts[accountNumber];
    }

    public void UpdateAccount(Account account)
    {
        // simulate db hit
        Thread.Sleep(_rand.Next(2, 5));

        _accounts[account.AccountNumber] = account;
    }
}
