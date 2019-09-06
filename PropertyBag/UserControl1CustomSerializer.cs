using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace PropertyBagTest
{
    public class UserControl1CustomSerializer : PropertyBagTest.ControlCodeDomSerializer
    {
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            var writePropertiesMethod = value.GetType().GetMethod("WriteProperties");
            if (writePropertiesMethod != null)
            {
                var propertyBag = new PropertyBag(this, manager,value);
                writePropertiesMethod.Invoke(value, new object[] { propertyBag } );
            }
            var res = base.Serialize(manager, value);

            this.AddCallInInitializeComponent(manager, value, res);


            return res;
        }

        private void AddCallInInitializeComponent(IDesignerSerializationManager manager,object control, object res)
        {
            var methodName = "ReadPropertiesFromResources";
            CodeStatementCollection statements = res as CodeStatementCollection;

            Type[] paramTypes = new Type[] { };

            MethodInfo method = TypeDescriptor.GetReflectionType(control).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, null, paramTypes, null);
            if (method != null)
            {
                CodeExpression targetObject = base.SerializeToExpression(manager, control);
                CodeMethodReferenceExpression method2 = new CodeMethodReferenceExpression(targetObject, methodName);
                CodeMethodInvokeExpression codeMethodInvokeExpression = new CodeMethodInvokeExpression();
                codeMethodInvokeExpression.Method = method2;
                //if (parameters != null)
                //{
                //    codeMethodInvokeExpression.Parameters.AddRange(parameters);
                //}
                CodeExpressionStatement codeExpressionStatement = new CodeExpressionStatement(codeMethodInvokeExpression);
     
                codeExpressionStatement.UserData["statement-ordering"] = "end";
     
                statements.Add(codeExpressionStatement);
            }
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
                    readPropertiesMethod.Invoke(res, new object[] { propertyBag });
                }
            }
            return res;
        }

    }

}
