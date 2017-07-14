using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GitSolutions
{
    enum CloningMode : int
    {
        Deep,
        Shallow,
        Ignore
    }

    interface ICloningService
    {
        T Clone<T>(T source);
        object Clone(object source);
    }

    class CloneableAttribute : Attribute
    {
        internal CloningMode Mode { get; }

        public CloneableAttribute()
        {
            Mode = CloningMode.Deep;
        }

        public CloneableAttribute(CloningMode type)
        {

            switch (type)
            {
                case CloningMode.Ignore:
                    Mode = CloningMode.Ignore;
                    break;

                case CloningMode.Shallow:
                    Mode = CloningMode.Shallow;
                    break;

                case CloningMode.Deep:
                    Mode = CloningMode.Deep;
                    break;
            }
        }
    }

    public class CloningService : ICloningService
    {
        public object Clone(object source)
        {
            object clone = source;
            foreach (PropertyInfo item in source.GetType().GetProperties())
            {
                CloneableAttribute[] cloneAttr = (CloneableAttribute[])item.GetCustomAttributes(typeof(CloneableAttribute), true);

                CloneableAttribute attr = null;
                if (cloneAttr.Length != 1) { attr = new CloneableAttribute(CloningMode.Deep); }
                else { attr = cloneAttr[0]; };

                switch (attr.Mode)
                {
                    case CloningMode.Ignore:
                        PropertyInfo prop = clone.GetType().GetProperty(item.Name);
                        if (prop.GetType().IsValueType)
                        {
                            prop.SetValue(clone, 0);
                        }
                        else
                        {
                            prop.SetValue(clone, null);
                        }
                        break;

                    case CloningMode.Shallow:
                        break;

                    case CloningMode.Deep:
                        PropertyInfo deepProp = clone.GetType().GetProperty(item.Name);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            if (source.GetType().IsSerializable)
                            {
                                BinaryFormatter formatter = new BinaryFormatter();
                                formatter.Serialize(stream, this);
                                stream.Position = 0;
                                deepProp.SetValue(clone, formatter.Deserialize(stream));
                            }
                        }
                        break;
                }
            }

            return clone;
        }

        public T Clone<T>(T source)
        {
            T clone = Activator.CreateInstance<T>();
            foreach (PropertyInfo item in source.GetType().GetProperties())
            {
                CloneableAttribute[] cloneAttr = (CloneableAttribute[])item.GetCustomAttributes(typeof(CloneableAttribute), true);

                CloneableAttribute attr = null;
                if (cloneAttr.Length != 1) { attr = new CloneableAttribute(CloningMode.Deep); }
                else { attr = cloneAttr[0]; };
                if (item.GetValue(source) == null) { attr = new CloneableAttribute(CloningMode.Ignore); };

                switch (attr.Mode)
                {
                    case CloningMode.Ignore:
                        PropertyInfo prop = clone.GetType().GetProperty(item.Name);
                        prop.SetValue(clone, null);
                        break;

                    case CloningMode.Shallow:
                        PropertyInfo shallowProp = clone.GetType().GetProperty(item.Name);
                        shallowProp.SetValue(clone, item.GetValue(source));
                        break;

                    case CloningMode.Deep:
                        PropertyInfo deepProp = clone.GetType().GetProperty(item.Name);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(stream, item.GetValue(source));
                            stream.Seek(0, SeekOrigin.Begin);
                            deepProp.SetValue(clone, formatter.Deserialize(stream));
                            stream.Close();
                        }
                        break;
                }
            }

            return clone;
        }
    }

    public class TestClass<T> where T : new()
    {
        [Cloneable(CloningMode.Shallow)]
        public char @Char { get; set; }

        public string @String { get; set; }

        [Cloneable(CloningMode.Ignore)]
        public Int16 @Int16 { get; set; }

        [Cloneable(CloningMode.Shallow)]
        public Int32 @Int32 { get; set; }

        public object @Object { get; set; }

        public ValueType @ValueType { get; set; }

        public T @Class { get; set; }

        public List<T> @List { get; set; }

    }


}
