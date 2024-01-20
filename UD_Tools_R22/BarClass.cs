using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;



namespace UD_Tools_R22
{
    public class BarClass : IExternalApplication
    {

        public Result OnStartup(UIControlledApplication application)
        {


            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
