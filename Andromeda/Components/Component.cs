using Andromeda.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andromeda.Components
{
    public class Component : ViewModelBase
    {
        public GameEntity Owner { get; private set; }

        public Component(GameEntity entity)
        {
            Debug.Assert(entity != null);
            Owner = entity;
        }
    }
}
