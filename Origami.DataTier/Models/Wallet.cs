using System;
using System.Collections.Generic;

namespace Origami.DataTier.Models;

public partial class Wallet
{
    public int WalletId { get; set; }

    public int UserId { get; set; }

    public decimal? Balance { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Transaction> TransactionReceiverWallets { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionSenderWallets { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
