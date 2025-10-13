using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? SenderWalletId { get; set; }

    public int? ReceiverWalletId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Wallet? ReceiverWallet { get; set; }

    public virtual Wallet? SenderWallet { get; set; }
}
