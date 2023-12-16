using System;
using System.Linq;
using System.Linq.Expressions;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Components.Dto
{
    public class UserBillingBalance
    {
        private UserBillingBalance()
        {
        }

        public long UserID { get; private set; }
        public User User { get; private set; }
        public UserBillingTransaction LastTransaction { get; private set; }

        public decimal CashBalance
        {
            get { return LastTransaction != null ? LastTransaction.CashBalance : 0m; }
        }

        public decimal BonusBalance
        {
            get { return LastTransaction != null ? LastTransaction.BonusBalance : 0m; }
        }

        public decimal TotalBalance
        {
            get { return CashBalance + BonusBalance; }
        }

        public decimal TotalSpendableBalance
        {
            get { return CashBalance < 0m ? 0m : Math.Max(CashBalance, 0m) + Math.Max(BonusBalance, 0m); }
        }

        public decimal CashTurnover
        {
            get { return LastTransaction != null ? LastTransaction.CashTurnover : 0m; }
        }

        public decimal BonusTurnover
        {
            get { return LastTransaction != null ? LastTransaction.BonusTurnover : 0m; }
        }

        public decimal TotalTurnover
        {
            get { return CashTurnover + BonusTurnover; }
        }

        public bool IsPartial
        {
            get { return LastTransaction != null && LastTransaction.IsPartial; }
        }

        public bool HasPartialInHistory
        {
            get { return LastTransaction != null && LastTransaction.HasPartialInHistory; }
        }

        public bool CanSpend
        {
            get { return TotalSpendableBalance > 0m; }
        }

        public static Expression<Func<User, UserBillingBalance>> MapFromUsers(DateTime? asOf = null)
        {
            if (!asOf.HasValue)
                asOf = DateTime.Now;

            return u => new UserBillingBalance
                        {
                            UserID = u.ID,
                            User = u,
                            LastTransaction = u.BillingTransactions
                                .Where(tx => tx.TransactionTime <= asOf.Value)
                                .OrderByDescending(tx => tx.TransactionTime).FirstOrDefault()
                        };
        }

        public static UserBillingBalance LoadForUserID(IQueryable<UserBillingTransaction> transactions, long userId, DateTime? asOf = null, bool includeAsOfMoment = true)
        {
            if (!asOf.HasValue)
                asOf = DateTime.Now;

	        transactions = transactions.Where(tx => tx.UserID == userId && tx.TransactionTime <= asOf.Value);

	        transactions = includeAsOfMoment
		        ? transactions.Where(tx => tx.TransactionTime <= asOf.Value)
		        : transactions.Where(tx => tx.TransactionTime < asOf.Value);

            var transaction = transactions
                .OrderByDescending(tx => tx.TransactionTime)
                .FirstOrDefault();

            return new UserBillingBalance {UserID = userId, LastTransaction = transaction};
        }

        public static UserBillingBalance CreateFromTransaction(UserBillingTransaction transaction, User user = null)
        {
			if (transaction == null)
				throw new ArgumentNullException("transaction");

            if (user != null && user.ID != transaction.UserID)
                throw new ArgumentException("Transaction's target user does not match passed User object");

            return new UserBillingBalance
                   {
                       LastTransaction = transaction,
                       UserID = transaction.UserID,
                       User = user
                   };
        }

	    public static UserBillingBalance Zero(long userId)
	    {
		    return new UserBillingBalance
		           {
			           LastTransaction = null,
			           UserID = userId,
			           User = null
		           };
	    }

	    public static UserBillingBalance Zero(User user)
	    {
		    if (user == null)
			    throw new ArgumentNullException("user");

		    var result = Zero(user.ID);
		    result.User = user;
		    return result;
	    }
    }
}