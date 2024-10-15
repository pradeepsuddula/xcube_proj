using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace xcube_proj.Model
{
    public class MenuItem
    {
        public string Name { get; set; }
        public ICommand Command { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
