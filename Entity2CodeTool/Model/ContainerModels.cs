using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool
{
    public class ContainerModels : IList<ContainerModel>, ICollection<ContainerModel>, IEnumerable<ContainerModel>, IEnumerable
    {
        private IList<ContainerModel> _models;

        public ContainerModels()
        {
            _models = new List<ContainerModel>();
        }

        public int IndexOf(ContainerModel item)
        {
            return _models.IndexOf(item);
        }

        public void Insert(int index, ContainerModel item)
        {
            _models.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _models.RemoveAt(index);
        }

        public ContainerModel this[int index]
        {
            get
            {
                return _models[index];
            }
            set
            {
                _models[index] = value;
            }
        }

        public ContainerModel this[string key]
        {
            get
            {
                if (_models != null && _models.Count > 0)
                    foreach (ContainerModel model in _models)
                    {
                        if (String.Compare(model.Key, key, false) == 0)
                            return model;
                    }
                return null;
            }
            set
            {
                if (_models != null && _models.Count > 0)
                    for (int i = 0; i < _models.Count; i++)
                    {
                        if (String.Compare(_models[i].Key, key, false) == 0)
                        {
                            _models[i] = value;
                            break;
                        }
                    }
            }
        }

        public void Add(ContainerModel item)
        {
            _models.Add(item);
        }

        public void Add(string key, object value, string comment, DateTime datetime, int modelType)
        {
            _models.Add(new ContainerModel() { Key = key, Comment = comment, Value = value, ModelType = modelType, LastModifyTime = datetime });
        }

        public void AddRange(ContainerModels models)
        {
            if (models != null && models.Count > 0)
                foreach (ContainerModel item in models)
                {
                    _models.Add(item);
                }
        }

        public void Clear()
        {
            _models.Clear();
        }

        public bool Contains(ContainerModel item)
        {
            return _models.Contains(item);
        }

        public bool Contains(string key)
        {
            if (_models != null && _models.Count > 0)
                foreach (ContainerModel model in _models)
                {
                    if (String.Compare(model.Key, key, false) == 0)
                        return true;
                }
            return false;
        }

        public void CopyTo(ContainerModel[] array, int arrayIndex)
        {
            _models.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _models.Count; }
        }

        public bool IsReadOnly
        {
            get { return _models.IsReadOnly; }
        }

        public bool Remove(ContainerModel item)
        {
            return _models.Remove(item);
        }

        public IEnumerator<ContainerModel> GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _models.GetEnumerator();
        }
    }
}
