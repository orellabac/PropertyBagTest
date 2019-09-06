using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System;

namespace PropertyBagTest
{
    [DesignerSerializer(typeof(UserControl1CustomSerializer),typeof(CodeDomSerializer))]
    public partial class UserControl1 : UserControl
    {
        public int MyProperty { get; set; }

        public UserControl1()
        {
            InitializeComponent();
            //this.ApplyPropertyBag();
        }

        private void ApplyPropertyBag()
        {
            if (!this.DesignMode)
            {
                var propertyBag = new PropertyBag(this);
                this.ReadProperties(propertyBag);
            }
        }

        public void ReadProperties(PropertyBag propertyBag)
        {
            var x = (int)propertyBag.ReadProperty("MyCuteProperty1", 10);
            var y = (int)propertyBag.ReadProperty("MyCuteProperty2", 10);
            this.MyProperty = x + y;
        }

        public void WriteProperties(PropertyBag propertyBag)
        {
            propertyBag.WriteProperty("MyCuteProperty1", 100, 0);
            propertyBag.WriteProperty("MyCuteProperty2", 100, 0);
        }
    }

    public class UserControl1CustomSerializer : PropertyBagTest.ControlCodeDomSerializer
    {
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            var writePropertiesMethod = value.GetType().GetMethod("WriteProperties", new System.Type[] { typeof(PropertyBag) });
            if (writePropertiesMethod != null)
            {
                var propertyBag = new PropertyBag(this, manager,value);
                writePropertiesMethod.Invoke(value, new object[] { propertyBag } );
            }
            return base.Serialize(manager, value);
        }

        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            var res = base.Deserialize(manager, codeObject);

            var readPropertiesMethod = res.GetType().GetMethod("ReadProperties", new System.Type[] { typeof(PropertyBag) });
            if (readPropertiesMethod != null)
            {
                var resources = FindResources(manager);
                if (resources != null)
                {
                    var propertyBag = new PropertyBag(this, manager, res);
                    try
                    {
                        readPropertiesMethod.Invoke(res, new object[] { propertyBag });
                    }
                    catch
                    {

                    }
                }
            }
            return res;
        }

    }

    public class PropertyBag
    {
        private ControlCodeDomSerializer serializer;
        private IDesignerSerializationManager manager;
        private object instance;
        private ComponentResourceManager resources;

        public PropertyBag(PropertyBagTest.ControlCodeDomSerializer serializer, IDesignerSerializationManager manager, object instance)
        {
            this.serializer = serializer;
            this.manager = manager;
            this.instance = instance;
        }


        public PropertyBag(object instance)
        {
           this.resources = new System.ComponentModel.ComponentResourceManager(instance.GetType());
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
                    return this.resources.GetObject(propertyName);
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
            this.serializer.CustomSerializeResource(this.manager, propertyName, value ?? defaultValue);
        }
    }

}
