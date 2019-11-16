using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    public abstract class DIYAction
    {
        public string Name { get; private set; }

        public DIYAction(string name)
        {
            Name = name;
        }

        public abstract void Redo(DIYProject pr);

        public abstract void Undo(DIYProject pr);
    }
}
