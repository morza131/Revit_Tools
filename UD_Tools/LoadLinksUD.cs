using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.IO;
using System.Net;

using System.Windows.Forms;

namespace UD_EL_Tools
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class LoadLinksUD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            string filtPath = openFileDialog.FileName;
            string ListOfPaths = String.Empty;
            using (StreamReader reader = new StreamReader(filtPath))
            {
                ListOfPaths = reader.ReadToEnd();
            }
            List<string> paths = ListOfPaths.Split('\n', '\r').ToList();
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            RevitLinkOptions rlo = new RevitLinkOptions(false);
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("Safety transaction");
                FilteredWorksetCollector worksets = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset);
                string revitVersion = "R21";
                foreach (string path in paths)
                {
                    if (path != "" && path != ModelPathUtils.ConvertModelPathToUserVisiblePath(doc.GetWorksharingCentralModelPath()))
                    {
                        string pathSuffix = path.Substring(path.IndexOf(revitVersion)+3);
                        string newPathSuffix = pathSuffix.Remove(pathSuffix.Length - 4);
                        Workset workset = worksets.Where(x => x.Name.Contains(newPathSuffix)).FirstOrDefault();

                        ModelPath mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(path);
                        var linkType = RevitLinkType.Create(doc, mp, rlo);
                        doc.GetElement(linkType.ElementId).get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(workset.Id.IntegerValue);
                        RevitLinkInstance revitLinkInstance = RevitLinkInstance.Create(doc, linkType.ElementId, ImportPlacement.Shared);
                        doc.GetElement(revitLinkInstance.Id).get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(workset.Id.IntegerValue);
                        revitLinkInstance.Pinned = true;
                    }
                }
                transaction.Commit();
            }
            return Result.Succeeded;
        }
    }
}

