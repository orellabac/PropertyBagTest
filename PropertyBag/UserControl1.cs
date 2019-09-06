using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
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
        }

        public void ReadProperties(PropertyBag propertyBag)
        {
            var x = Convert.ToInt32( propertyBag.ReadProperty("MyCuteProperty1", 10));
            var y = Convert.ToInt32( propertyBag.ReadProperty("MyCuteProperty2", 10));
            this.MyProperty = x + y;
        }

        public void WriteProperties(PropertyBag propertyBag)
        {
            propertyBag.WriteProperty("MyCuteProperty1", 100, 0);
            propertyBag.WriteProperty("MyCuteProperty2", 100, 0);
        }

        public void ReadPropertiesFromResources()
        {
            if (this.Parent != null && !DesignMode)
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(this.Parent.GetType());
                var propertyBag = new PropertyBag(resources, this.Name);
                this.ReadProperties(propertyBag);
            }
        }
    }

}
