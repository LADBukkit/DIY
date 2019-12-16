using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    /// <summary>
    /// The Action for renaming a Layer
    /// </summary>
    public class RenameAction : DIYAction
    {
        /// <summary>
        /// The old name
        /// </summary>
        public string Oldname { get; set; }

        /// <summary>
        /// The new name
        /// </summary>
        public string Newname { get; set; }

        /// <summary>
        /// The Layer the changes are from
        /// </summary>
        public Layer Layer { get; set; }

        public RenameAction() : base("Rename Layer") { }

        public override void Redo(DIYProject pr)
        {
            Layer.Name = Newname;
        }

        public override void Undo(DIYProject pr)
        {
            Layer.Name = Oldname;
        }
    }
}
