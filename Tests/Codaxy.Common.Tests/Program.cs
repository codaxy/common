using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codaxy.Common.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Codaxy.Common.Localization.LocalizationUtil.WriteLocalizationFiles(".", new[] { 
                new Codaxy.Common.Localization.DefaultLocalizationDataProvider()
            });

            PetaTest.Runner.RunMain(args);
        }
    }
}
