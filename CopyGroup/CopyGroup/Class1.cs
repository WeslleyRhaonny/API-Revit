using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Transaction = Autodesk.Revit.DB.Transaction;
using Document = Autodesk.Revit.DB.Document;
using Group = Autodesk.Revit.DB.Group;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace CopyGroup
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                //Get application and document objects
                UIApplication uiapp = commandData.Application;
                Document doc = uiapp.ActiveUIDocument.Document;

                //Define a reference Object to accept the pick result
                Reference pickedref = null;

                //Pick a group
                Selection sel = uiapp.ActiveUIDocument.Selection;
                pickedref = sel.PickObject(ObjectType.Element, "Please select a group");
                GroupPickFilter selFilter = new GroupPickFilter();
                pickedref = sel.PickObject(ObjectType.Element, selFilter, "Please select a group");
                Element elem = doc.GetElement(pickedref);
                Group group = elem as Group;

                //Pick point
                XYZ point = sel.PickPoint("Please pick a point to place group");

                //Place the group
                Transaction trans = new Transaction(doc);
                trans.Start("Lab");
                doc.Create.PlaceGroup(point, group.GroupType);
                trans.Commit();

                return Result.Succeeded;
            }

            //If the user right-clicks or presses Esc, handle the exception
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

        }
    }
}

// Filter to constrain picking to model groups. Only model groups
// are highlighted and can be selected when cursor is hovering.
public class GroupPickFilter : ISelectionFilter
{
    public bool AllowElement(Element e)
    {
        return (e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_IOSModelGroups));
    }
    public bool AllowReference(Reference r, XYZ p)
    {
        return false;
    }
}