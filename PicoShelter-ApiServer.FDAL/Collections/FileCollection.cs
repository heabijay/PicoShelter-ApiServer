using PicoShelter_ApiServer.FDAL.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PicoShelter_ApiServer.FDAL.Collections
{
    public class FileCollection<T> : IFileCollection<T> where T : IFileEntity
    {
        public string BasePath { get; set; }

        public string GetFullPath(T item)
        {
            return Path.Combine(BasePath, item.Filename);
        }

        public FileCollection(string basePath)
        {
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            BasePath = basePath;
        }

        public int Count => Directory.GetFiles(BasePath).Length;

        public bool IsReadOnly => false;

        [Obsolete("Use `CreateOrUpdate()` to create a new item", error: true)]
        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public Stream CreateOrUpdate(T item)
        {
            return File.OpenWrite(GetFullPath(item));
        }

        public void Clear()
        {
            var files = Directory.GetFiles(BasePath);
            foreach (string filepath in files)
            {
                File.Delete(filepath);
            }
        }

        public bool Contains(T item)
        {
            return File.Exists(GetFullPath(item));
        }

        [Obsolete("Method is not allowed", error: true)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Method is not allowed", error: true)]
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            var fullpath = Path.Combine(BasePath, item.Filename);

            try
            {
                if (File.Exists(fullpath))
                    File.Delete(fullpath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        [Obsolete("Method is not allowed", error: true)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public Stream Get(T item)
        {
            var path = GetFullPath(item);
            if (File.Exists(path))
                return File.OpenRead(path);

            return null;
        }
    }
}
