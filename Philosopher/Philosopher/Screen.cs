using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Philosopher
{
    abstract class Screen
    {
        public abstract void Update();

        public abstract void Render();
    }
}
