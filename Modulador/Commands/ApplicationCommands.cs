﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Bordeo.UI;
using DaSoft.Riviera.Modulador.Controller;
using DaSoft.Riviera.Modulador.Core.Runtime;
using DaSoft.Riviera.Modulador.Core.UI;
using DaSoft.Riviera.OldModulador.UI.Delta;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using Nameless.Libraries.HoukagoTeaTime.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Windows.Controls;
using static DaSoft.Riviera.Modulador.Core.Assets.CMDS;

namespace DaSoft.Riviera.Modulador.Commands
{
    /// <summary>
    /// Defines the application commands
    /// </summary>
    public partial class ModuladorCommands : MioPlugin
    {
        /// <summary>
        /// Gets the riviera application.
        /// </summary>
        /// <value>
        /// The riviera application.
        /// </value>
        public RivieraApplication RivApp
        {
            get { return App.Riviera; }
        }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "Modulador Riviera";
        /// <summary>
        /// Gets the tabs.
        /// </summary>
        /// <value>
        /// The tabs.
        /// </value>
        public override string[] Tabs
        {
            get
            {
                if (App.Riviera.DeveloperMode)
                    return new String[]
                    {
                       "Bordeo",
                       "Enfasis Plus",
                       "Developer"
                    };
                else
                    return new String[]
                    {
                       "Bordeo",
                       "Enfasis Plus",
                    };
            }
        }
        /// <summary>
        /// Gets the controls.
        /// </summary>
        /// <value>
        /// The controls.
        /// </value>
        public override UserControl[] Controls
        {
            get
            {
                if (App.Riviera.DeveloperMode && _Controls == null)
                    _Controls = new UserControl[]
                {
                    new TabBordeoMenu(),
                    new Ctrl_Mampara(),
                    new TabDevMenu()
                };
                else if (_Controls == null)
                    _Controls = new UserControl[]
                {
                    new TabBordeoMenu(),
                    new Ctrl_Mampara()
                };
                return _Controls;
            }
        }
        /// <summary>
        /// The controls
        /// </summary>
        private UserControl[] _Controls;
        /// <summary>
        /// Initializes the delta application.
        /// </summary>
        [CommandMethod(START)]
        public void InitDeltaApplication()
        {
            if (App.Riviera == null)
                App.Riviera = new RivieraApplication();
            RivApp.Is3DEnabled = false;
            RivApp.Database.DatabaseLoaded = InitCommand;
            RivApp.InitDatabase();
          //  this.LoadApplication();
        }
        /// <summary>
        /// Loads the application.
        /// </summary>
        private void LoadApplication()
        {
            new QuickTransactionWrapper(
                (Document doc, Transaction tr) =>
                {
                    BlockTable blkTab = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
                    BlockTableRecord model = (BlockTableRecord)blkTab[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForRead);
                    ExtensionDictionaryManager dMan;
                    BordeoLoader bLoader;
                    foreach (ObjectId entId in model)
                        if (entId.GetObject(OpenMode.ForRead) is Entity)
                        {
                            dMan = new ExtensionDictionaryManager(entId, tr);
                            bLoader = new BordeoLoader(dMan);
                            //string code = dMan.GetXRecord(KEY_)
                            //bLoader.Load()
                        }
                });
        }

        /// <summary>
        /// Swaps between a 2D view and the 3D view.
        /// </summary>
        [CommandMethod(SWAP_3D_MODE)]
        public void Swap3DView()
        {
            App.RunCommand(
                delegate ()
                {
                    try
                    {
                        new QuickTransactionWrapper(
                            (Document doc, Transaction tr) =>
                            {
                                App.Riviera.Is3DEnabled = !App.Riviera.Is3DEnabled;
                                try
                                {
                                    foreach (var obj in App.Riviera.Database.ValidObjects)
                                        obj.Draw(tr);
                                }
                                catch (System.Exception exc)
                                {
                                    Selector.Ed.WriteMessage(exc.Message);
                                }

                                Selector.Ed.Regen();
                            }).Run();
                    }
                    catch (System.Exception exc)
                    {
                        Selector.Ed.WriteMessage(exc.Message);
                    }
                });
        }
        /// <summary>
        /// Initialize the delta
        /// </summary>
        [CommandMethod(INIT_APP_UI)]
        public override void InitCommand()
        {
            App.RunCommand(
                delegate ()
                {
                    base.InitCommand();
                    try
                    {

                    }
                    catch (System.Exception exc)
                    {
                        Selector.Ed.WriteMessage(exc.Message);
                    }
                });
        }
        /// <summary>
        /// Configurations this instance.
        /// </summary>
        [CommandMethod(CONFIG_UI)]
        public void Configuration()
        {
            if (App.Riviera == null)
                App.Riviera = new RivieraApplication();
            WinAppSettings win = new WinAppSettings();
            win.ShowDialog();
        }
    }
}
