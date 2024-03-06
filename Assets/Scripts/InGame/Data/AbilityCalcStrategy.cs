using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InGame.Data
{
    public enum AbilityCalcStrategy
    {
        GeneralCalcStrategy,
        PercentCalcStrategy,
        PercentPointCalcStrategy,
        OverwriteGreaterCalcStrategy,
        RateCalcStrategy
    }
}
