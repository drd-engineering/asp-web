﻿using System.Collections.Generic;
using DRD.Models;

namespace DRD.Service
{
    public interface IMemberDepositTrxService
    {
        long GetByQueryCount(long memberId);
        decimal GetDepositBalance(long memberId);
    }
}