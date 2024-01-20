using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using AutodeskApplication = Autodesk.Revit.ApplicationServices;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Excel = Microsoft.Office.Interop.Excel;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Events;
using System.IO;

namespace UD_Tools_R22
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class TestRSN : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath("rsn://829mfk/0829_MFK_South/test.rvt");
            OpenOptions opts = new OpenOptions();                      
            UIDocument uiDoc = commandData.Application.OpenAndActivateDocument("rsn://829mfk/0829_MFK_South/test.rvt");
            Document doc = uiDoc.Document;
            return Result.Succeeded;
        }
    }
}
