using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoonFactory
{
    public interface IYoonObject : IDisposable
    {
        int Label { get; set; }

        bool Equals(IYoonObject pObject);
        void CopyFrom(IYoonObject pObject);
        IYoonObject Clone();
    }
}
