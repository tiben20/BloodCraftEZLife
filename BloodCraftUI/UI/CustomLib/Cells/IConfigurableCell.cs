using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodmoonUI.UI.CustomLib.Cells
{
    public interface IConfigurableCell<T>
    {
        void SetValue(T value);
        T GetValue();
        void SetLabel(string label);
        event Action<T,int> OnValueChanged;
        int GetIndex();
        void SetIndex(int value);
    }
}
