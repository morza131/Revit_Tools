using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UD_Tools_R19
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class PL_fittingsWallThickness : IExternalCommand
    {
        

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            List<FamilyInstance> fittings = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
                                                                             .OfCategory(BuiltInCategory.OST_PipeFitting)
                                                                             .Cast<FamilyInstance>()
                                                                             .ToList();
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("Safety transaction");
                foreach (FamilyInstance fitting in fittings)
                {
                    string parValue = "";
                    try
                    {
                        foreach (Connector fitCon in fitting.MEPModel.ConnectorManager.Connectors)
                        {
                            foreach (Connector con in fitCon.AllRefs)
                            {
                                Element ownerPipe = con.Owner;
                                double thickness = UnitUtils.ConvertFromInternalUnits(ownerPipe.LookupParameter("ADSK_Толщина стенки").AsDouble(), DisplayUnitType.DUT_MILLIMETERS);
                                
                                if (thickness % 2 == 0 || thickness % 2 == 1)
                                {
                                    parValue = parValue + thickness +",0"+ "-";
                                }
                                else
                                parValue = parValue + thickness + "-";
                            }            
                        }
                        
                        fitting.LookupParameter("ADSK_Примечание").Set(parValue.Remove(parValue.Length - 1, 1));
                    }
                    catch { continue; }
                }
                transaction.Commit();
                
            }
            return Result.Succeeded;
        }
    }
}
