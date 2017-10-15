namespace RosbridgeClientCommon
{
    using System;
    using System.Collections.Generic;

    public static class AttributeHelper
    {
        /// <summary>
        /// Returns the given attribute type if the type has that custom attribute.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
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

        /// <summary>
        /// Returns the given attribute type as a list if the type has multiple of that type of attribute.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetCustomAttributeList<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }

            foreach (Attribute attribute in type.GetCustomAttributes(inherit))
            {
                if (attribute is TAttribute)
                {
                    yield return attribute as TAttribute;
                }
            }

            yield return null;
        }
    }
}
