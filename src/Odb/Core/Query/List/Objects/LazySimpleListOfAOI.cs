using System;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Query.List.Objects
{
    /// <summary>
    ///   A simple list to hold query result.
    /// </summary>
    /// <remarks>
    ///   A simple list to hold query result. It is used when no index and no order by This collection does not store the objects, it only holds the Abstract Object Info (AOI) of the objects. When user ask an object the object is lazy loaded by the buildInstance method
    /// </remarks>
    internal sealed class LazySimpleListOfAoi<T> : OdbList<T>, IObjectSet<T>
    {
        /// <summary>
        ///   The odb engine to lazily get objects
        /// </summary>
        [NonPersistent] private readonly IInstanceBuilder _instanceBuilder;

        /// <summary>
        ///   indicate if objects must be returned as instance (true) or as non native objects (false)
        /// </summary>
        private readonly bool _returnInstance;

        /// <summary>
        ///   a cursor when getting objects
        /// </summary>
        private int _currentPosition;

        public LazySimpleListOfAoi(IInstanceBuilder builder, bool returnInstance) : base(10)
        {
            if (builder == null)
                throw new OdbRuntimeException(
                    NDatabaseError.InternalError.AddParameter("instance builder cannot be null"));

            _instanceBuilder = builder;
            _returnInstance = returnInstance;
        }

        #region IObjectSet<T> Members

        public T GetFirst()
        {
            try
            {
                return this[0];
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.ErrorWhileGettingObjectFromListAtIndex.AddParameter(0), e);
            }
        }

        public bool HasNext()
        {
            return _currentPosition < Count;
        }

        public T Next()
        {
            try
            {
                return this[_currentPosition++];
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.ErrorWhileGettingObjectFromListAtIndex.AddParameter(0), e);
            }
        }

        public void Reset()
        {
            _currentPosition = 0;
        }

        #endregion

        public T Get(int index)
        {
            object o = base[index];
            var aoi = (AbstractObjectInfo) o;
            try
            {
                if (aoi.IsNull())
                    return default(T);

                if (_returnInstance)
                {
                    if (aoi.IsNative())
                        return (T) aoi.GetObject();

                    return (T) _instanceBuilder.BuildOneInstance((NonNativeObjectInfo) aoi);
                }
                // No need to return Instance return the layer 2 representation
                o = aoi;
                return (T) o;
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.ErrorWhileGettingObjectFromListAtIndex.AddParameter(index),
                                              e);
            }
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("list with ").Append(Count).Append(" elements");
            return buffer.ToString();
        }
    }
}