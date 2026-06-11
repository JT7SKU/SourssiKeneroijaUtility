using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SourssiKeneroijaUtility.EnumTestit
{
    public static class ModuuliKaynnistin
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifySourceGenerators.Initialize();
        }
    }
}
