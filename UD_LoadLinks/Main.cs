using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LoadLinks.View;

namespace LoadLinks
{
    //Для сервисов, которые не требуют данных из ViewModel, мы можем использовать создание ExternalEvents. Но если данные требуются - нужно выстраивать сервисы так
    //, чтобы сервисы выдавали то, что нужно внести в транзакции во ViewModel.
    //Альтернатива - использование библиотеки RevitTask.
    [Transaction(TransactionMode.Manual)]
    internal class Main : IExternalCommand
    {        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MainView window = new MainView(commandData);             
            window.ShowDialog();
            return Result.Succeeded;
        }
    }
}
