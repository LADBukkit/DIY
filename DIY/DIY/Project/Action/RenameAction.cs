using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    public class RenameAction : DIYAction
    {
        public string Oldname { get; set; }
        public string Newname { get; set; }
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
