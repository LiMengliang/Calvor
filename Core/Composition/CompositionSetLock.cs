using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calvor.Core.Composition
{
    /// <summary>
    /// Class used to maintain a lock on a CompositionSet.
    /// This is returned from CompositionSet.Initialize to manage the lifetime of a CompositionSet.
    /// </summary>
    public class CompositionSetLock : IDisposable
    {
        private CompositionSet _compositionSet;

        internal CompositionSetLock(CompositionSet set)
        {
            _compositionSet = set;
            _compositionSet.Lock();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_compositionSet != null)
            {
                _compositionSet.Unlock();
            }
        }

        #endregion
    }
}
