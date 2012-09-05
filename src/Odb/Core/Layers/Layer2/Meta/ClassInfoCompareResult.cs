using System.Text;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   To keep track of differences between two ClassInfo.
    /// </summary>
    /// <remarks>
    ///   To keep track of differences between two ClassInfo. Ussed by the MetaModel compatibility checker
    /// </remarks>
    /// <author>osmadja</author>
    
    public sealed class ClassInfoCompareResult
    {
        private readonly string _fullClassName;

        private IOdbList<string> _compatibleChanges;
        private IOdbList<string> _incompatibleChanges;

        public ClassInfoCompareResult(string fullClassName)
        {
            _fullClassName = fullClassName;
            _incompatibleChanges = new OdbList<string>(5);
            _compatibleChanges = new OdbList<string>(5);
        }

        /// <returns> the compatibleChanges </returns>
        public IOdbList<string> GetCompatibleChanges()
        {
            return _compatibleChanges;
        }

        /// <param name="compatibleChanges"> the compatibleChanges to set </param>
        public void SetCompatibleChanges(IOdbList<string> compatibleChanges)
        {
            _compatibleChanges = compatibleChanges;
        }

        /// <returns> the incompatibleChanges </returns>
        public IOdbList<string> GetIncompatibleChanges()
        {
            return _incompatibleChanges;
        }

        /// <param name="incompatibleChanges"> the incompatibleChanges to set </param>
        public void SetIncompatibleChanges(IOdbList<string> incompatibleChanges)
        {
            _incompatibleChanges = incompatibleChanges;
        }

        /// <returns> the isCompatible </returns>
        public bool IsCompatible()
        {
            return _incompatibleChanges.IsEmpty();
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();

            buffer.Append(_fullClassName).Append(" is Compatible = ").Append(IsCompatible()).Append("\n");
            buffer.Append("Incompatible changes = ").Append(_incompatibleChanges);
            buffer.Append("\nCompatible changes = ").Append(_compatibleChanges);

            return buffer.ToString();
        }

        public void AddCompatibleChange(string o)
        {
            _compatibleChanges.Add(o);
        }

        public void AddIncompatibleChange(string o)
        {
            _incompatibleChanges.Add(o);
        }

        public bool HasCompatibleChanges()
        {
            return !_compatibleChanges.IsEmpty();
        }

        /// <returns> the fullClassName </returns>
        public string GetFullClassName()
        {
            return _fullClassName;
        }
    }
}
