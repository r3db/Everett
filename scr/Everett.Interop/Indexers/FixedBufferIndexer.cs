using System;

namespace Everett.Interop
{
    // Todo: Test!
    public class FixedBufferIndexer
    {
        // Internal Instance Data
        private readonly unsafe IntPtr* _address;

        // .Ctor
        public unsafe FixedBufferIndexer(IntPtr address)
        {
            _address = &address;
        }

        public unsafe FixedBufferIndexer(int address)
        {
            _address = (IntPtr*)&address;
        }

        // Indexers
        public unsafe IntPtr this[int index]
        {
            get { return *(_address + index); }
            set { *(_address + index) = value; }
        }
    }
}