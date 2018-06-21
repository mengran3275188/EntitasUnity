using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    public interface IData
    {
        bool CollectDataFromDBC(DBC_Row node);
        int GetId();
    }
    public interface IData2
    {
        bool CollectDataFromDBC(DBC_Row node);
        bool CollectDataFromBinary(BinaryTable table, int index);
        void AddToBinary(BinaryTable table);
        int GetId();
    }
}
