using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;

namespace NamelessOld.Libraries.HoukagoTeaTime.Mio.Controller
{
    public abstract class MioPlugin : NamelessObject
    {
        /// <summary>
        /// AutoCAD aplication editor
        /// </summary>
        public Editor Ed
        {
            get { return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor; }
        }
        /// <summary>
        /// The Palette name
        /// </summary>
        public abstract String Name { get; }
        /// <summary>
        /// The name of the palette tabs
        /// </summary>
        public abstract String[] Tabs { get; }
        /// <summary>
        /// The collection of controls to be add to the palette
        /// </summary>
        public abstract UserControl[] Controls { get; }
        /// <summary>
        /// Gets the user control asigned to a tab
        /// If the tab name does not exists returns null
        /// </summary>
        /// <param name="tabName">The selected tab</param>
        /// <returns>The user control</returns>
        public UserControl this[String tabName]
        {
            get { return _Controls.ContainsKey(tabName) ? _Controls[tabName] : null; }
        }
        /// <summary>
        /// The plugin palette
        /// </summary>
        public PaletteSet Palette;
        Dictionary<String, UserControl> _Controls;
        Dictionary<String, Palette> _Palletes;
        /// <summary>
        /// Initialize the palette set
        /// </summary>
        public virtual void InitCommand()
        {
            if (this.Palette == null)
            {
                this.Palette = new PaletteSet(this.Name);
                this.Fill(this.Controls);
                this.InitView();
            }
            else
                this.Reset();
        }

        /// <summary>
        /// Initialize the Palette View
        /// By Default It has a Size of 400,600
        /// And DockEnabaled on Left and Right
        /// </summary>
        public virtual void InitView()
        {
            if (this.Palette != null)
            {
                this.Palette.Size = new Size(300, 600);
                this.Palette.Visible = true;
                this.Palette.DockEnabled = (DockSides)((int)DockSides.Left + (int)DockSides.Right);
                this.Palette.Dock = DockSides.Left;
                this.Palette.KeepFocus = false;
            }
        }
        /// <summary>
        /// Adds an item to the control
        /// </summary>
        /// <param name="name"></param>
        /// <param name="control"></param>
        public void Add(String name, UserControl control)
        {
            if (_Controls == null)
                _Controls = new Dictionary<String, UserControl>();
            if (_Palletes == null)
                _Palletes = new Dictionary<String, Palette>();
            if (!_Controls.ContainsKey(name))
            {
                this._Palletes.Add(name, this.Palette.AddVisual(name, control));
                _Controls.Add(name, control);
            }
        }
        /// <summary>
        /// Fills the current tablet
        /// </summary>
        /// <param name="controls"></param>
        public void Fill(UserControl[] controls)
        {
            if (controls.Length == Tabs.Length)
            {
                for (int i = 0; i < controls.Length; i++)
                    this.Add(Tabs[i], controls[i]);
            }
            else
                throw new RomioException(Errors.BadTabSize);

        }
        /// <summary>
        /// USe this function in case the Palette is not show
        /// </summary>
        public void Reset()
        {
            _Controls = new Dictionary<string, UserControl>();
            _Palletes = new Dictionary<string, Palette>();
            if (this.Palette != null)
            {
                this.Palette.Close();
                this.Palette = null;
            }
            this.InitCommand();
        }
        /// <summary>
        /// Gets the palette by its tab name
        /// returns null if the pallete does not exist
        /// </summary>
        /// <param name="tabName">The name of the tab</param>
        /// <returns>The Palette</returns>
        public Palette GetPalette(String tabName)
        {
            return this._Palletes.ContainsKey(tabName) ? this._Palletes[tabName] : null;
        }
        /// <summary>
        /// Print a message on the AutoCAD console
        /// </summary>
        /// <param name="msg">The message to be printed</param>
        /// <param name="parameters">The AutoCAD print parameters</param>
        public void Print(String msg, params Object[] parameters)
        {
            this.Ed.WriteMessage(msg, parameters);
        }

    }
}
