// (C) Copyright 2020
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Text.RegularExpressions;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(Mtextedit.MyCommands))]

namespace Mtextedit
{

    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public class MyCommands
    {
        // The CommandMethod attribute can be applied to any public  member 
        // function of any public class.
        // The function should take no arguments and return nothing.
        // If the method is an intance member then the enclosing class is 
        // intantiated for each document. If the member is a static member then
        // the enclosing class is NOT intantiated.
        //
        // NOTE: CommandMethod has overloads where you can provide helpid and
        // context menu.

        // Modal Command with localized name


        [CommandMethod("renumDD", CommandFlags.UsePickSet)]
        public static void CheckForPickfirstSelection()
        {
            // Get the current document and database and editor
            var acDoc = Application.DocumentManager.MdiActiveDocument;
            var acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
            var acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            try
            {
                // Get the PickFirst selection set
                var acSSPrompt = acDocEd.SelectImplied();

                var acSSet = acSSPrompt.Value;

                // Start a transaction
                using (var acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    //loop through objectIds in the selection
                    var numberOfStekers = 1;
                    var index = 1;
                    var promeni = "DD-111-11-1014";
                    var sPattern = "^DD-\\d{3}-\\d{2}-\\d{3}$";//^pocetak stringa, $ kraj stringa, d{3} broj mesta, dakle 3
                    var celina = " ";
                    var sprat = " ";
                    var steker = string.Empty;

                    foreach (ObjectId mTId in acSSet.GetObjectIds())
                    {
                        //set the current MText in the looping selection to btr
                        MText btr = (MText)acTrans.GetObject(mTId, OpenMode.ForWrite);//btr je MText tipa, i iniciramo za write

                        // Get the first one and put it in string promeni
                        if (index == 1)
                        {
                            MText firstBtr = (MText)acTrans.GetObject(mTId, OpenMode.ForRead);//uzimamo firstBtr da dobijemo startni
                            //Putting first MText to promeni
                            promeni = firstBtr.Contents;
                            //getting the DD position
                            int stringPosition = promeni.IndexOf("DD");
                            //getting the DD-XXX-XX-XXX format
                            promeni = promeni.Substring(stringPosition, 13);
                            //checking if format is OK 
                            while (!Regex.IsMatch(promeni, sPattern))
                            {
                                PromptStringOptions pStrOpts = new PromptStringOptions("\nWrong format, please input DD number in format: DD-XXX-XX-XXX: ");
                                pStrOpts.AllowSpaces = true;
                                PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts);
                                promeni = pStrRes.StringResult;
                            }

                            celina = promeni.Substring(3, 3);
                            sprat = promeni.Substring(7, 2);
                            steker = promeni.Substring(10, 3);
                            numberOfStekers = Int32.Parse(steker);

                            index++;
                        }
                        //write a new value to the current MText in the selection
                        int ss = numberOfStekers + 1;
                        //convert to string
                        string sS = numberOfStekers.ToString();
                        string ssS = ss.ToString();
                        //@ is literal string, without @ error escape string
                        //pxr dont know what is? The rest is just string basic op.
                        btr.Contents = @"\pxr-0.25886;" + "DD" + "-" + celina + "-" + sprat + "-" + sS + @"\P" + "DD" + "-" + celina + "-" + sprat + "-" + ssS;
                        numberOfStekers = ss + 1;
                    }
                    //save changes and dispose of the transaction
                    acTrans.Commit();
                }
            }
            catch
            {
                Application.ShowAlertDialog("Try again select only MText!");//greska je uglavnom ako se selektuje nesto sem MText tipa

            }
        }
        /*
        // Start a transaction
        using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
        {

            foreach (ObjectId mTId in acSSet.GetObjectIds())
            {
                MText btr = (MText)acTrans.GetObject(mTId,OpenMode.ForRead);
                if (btr.Contents == "asd")
                { Application.ShowAlertDialog("Number of objects selected: 0");
                }
                else
                {
                    Application.ShowAlertDialog("Number of objects selected: ");
                }

            }


        }
        */

        /* Clear the PickFirst selection set
        ObjectId[] idarrayEmpty = new ObjectId[0];
        acDocEd.SetImpliedSelection(idarrayEmpty);

        // Request for objects to be selected in the drawing area
        acSSPrompt = acDocEd.GetSelection();

        // If the prompt status is OK, objects were selected
        if (acSSPrompt.Status == PromptStatus.OK)
        {
            acSSet = acSSPrompt.Value;

            Application.ShowAlertDialog("Number of objects selected: " +
                                        acSSet.Count.ToString());
        }
        else
        {
            Application.ShowAlertDialog("Number of objects selected: 0");
        }*/
    }

}


