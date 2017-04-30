using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MDDevDaysApp.ViewModels
{
    public class Grouping<TKey, TItems> : ObservableCollection<TItems>
    {
        public Grouping(TKey key, IEnumerable<TItems> items)
        {
            Key = key;
            foreach (var item in items)
                Add(item);
        }

        public TKey Key { get; set; }
    }
}