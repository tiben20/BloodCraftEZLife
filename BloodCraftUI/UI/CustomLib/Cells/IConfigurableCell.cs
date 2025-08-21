using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodCraftEZLife.UI.CustomLib.Cells
{
    public interface IConfigurableCell<T>
    {
        void SetValue(T value);
        T GetValue();
        void SetLabel(string label);
        event Action<T> OnValueChanged;
    }
}
