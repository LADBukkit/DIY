using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    /// <summary>
    /// A Blueprint for the actions of the Undo/Redo System
    /// </summary>
    public abstract class DIYAction
    {
        /// <summary>
        /// The Name of the Action
        /// This is displayed in the edit tab
        /// </summary>
        public string Name { get; private set; }

        public DIYAction(string name)
        {
            Name = name;
        }

        /// <summary>
        /// What to do when Redo is pressed
        /// </summary>
        /// <param name="pr">The Project to redo upon</param>
        public abstract void Redo(DIYProject pr);

        /// <summary>
        /// What to do when Undo is pressed
        /// </summary>
        /// <param name="pr">The Project to undo upon</param>
        public abstract void Undo(DIYProject pr);
    }
}
