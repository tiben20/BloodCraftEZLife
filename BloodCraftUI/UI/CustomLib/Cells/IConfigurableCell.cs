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

        event Action<T> OnValueChanged;
        event Action<T> OnInputBox;
    }
}
