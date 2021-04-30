using System;

namespace Disco
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PrivateVariableAttribute : Attribute { public PrivateVariableAttribute() { } }
}