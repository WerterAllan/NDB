using System;
using NDatabase.Btree;
using NDatabase.Btree.Impl.Singlevalue;

namespace NDatabase.Odb.Impl.Core.Btree
{
    /// <summary>
    ///   The NDatabase ODB BTree.
    /// </summary>
    /// <remarks>
    ///   The NDatabase ODB BTree. It extends the DefaultBTree implementation to add the ODB OID generated by the ODB database.
    /// </remarks>
    /// <author>osmadja</author>
    [Serializable]
    public class OdbBtreeSingle : BTreeSingleValuePerKey
    {
        private OID _oid;

        public OdbBtreeSingle()
        {
            //only for deserialization purposes
        }

        public OdbBtreeSingle(string name, int degree, IBTreePersister persister) : base(name, degree, persister)
        {
        }

        public override IBTreeNode BuildNode()
        {
            return new OdbBtreeNodeSingle(this);
        }

        public override object GetId()
        {
            return _oid;
        }

        public override void SetId(object id)
        {
            _oid = (OID) id;
        }
    }
}
