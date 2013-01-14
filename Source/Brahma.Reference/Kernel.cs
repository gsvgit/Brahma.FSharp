using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Brahma.Reference
{
    internal interface ICSKernel : IKernel
    {
        StringBuilder Source
        {
            get;
        }

        int WorkDim
        {
            get;
        }

        void SetClosures(IEnumerable<MemberExpression> closures);
        void SetParameters(IEnumerable<ParameterExpression> parameters);
    }
}
