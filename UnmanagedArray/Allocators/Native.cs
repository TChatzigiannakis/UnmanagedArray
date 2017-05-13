using System;

namespace UnmanagedArray.Allocators
{
    public struct Native
    {
        public IntPtr Value { get; }

        public Native(IntPtr value)
        {
            Value = value;
        }

        public Native(int value)
            : this(new IntPtr(value))
        {
        }

        public Native(long value)
            : this(new IntPtr(value))
        {
        }

        public static implicit operator Native(int value) => new Native(value);
        public static implicit operator Native(long value) => new Native(value);
    }
}
