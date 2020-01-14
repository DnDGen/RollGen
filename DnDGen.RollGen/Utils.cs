using System;

namespace DnDGen.RollGen
{
    public static class Utils
    {
        public static T ChangeType<T>(object rawEvaluatedExpression)
        {
            if (rawEvaluatedExpression is T)
                return (T)rawEvaluatedExpression;

            return (T)Convert.ChangeType(rawEvaluatedExpression, typeof(T));
        }

        /// <summary>Returns a string of the provided object as boolean, if it is one, otherwise of Type T.</summary>
        public static string BooleanOrType<T>(object rawEvaluatedExpression)
        {
            if (rawEvaluatedExpression is bool)
                return rawEvaluatedExpression.ToString();

            return ChangeType<T>(rawEvaluatedExpression).ToString();
        }
    }
}
