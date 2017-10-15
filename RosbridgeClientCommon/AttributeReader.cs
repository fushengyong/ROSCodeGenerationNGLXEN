namespace RosbridgeClientCommon
{
    using System;

    public static class AttributeReader
    {
        public static TAttribute GetAttribute<TAttribute>(Type type, bool inherit = false) where TAttribute : Attribute
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }

            foreach (Attribute attribute in type.GetCustomAttributes(inherit))
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
