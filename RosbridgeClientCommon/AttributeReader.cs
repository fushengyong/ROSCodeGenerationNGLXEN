using System;

namespace RosbridgeClientCommon
{
    public static class AttributeReader
    {
        public static TAttribute GetAttribute<TAttribute>(Type type) where TAttribute : Attribute
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }

            foreach (Attribute attribute in type.GetCustomAttributes(false))
            {
                if (attribute is TAttribute)
                {
                    return attribute as TAttribute;
                }
            }

            return null;
        }
    }
}
