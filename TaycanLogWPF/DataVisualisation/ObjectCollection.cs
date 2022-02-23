using System.Collections;
using System.Collections.ObjectModel;

namespace System.Windows.Controls.Samples.DataVisualisation
{
    public partial class ObjectCollection : Collection<Object>
    {
        public ObjectCollection()
            : base()
        {
        }

        public ObjectCollection(IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            foreach (Object item in enumerable)
            {
                Add(item);
            }
        }
    }
}