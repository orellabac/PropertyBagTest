using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;

namespace PropertyBagTest
{
    [DesignerSerializer(typeof(UserControl1CustomSerializer),typeof(CodeDomSerializer))]
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public void ReadProperties(PropertyBag propertyBag)
        {

        }

        public void WriteProperties(PropertyBag propertyBag)
        {

        }
    }

    public class UserControl1CustomSerializer : PropertyBagTest.ControlCodeDomSerializer
    {
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            var writePropertiesMethod = value.GetType().GetMethod("WriteProperties", new System.Type[] { typeof(PropertyBag) });
            if (writePropertiesMethod != null)
            {
                var propertyBag = new PropertyBag(this, manager);
                writePropertiesMethod.Invoke(value, new object[] { propertyBag } );
            }
            System.Diagnostics.Debugger.Launch();
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
                    var propertyBag = new PropertyBag(this, manager);
                    readPropertiesMethod.Invoke(res, new object[] { propertyBag });
                }
            }
            return res;
        }

        private static ComponentResourceManager FindResources(IDesignerSerializationManager manager)
        {
            ComponentResourceManager resources = null;
            for (int i = 0; i < 10; i++)
            {
                var possibleResources = manager.Context[i];
                if (possibleResources == null)
                {
                    break;
                }
                else
                {
                    if (possibleResources is ComponentResourceManager)
                    {
                        resources = null;
                    }
                }
            }

            return resources;
        }
    }

    public class PropertyBag
    {
        private ControlCodeDomSerializer serializer;
        private object instance;

        public PropertyBag(PropertyBagTest.ControlCodeDomSerializer serializer, object instance)
        {
            this.serializer = serializer;
            this.instance = instance;
        }


        public object ReadProperty(string propertyName, object defaultValue)
        {
            var propInfo = instance.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (propInfo != null)
            {
            }
            return defaultValue;
        }

        public void WriteProperty(string propertyName, object value, object defaultValue)
        {

        }
    }

}
