using System.ComponentModel;

namespace FrameWork
{
    public interface ISingletonInit
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        void Init(ref Register register);

        public struct Register
        {

        }
    }
}
