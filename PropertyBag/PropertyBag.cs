using System.ComponentModel.Design.Serialization;
using System.ComponentModel;

namespace PropertyBagTest
{
    public class PropertyBag
    {
        private ControlCodeDomSerializer serializer;
        private IDesignerSerializationManager manager;
        private object instance;
        private string instanceName;
        private ComponentResourceManager resources;

        public PropertyBag(PropertyBagTest.ControlCodeDomSerializer serializer, IDesignerSerializationManager manager, object instance)
        {
            this.serializer = serializer;
            this.manager = manager;
            this.instance = instance;
            this.instanceName = serializer.CustomGetUniqueName(manager, instance) ?? "undefined";
        }


        public PropertyBag(System.ComponentModel.ComponentResourceManager resources, string name)
        {
           this.instanceName = name;
           this.resources = resources;
        }


        public object ReadProperty(string propertyName, object defaultValue)
        {
            //var propInfo = instance.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            //if (propInfo != null)
            //{

            //}
            if (this.resources != null)
            {
                try
                {
                    return this.resources.GetObject($"{this.instanceName}.{propertyName}");
                }
                catch
                {
                    return defaultValue; 
                }
            }
            else 
                return this.serializer.CustomDeSerializeResource(manager, propertyName, this.instance) ?? defaultValue;
        }

        public void WriteProperty(string propertyName, object value, object defaultValue)
        {
            this.serializer.CustomSerializeResource(this.manager, $"{this.instanceName}.{propertyName}", value ?? defaultValue);
        }
    }

}
