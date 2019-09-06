using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using System.Reflection;
using System.ComponentModel.Design;
using System.Collections;
using System.Windows.Forms.Design;
using System.Globalization;

namespace PropertyBagTest
{
    public class ControlCodeDomSerializer : CodeDomSerializer
    {

        public ComponentResourceManager FindResources(IDesignerSerializationManager manager)
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
                        resources = (ComponentResourceManager)possibleResources;
                    }
                }
            }

            return resources;
        }


        public void CustomSerializeResource(IDesignerSerializationManager manager, string resourceName, object value)
        {
            base.SerializeResource(manager, resourceName, value);
        }

        public object CustomDeSerializeResource(IDesignerSerializationManager manager, string resourceName, object value)
        {
            var resources = this.FindResources(manager);
            return resources.GetObject(resourceName);
            //ResourceCodeDomSerializer.Default.WriteResource(manager, resourceName, value);
        }


        private enum StatementOrdering
        {
            Prepend,
            Append
        }

        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            if (manager == null || codeObject == null)
            {
                throw new ArgumentNullException((manager == null) ? "manager" : "codeObject");
            }
            IContainer container = (IContainer)manager.GetService(typeof(IContainer));
            ArrayList arrayList = null;
            if (container != null)
            {
                arrayList = new ArrayList(container.Components.Count);
                foreach (IComponent component in container.Components)
                {
                    Control control = component as Control;
                    if (control != null)
                    {
                        control.SuspendLayout();
                        arrayList.Add(control);
                    }
                }
            }
            object result = null;
            try
            {
                CodeDomSerializer codeDomSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));
                if (codeDomSerializer == null)
                {
                    return null;
                }
                result = codeDomSerializer.Deserialize(manager, codeObject);
            }
            finally
            {
                if (arrayList != null)
                {
                    foreach (Control control2 in arrayList)
                    {
                        control2.ResumeLayout(true);
                    }
                }
            }
            return result;
        }

        private bool HasAutoSizedChildren(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control.AutoSize)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasMixedInheritedChildren(Control parent)
        {
            bool flag = false;
            bool flag2 = false;
            foreach (Control component in parent.Controls)
            {
                InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(component)[typeof(InheritanceAttribute)];
                if (inheritanceAttribute != null && inheritanceAttribute.InheritanceLevel != InheritanceLevel.NotInherited)
                {
                    flag = true;
                }
                else
                {
                    flag2 = true;
                }
                if (flag & flag2)
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual bool HasSitedNonReadonlyChildren(Control parent)
        {
            if (!parent.HasChildren)
            {
                return false;
            }
            foreach (Control control in parent.Controls)
            {
                if (control.Site != null && control.Site.DesignMode)
                {
                    InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(control)[typeof(InheritanceAttribute)];
                    if (inheritanceAttribute != null && inheritanceAttribute.InheritanceLevel != InheritanceLevel.InheritedReadOnly)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            if (manager == null || value == null)
            {
                throw new ArgumentNullException((manager == null) ? "manager" : "value");
            }
            CodeDomSerializer codeDomSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));
            if (codeDomSerializer == null)
            {
                return null;
            }
            object obj = codeDomSerializer.Serialize(manager, value);
            InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(value)[typeof(InheritanceAttribute)];
            InheritanceLevel inheritanceLevel = InheritanceLevel.NotInherited;
            if (inheritanceAttribute != null)
            {
                inheritanceLevel = inheritanceAttribute.InheritanceLevel;
            }
            if (inheritanceLevel != InheritanceLevel.InheritedReadOnly)
            {
                IDesignerHost designerHost = (IDesignerHost)manager.GetService(typeof(IDesignerHost));
                if (designerHost != null)
                {
                    PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(designerHost.RootComponent)["Localizable"];
                    if (propertyDescriptor != null && propertyDescriptor.PropertyType == typeof(bool) && (bool)propertyDescriptor.GetValue(designerHost.RootComponent))
                    {
                        this.SerializeControlHierarchy(manager, designerHost, value);
                    }
                }
                CodeStatementCollection codeStatementCollection = obj as CodeStatementCollection;
                if (codeStatementCollection != null)
                {
                    Control control = (Control)value;
                    if ((designerHost != null && control == designerHost.RootComponent) || this.HasSitedNonReadonlyChildren(control))
                    {
                        this.SerializeSuspendLayout(manager, codeStatementCollection, value);
                        this.SerializeResumeLayout(manager, codeStatementCollection, value);
                        ControlDesigner controlDesigner = designerHost.GetDesigner(control) as ControlDesigner;
                        var serializePerformLayoutProp = typeof(ControlDesigner).GetProperty("SerializePerformLayout", BindingFlags.NonPublic | BindingFlags.Instance);
                        
                        if (this.HasAutoSizedChildren(control) || (controlDesigner != null && (bool)serializePerformLayoutProp.GetValue(controlDesigner)))
                        {
                            this.SerializePerformLayout(manager, codeStatementCollection, value);
                        }
                    }
                    if (this.HasMixedInheritedChildren(control))
                    {
                        this.SerializeZOrder(manager, codeStatementCollection, control);
                    }
                }
            }
            return obj;
        }

        private void SerializeControlHierarchy(IDesignerSerializationManager manager, IDesignerHost host, object value)
        {
            Control control = value as Control;
            if (control != null)
            {
                IMultitargetHelperService multitargetHelperService = host.GetService(typeof(IMultitargetHelperService)) as IMultitargetHelperService;
                string text;
                if (control == host.RootComponent)
                {
                    text = "$this";
                    IEnumerator enumerator = host.Container.Components.GetEnumerator();
                    {
                        while (enumerator.MoveNext())
                        {
                            IComponent component = (IComponent)enumerator.Current;
                            if (!(component is Control) && !TypeDescriptor.GetAttributes(component).Contains(InheritanceAttribute.InheritedReadOnly))
                            {
                                string name = manager.GetName(component);
                                string value2 = (multitargetHelperService == null) ? component.GetType().AssemblyQualifiedName : multitargetHelperService.GetAssemblyQualifiedName(component.GetType());
                                if (name != null)
                                {
                                    base.SerializeResourceInvariant(manager, ">>" + name + ".Name", name);
                                    base.SerializeResourceInvariant(manager, ">>" + name + ".Type", value2);
                                }
                            }
                        }
                        goto IL_107;
                    }
                }
                text = manager.GetName(value);
                if (text == null)
                {
                    return;
                }
            IL_107:
                base.SerializeResourceInvariant(manager, ">>" + text + ".Name", manager.GetName(value));
                base.SerializeResourceInvariant(manager, ">>" + text + ".Type", (multitargetHelperService == null) ? control.GetType().AssemblyQualifiedName : multitargetHelperService.GetAssemblyQualifiedName(control.GetType()));
                Control parent = control.Parent;
                if (parent != null && parent.Site != null)
                {
                    string text2;
                    if (parent == host.RootComponent)
                    {
                        text2 = "$this";
                    }
                    else
                    {
                        text2 = manager.GetName(parent);
                    }
                    if (text2 != null)
                    {
                        base.SerializeResourceInvariant(manager, ">>" + text + ".Parent", text2);
                    }
                    for (int i = 0; i < parent.Controls.Count; i++)
                    {
                        if (parent.Controls[i] == control)
                        {
                            base.SerializeResourceInvariant(manager, ">>" + text + ".ZOrder", i.ToString(CultureInfo.InvariantCulture));
                            return;
                        }
                    }
                }
            }
        }

        private static Type ToTargetType(object context, Type runtimeType)
        {
            return TypeDescriptor.GetProvider(context).GetReflectionType(runtimeType);
        }

        private static Type[] ToTargetTypes(object context, Type[] runtimeTypes)
        {
            Type[] array = new Type[runtimeTypes.Length];
            for (int i = 0; i < runtimeTypes.Length; i++)
            {
                array[i] = ControlCodeDomSerializer.ToTargetType(context, runtimeTypes[i]);
            }
            return array;
        }

        private void SerializeMethodInvocation(IDesignerSerializationManager manager, CodeStatementCollection statements, object control, string methodName, CodeExpressionCollection parameters, Type[] paramTypes, ControlCodeDomSerializer.StatementOrdering ordering)
        {
            
            {
                string name = manager.GetName(control);
                paramTypes = ControlCodeDomSerializer.ToTargetTypes(control, paramTypes);
                MethodInfo method = TypeDescriptor.GetReflectionType(control).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, null, paramTypes, null);
                if (method != null)
                {
                    CodeExpression targetObject = base.SerializeToExpression(manager, control);
                    CodeMethodReferenceExpression method2 = new CodeMethodReferenceExpression(targetObject, methodName);
                    CodeMethodInvokeExpression codeMethodInvokeExpression = new CodeMethodInvokeExpression();
                    codeMethodInvokeExpression.Method = method2;
                    if (parameters != null)
                    {
                        codeMethodInvokeExpression.Parameters.AddRange(parameters);
                    }
                    CodeExpressionStatement codeExpressionStatement = new CodeExpressionStatement(codeMethodInvokeExpression);
                    if (ordering != ControlCodeDomSerializer.StatementOrdering.Prepend)
                    {
                        if (ordering == ControlCodeDomSerializer.StatementOrdering.Append)
                        {
                            codeExpressionStatement.UserData["statement-ordering"] = "end";
                        }
                    }
                    else
                    {
                        codeExpressionStatement.UserData["statement-ordering"] = "begin";
                    }
                    statements.Add(codeExpressionStatement);
                }
            }
        }

        private void SerializePerformLayout(IDesignerSerializationManager manager, CodeStatementCollection statements, object control)
        {
            this.SerializeMethodInvocation(manager, statements, control, "PerformLayout", null, new Type[0], ControlCodeDomSerializer.StatementOrdering.Append);
        }

        private void SerializeResumeLayout(IDesignerSerializationManager manager, CodeStatementCollection statements, object control)
        {
            CodeExpressionCollection codeExpressionCollection = new CodeExpressionCollection();
            codeExpressionCollection.Add(new CodePrimitiveExpression(false));
            Type[] paramTypes = new Type[]
            {
                typeof(bool)
            };
            this.SerializeMethodInvocation(manager, statements, control, "ResumeLayout", codeExpressionCollection, paramTypes, ControlCodeDomSerializer.StatementOrdering.Append);
        }

        private void SerializeSuspendLayout(IDesignerSerializationManager manager, CodeStatementCollection statements, object control)
        {
            this.SerializeMethodInvocation(manager, statements, control, "SuspendLayout", null, new Type[0], ControlCodeDomSerializer.StatementOrdering.Prepend);
        }

        private void SerializeZOrder(IDesignerSerializationManager manager, CodeStatementCollection statements, Control control)
        {
                for (int i = control.Controls.Count - 1; i >= 0; i--)
                {
                    Control control2 = control.Controls[i];
                    if (control2.Site != null && control2.Site.Container == control.Site.Container)
                    {
                        InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(control2)[typeof(InheritanceAttribute)];
                        if (inheritanceAttribute.InheritanceLevel != InheritanceLevel.InheritedReadOnly)
                        {
                            CodeExpression targetObject = new CodePropertyReferenceExpression(base.SerializeToExpression(manager, control), "Controls");
                            CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(targetObject, "SetChildIndex");
                            CodeMethodInvokeExpression codeMethodInvokeExpression = new CodeMethodInvokeExpression();
                            codeMethodInvokeExpression.Method = method;
                            CodeExpression value = base.SerializeToExpression(manager, control2);
                            codeMethodInvokeExpression.Parameters.Add(value);
                            codeMethodInvokeExpression.Parameters.Add(base.SerializeToExpression(manager, 0));
                            CodeExpressionStatement value2 = new CodeExpressionStatement(codeMethodInvokeExpression);
                            statements.Add(value2);
                        }
                    }
                }
        }
    }

}
