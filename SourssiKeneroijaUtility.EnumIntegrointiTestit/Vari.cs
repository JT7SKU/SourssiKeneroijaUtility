using System;
using System.Collections.Generic;
using System.Text;

namespace SourssiKeneroijaUtility.EnumIntegrointiTestit
{
    [EnumLaajennokset]
    [Flags]
    public enum Vari
    {
        Punanen = 1,
        Sininen = 2,
        Vihrea = 4
    }
}
