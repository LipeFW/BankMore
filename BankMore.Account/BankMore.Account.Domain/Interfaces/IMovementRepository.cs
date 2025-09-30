using BankMore.Account.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Account.Domain.Interfaces
{
    public interface IMovementRepository
    {
        Task Add(Movimento movimento);
    }
}
