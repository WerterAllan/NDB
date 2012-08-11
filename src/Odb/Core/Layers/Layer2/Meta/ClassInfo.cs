using System;
using System.Collections.Generic;
using System.Text;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   A meta representation of a class
    /// </summary>
    /// <author>osmadja</author>
    [Serializable]
    public class ClassInfo
    {
        /// <summary>
        ///   Constant used for the classCategory variable to indicate a system class
        /// </summary>
        public const byte CategorySystemClass = 1;

        /// <summary>
        ///   Constant used for the classCategory variable to indicate a user class
        /// </summary>
        public const byte CategoryUserClass = 2;

        /// <summary>
        ///   To keep session numbers, number of committed objects,first and last object position
        /// </summary>
        private readonly CommittedCIZoneInfo _committed;

        /// <summary>
        ///   To keep session original numbers, original number of committed objects,first and last object position
        /// </summary>
        private readonly CommittedCIZoneInfo _original;

        /// <summary>
        ///   To keep session uncommitted numbers, number of uncommitted objects,first and last object position
        /// </summary>
        private readonly CIZoneInfo _uncommitted;

        private IOdbList<ClassAttributeInfo> _attributes;

        /// <summary>
        ///   This map is redundant with the field 'attributes', but it is to enable fast access to attributes by id key=attribute Id(Integer), key =ClassAttributeInfo
        /// </summary>
        private IDictionary<int, ClassAttributeInfo> _attributesById;

        /// <summary>
        ///   This map is redundant with the field 'attributes', but it is to enable fast access to attributes by name TODO use only the map and remove list key=attribute name, key =ClassInfoattribute
        /// </summary>
        private IDictionary<string, ClassAttributeInfo> _attributesByName;

        /// <summary>
        ///   Where starts the block of attributes definition of this class ?
        /// </summary>
        private long _attributesDefinitionPosition;

        /// <summary>
        ///   The size (in bytes) of the class block
        /// </summary>
        private int _blockSize;

        /// <summary>
        ///   To specify the type of the class : system class or user class
        /// </summary>
        private byte _classCategory;

        /// <summary>
        ///   Extra info of the class - no used in java version
        /// </summary>
        private string _extraInfo;

        /// <summary>
        ///   The full class name with package
        /// </summary>
        private string _fullClassName;

        [NonSerialized]
        private IOdbList<object> _history;

        private OID _id;

        private IOdbList<ClassInfoIndex> _indexes;

        /// <summary>
        ///   Infos about the last object of this class
        /// </summary>
        private ObjectInfoHeader _lastObjectInfoHeader;

        /// <summary>
        ///   The max id is used to give a unique id for each attribute and allow refactoring like new field and/or removal
        /// </summary>
        private int _maxAttributeId;

        /// <summary>
        ///   Where is the next class, -1, if it does not exist
        /// </summary>
        private OID _nextClassOID;

        /// <summary>
        ///   Physical location of this class in the file (in byte)
        /// </summary>
        private long _position;

        /// <summary>
        ///   Where is the previous class.
        /// </summary>
        /// <remarks>
        ///   Where is the previous class. -1, if it does not exist
        /// </remarks>
        private OID _previousClassOID;

        public ClassInfo()
        {
            _original = new CommittedCIZoneInfo(this, null, null, 0);
            _committed = new CommittedCIZoneInfo(this, null, null, 0);
            _uncommitted = new CIZoneInfo(this, null, null, 0);
            _previousClassOID = null;
            _nextClassOID = null;
            _blockSize = -1;
            _position = -1;
            _maxAttributeId = -1;
            _classCategory = CategoryUserClass;
            _history = new OdbArrayList<object>();
        }

        public ClassInfo(string className) : this(className, string.Empty, null)
        {
        }

        protected ClassInfo(string fullClassName, string extraInfo, IOdbList<ClassAttributeInfo> attributes) : this()
        {
            _fullClassName = fullClassName;
            _extraInfo = extraInfo;
            _attributes = attributes;
            _attributesByName = new OdbHashMap<string, ClassAttributeInfo>();
            _attributesById = new OdbHashMap<int, ClassAttributeInfo>();

            if (attributes != null)
                FillAttributesMap();

            _maxAttributeId = (attributes == null
                                  ? 1
                                  : attributes.Count + 1);
        }

        private void FillAttributesMap()
        {
            if (_attributesByName == null)
            {
                _attributesByName = new OdbHashMap<string, ClassAttributeInfo>();
                _attributesById = new OdbHashMap<int, ClassAttributeInfo>();
            }
            // attributesMap.clear();
            foreach (var classAttributeInfo in _attributes)
            {
                _attributesByName[classAttributeInfo.GetName()] = classAttributeInfo;
                _attributesById[classAttributeInfo.GetId()] = classAttributeInfo;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof (ClassInfo))
                return false;
            var classInfo = (ClassInfo) obj;
            return classInfo._fullClassName.Equals(_fullClassName);
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();

            buffer.Append(" [ ").Append(_fullClassName).Append(" - id=").Append(_id);
            buffer.Append(" - previousClass=").Append(_previousClassOID).Append(" - nextClass=").Append(_nextClassOID).
                Append(" - attributes=(");

            // buffer.append(" | position=").append(position);
            // buffer.append(" | class=").append(className).append(" | attributes=[");

            if (_attributes != null)
            {
                foreach (var classAttributeInfo in _attributes)
                    buffer.Append(classAttributeInfo.GetName()).Append(",");
            }
            else
            {
                buffer.Append("not yet defined");
            }

            buffer.Append(") ]");

            return buffer.ToString();
        }

        public virtual IOdbList<ClassAttributeInfo> GetAttributes()
        {
            return _attributes;
        }

        public virtual void SetAttributes(IOdbList<ClassAttributeInfo> attributes)
        {
            _attributes = attributes;
            _maxAttributeId = attributes.Count;
            FillAttributesMap();
        }

        public virtual CommittedCIZoneInfo GetCommitedZoneInfo()
        {
            return _committed;
        }

        public virtual long GetAttributesDefinitionPosition()
        {
            return _attributesDefinitionPosition;
        }

        public virtual void SetAttributesDefinitionPosition(long definitionPosition)
        {
            _attributesDefinitionPosition = definitionPosition;
        }

        public virtual OID GetNextClassOID()
        {
            return _nextClassOID;
        }

        public virtual void SetNextClassOID(OID nextClassOID)
        {
            _nextClassOID = nextClassOID;
        }

        public virtual OID GetPreviousClassOID()
        {
            return _previousClassOID;
        }

        public virtual void SetPreviousClassOID(OID previousClassOID)
        {
            _previousClassOID = previousClassOID;
        }

        public virtual long GetPosition()
        {
            return _position;
        }

        public virtual void SetPosition(long position)
        {
            _position = position;
        }

        public virtual int GetBlockSize()
        {
            return _blockSize;
        }

        public virtual void SetBlockSize(int blockSize)
        {
            _blockSize = blockSize;
        }

        /// <returns> the fullClassName </returns>
        public virtual string GetFullClassName()
        {
            return _fullClassName;
        }

        /// <summary>
        ///   This method could be optimized, but it is only on Class creation, one time in the database life time...
        /// </summary>
        /// <remarks>
        ///   This method could be optimized, but it is only on Class creation, one time in the database life time... This is used to get all (non native) attributes a class info have to store them in the meta model before storing the class itself
        /// </remarks>
        /// <returns> </returns>
        public virtual IOdbList<ClassAttributeInfo> GetAllNonNativeAttributes()
        {
            IOdbList<ClassAttributeInfo> result = new OdbArrayList<ClassAttributeInfo>(_attributes.Count);
            
            foreach (var classAttributeInfo in _attributes)
            {
                if (!classAttributeInfo.IsNative() || classAttributeInfo.GetAttributeType().IsEnum())
                {
                    result.Add(classAttributeInfo);
                }
                else
                {
                    if (classAttributeInfo.GetAttributeType().IsArray() && !classAttributeInfo.GetAttributeType().GetSubType().IsNative())
                        result.Add(new ClassAttributeInfo(-1, "subtype", classAttributeInfo.GetAttributeType().GetSubType().GetName(),
                                                          null));
                }
            }

            return result;
        }

        public virtual OID GetId()
        {
            return _id;
        }

        public virtual void SetId(OID id)
        {
            _id = id;
        }

        public virtual ClassAttributeInfo GetAttributeInfoFromId(int id)
        {
            return _attributesById[id];
        }

        public virtual int GetAttributeId(string name)
        {
            var classAttributeInfo = _attributesByName[name];

            if (classAttributeInfo != null)
                return classAttributeInfo.GetId();

            var enrichedName = EnrichNameForAutoProperty(name);

            classAttributeInfo = _attributesByName[enrichedName];

            return classAttributeInfo != null
                       ? classAttributeInfo.GetId()
                       : -1;
        }

        private string EnrichNameForAutoProperty(string name)
        {
            return string.Format("<{0}>k__BackingField", name);
        }

        public virtual ClassAttributeInfo GetAttributeInfoFromName(string name)
        {
            return _attributesByName[name];
        }

        public virtual ClassAttributeInfo GetAttributeInfo(int index)
        {
            return _attributes[index];
        }

        public virtual int GetMaxAttributeId()
        {
            return _maxAttributeId;
        }

        public virtual void SetMaxAttributeId(int maxAttributeId)
        {
            _maxAttributeId = maxAttributeId;
        }

        public virtual ClassInfoCompareResult ExtractDifferences(ClassInfo newCI, bool update)
        {
            string attributeName;
            ClassAttributeInfo cai1;
            ClassAttributeInfo cai2;

            var result = new ClassInfoCompareResult(GetFullClassName());
            IOdbList<ClassAttributeInfo> attributesToRemove = new OdbArrayList<ClassAttributeInfo>(10);
            IOdbList<ClassAttributeInfo> attributesToAdd = new OdbArrayList<ClassAttributeInfo>(10);

            var attributesCount = _attributes.Count;
            for (var id = 0; id < attributesCount; id++)
            {
                // !!!WARNING : ID start with 1 and not 0
                cai1 = _attributes[id];
                if (cai1 == null)
                    continue;
                attributeName = cai1.GetName();
                cai2 = newCI.GetAttributeInfoFromId(cai1.GetId());
                if (cai2 == null)
                {
                    result.AddCompatibleChange(string.Format("Field '{0}' has been removed", attributeName));
                    if (update)
                    {
                        // Simply remove the attribute from meta-model
                        attributesToRemove.Add(cai1);
                    }
                }
                else
                {
                    if (!OdbType.TypesAreCompatible(cai1.GetAttributeType(), cai2.GetAttributeType()))
                    {
                        result.AddIncompatibleChange(
                            string.Format("Type of Field '{0}' has changed : old='{1}' - new='{2}'", attributeName,
                                          cai1.GetFullClassname(), cai2.GetFullClassname()));
                    }
                }
            }

            var nbNewAttributes = newCI._attributes.Count;
            for (var id = 0; id < nbNewAttributes; id++)
            {
                // !!!WARNING : ID start with 1 and not 0
                cai2 = newCI._attributes[id];
                if (cai2 == null)
                    continue;
                attributeName = cai2.GetName();
                cai1 = GetAttributeInfoFromId(cai2.GetId());
                if (cai1 == null)
                {
                    result.AddCompatibleChange("Field '" + attributeName + "' has been added");
                    if (update)
                    {
                        // Sets the right id of attribute
                        cai2.SetId(_maxAttributeId + 1);
                        _maxAttributeId++;
                        // Then adds the new attribute to the meta-model
                        attributesToAdd.Add(cai2);
                    }
                }
            }
            _attributes.RemoveAll(attributesToRemove);
            _attributes.AddAll(attributesToAdd);
            FillAttributesMap();
            return result;
        }

        public virtual int GetNumberOfAttributes()
        {
            return _attributes.Count;
        }

        public virtual ClassInfoIndex AddIndexOn(string name, string[] indexFields, bool acceptMultipleValuesForSameKey)
        {
            if (_indexes == null)
                _indexes = new OdbArrayList<ClassInfoIndex>();
            var cii = new ClassInfoIndex();
            cii.SetClassInfoId(_id);
            cii.SetCreationDate(OdbTime.GetCurrentTimeInTicks());
            cii.SetLastRebuild(cii.GetCreationDate());
            cii.SetName(name);
            cii.SetStatus(ClassInfoIndex.Enabled);
            cii.SetUnique(!acceptMultipleValuesForSameKey);
            var attributeIds = new int[indexFields.Length];
            for (var i = 0; i < indexFields.Length; i++)
                attributeIds[i] = GetAttributeId(indexFields[i]);
            cii.SetAttributeIds(attributeIds);
            _indexes.Add(cii);
            return cii;
        }

        /// <summary>
        ///   Removes an index
        /// </summary>
        /// <param name="cii"> </param>
        public virtual void RemoveIndex(ClassInfoIndex cii)
        {
            _indexes.Remove(cii);
        }

        public virtual int GetNumberOfIndexes()
        {
            if (_indexes == null)
                return 0;
            return _indexes.Count;
        }

        public virtual ClassInfoIndex GetIndex(int index)
        {
            if (_indexes == null || index >= _indexes.Count)
                throw new OdbRuntimeException(
                    NDatabaseError.IndexNotFound.AddParameter(GetFullClassName()).AddParameter(index));
            return _indexes[index];
        }

        public virtual void SetIndexes(IOdbList<ClassInfoIndex> indexes2)
        {
            _indexes = indexes2;
        }

        /// <summary>
        ///   To detect if a class has cyclic reference
        /// </summary>
        /// <returns> true if this class info has cyclic references </returns>
        public virtual bool HasCyclicReference()
        {
            return HasCyclicReference(new OdbHashMap<string, ClassInfo>());
        }

        /// <summary>
        ///   To detect if a class has cyclic reference
        /// </summary>
        /// <param name="alreadyVisitedClasses"> A hashmap containg all the already visited classes </param>
        /// <returns> true if this class info has cyclic references </returns>
        private bool HasCyclicReference(IDictionary<string, ClassInfo> alreadyVisitedClasses)
        {
            if (alreadyVisitedClasses[_fullClassName] != null)
                return true;

            alreadyVisitedClasses.Add(_fullClassName, this);
            
            for (var i = 0; i < _attributes.Count; i++)
            {
                var classAttributeInfo = GetAttributeInfo(i);
                if (!classAttributeInfo.IsNative())
                {
                    IDictionary<string, ClassInfo> localMap = new OdbHashMap<string, ClassInfo>(alreadyVisitedClasses);
                    var hasCyclicRef = classAttributeInfo.GetClassInfo().HasCyclicReference(localMap);
                    if (hasCyclicRef)
                        return true;
                }
            }
            return false;
        }

        public virtual byte GetClassCategory()
        {
            return _classCategory;
        }

        public virtual void SetClassCategory(byte classInfoType)
        {
            _classCategory = classInfoType;
        }

        public virtual ObjectInfoHeader GetLastObjectInfoHeader()
        {
            return _lastObjectInfoHeader;
        }

        public virtual void SetLastObjectInfoHeader(ObjectInfoHeader lastObjectInfoHeader)
        {
            _lastObjectInfoHeader = lastObjectInfoHeader;
        }

        public virtual CIZoneInfo GetUncommittedZoneInfo()
        {
            return _uncommitted;
        }

        /// <summary>
        ///   Get number of objects: committed and uncommitted
        /// </summary>
        /// <returns> The number of committed and uncommitted objects </returns>
        public virtual long GetNumberOfObjects()
        {
            return _committed.GetNbObjects() + _uncommitted.GetNbObjects();
        }

        public virtual CommittedCIZoneInfo GetOriginalZoneInfo()
        {
            return _original;
        }

        public virtual bool IsSystemClass()
        {
            return _classCategory == CategorySystemClass;
        }

        public virtual ClassInfoIndex GetIndexWithName(string name)
        {
            if (_indexes == null)
                return null;
            
            for (var i = 0; i < _indexes.Count; i++)
            {
                var classInfoIndex = _indexes[i];
                if (classInfoIndex.GetName().Equals(name))
                    return classInfoIndex;
            }
            return null;
        }

        public virtual ClassInfoIndex GetIndexForAttributeId(int attributeId)
        {
            if (_indexes == null)
                return null;
            for (var i = 0; i < _indexes.Count; i++)
            {
                var classInfoIndex = _indexes[i];
                if (classInfoIndex.GetAttributeIds().Length == 1 && classInfoIndex.GetAttributeId(0) == attributeId)
                    return classInfoIndex;
            }
            return null;
        }

        public virtual ClassInfoIndex GetIndexForAttributeIds(int[] attributeIds)
        {
            if (_indexes == null)
                return null;
            for (var i = 0; i < _indexes.Count; i++)
            {
                var classInfoIndex = _indexes[i];
                if (classInfoIndex.MatchAttributeIds(attributeIds))
                    return classInfoIndex;
            }
            return null;
        }

        public virtual string[] GetAttributeNames(int[] attributeIds)
        {
            var attributeIdsLength = attributeIds.Length;
            var names = new string[attributeIdsLength];
            
            for (var i = 0; i < attributeIdsLength; i++)
                names[i] = GetAttributeInfoFromId(attributeIds[i]).GetName();

            return names;
        }

        public virtual IList<string> GetAttributeNamesAsList(int[] attributeIds)
        {
            var nbIds = attributeIds.Length;

            IList<string> names = new List<string>(attributeIds.Length);
            for (var i = 0; i < nbIds; i++)
                names.Add(GetAttributeInfoFromId(attributeIds[i]).GetName());

            return names;
        }

        public virtual IOdbList<ClassInfoIndex> GetIndexes()
        {
            if (_indexes == null)
                return new OdbArrayList<ClassInfoIndex>();
            return _indexes;
        }

        public virtual void RemoveAttribute(ClassAttributeInfo cai)
        {
            _attributes.Remove(cai);
            _attributesByName.Remove(cai.GetName());
        }

        public virtual void AddAttribute(ClassAttributeInfo cai)
        {
            cai.SetId(_maxAttributeId++);
            _attributes.Add(cai);
            _attributesByName.Add(cai.GetName(), cai);
        }

        public virtual void SetFullClassName(string fullClassName)
        {
            _fullClassName = fullClassName;
        }

        public virtual void AddHistory(object o)
        {
            if (_history == null)
                _history = new OdbArrayList<object>(1);
            _history.Add(o);
        }

        public virtual IOdbList<object> GetHistory()
        {
            return _history;
        }

        public virtual bool HasIndex(string indexName)
        {
            if (_indexes == null)
                return false;
            
            for (var i = 0; i < _indexes.Count; i++)
            {
                var classInfoIndex = _indexes[i];
                if (indexName.Equals(classInfoIndex.GetName()))
                    return true;
            }

            return false;
        }

        public virtual bool HasIndex()
        {
            return _indexes != null && !_indexes.IsEmpty();
        }

        public virtual void SetExtraInfo(string extraInfo)
        {
            _extraInfo = extraInfo;
        }

        public virtual string GetExtraInfo()
        {
            return _extraInfo;
        }

        public virtual ClassInfo Duplicate(bool onlyData)
        {
            var ci = new ClassInfo(_fullClassName) {_extraInfo = _extraInfo};

            ci.SetAttributes(_attributes);
            ci.SetClassCategory(_classCategory);
            ci.SetMaxAttributeId(_maxAttributeId);
            
            if (onlyData)
                return ci;

            ci.SetAttributesDefinitionPosition(_attributesDefinitionPosition);
            ci.SetBlockSize(_blockSize);
            ci.SetExtraInfo(_extraInfo);
            ci.SetId(_id);
            ci.SetPreviousClassOID(_previousClassOID);
            ci.SetNextClassOID(_nextClassOID);
            ci.SetLastObjectInfoHeader(_lastObjectInfoHeader);
            ci.SetPosition(_position);
            ci.SetIndexes(_indexes);

            return ci;
        }
    }
}
