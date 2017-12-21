using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Input
{
    /// <summary>
    /// Represents a keyboard key and modifiers combined.
    /// </summary>
    public struct KeySpec : IComparable<KeySpec>
    {
        public Key Key { get; set; }
        public KeyModifiers Modifiers { get; set; }

        public KeySpec(Key _key, KeyModifiers _modifiers)
        {
            Key = _key;
            Modifiers = _modifiers;
        }

        public int CompareTo(KeySpec other)
        {
            return Pack() - other.Pack();
        }

        public int Pack()
        {
            return ((int)Key << 8) | (int)Modifiers;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is KeySpec)) return false;
            var o = (KeySpec)obj;
            return Key.Equals(o.Key) && Modifiers.Equals(o.Modifiers);
        }

        public override int GetHashCode()
        {
            return Pack().GetHashCode();
        }

        public override string ToString()
        {
            return $"{((Modifiers & KeyModifiers.Alt) > 0 ? "Alt-" : "")}{((Modifiers & KeyModifiers.Control) > 0 ? "Ctrl-" : "")}{((Modifiers & KeyModifiers.Shift) > 0 ? "Shift-" : "")}{Key.ToString()}";
        }
    }
}
