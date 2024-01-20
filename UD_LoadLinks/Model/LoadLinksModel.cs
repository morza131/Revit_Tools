using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using LoadLinks.ViewModel;
using MyHelpfullTools;
namespace LoadLinks.Model
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class LoadLinksModel
    {
        public static Dictionary<ModelPath, Workset> GetLinksWorksetsPairs(Document doc, List<string> pathsToLinks, RevitVersion revitVersion)
        {
            string revitSuffix = revitVersion.GetEnumDescription();

            FilteredWorksetCollector worksets = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset);
            Dictionary<ModelPath, Workset> linksWorksetsPairs = new Dictionary<ModelPath, Workset>();
            string currentDocPath = String.Empty;
            if (doc.IsWorkshared)
                currentDocPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(doc.GetWorksharingCentralModelPath());
            else
                currentDocPath = doc.PathName;
            foreach (string path in pathsToLinks)
            {
                if (path != "" && path != currentDocPath)
                {
                    string pathSuffix = path.Substring(path.IndexOf(revitSuffix) + 3);
                    string newPathSuffix = pathSuffix.Remove(pathSuffix.Length - 4);
                    Workset workset = worksets.Where(x => x.Name.Contains(newPathSuffix)).FirstOrDefault();
                    ModelPath mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(path);
                    linksWorksetsPairs.Add(mp, workset);
                }
            }
            return linksWorksetsPairs;
        }
        public static List<string> CheckLinksWorksetsPairs(Dictionary<ModelPath, Workset> worksetsPairs, List<string> pathsToLinks)
        {
            int count = 0;
            List<string> linksWithoutWorkset = new List<string>();
            foreach (string pathsToLink in pathsToLinks)
            {
                if (worksetsPairs.ContainsKey(ModelPathUtils.ConvertUserVisiblePathToModelPath(pathsToLink)))
                {
                    count++;
                }
                else
                    linksWithoutWorkset.Add(pathsToLink);
            }
            return linksWithoutWorkset;
        }
    }
}

