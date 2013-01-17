#region License and Copyright Notice
// Copyright (c) 2010 Ananth B.
// All rights reserved.
// 
// The contents of this file are made available under the terms of the
// Eclipse Public License v1.0 (the "License") which accompanies this
// distribution, and is available at the following URL:
// http://www.opensource.org/licenses/eclipse-1.0.php
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either expressed or implied. See the License for
// the specific language governing rights and limitations under the License.
// 
// By using this software in any fashion, you are agreeing to be bound by the
// terms of the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Brahma.OpenCL
{
    public static class CLCodeGenerator
    {

        public const string KernelName = "brahmaKernel";

        private sealed class CodeGenerator : ExpressionVisitor
        {
            private readonly ComputeProvider _provider;
            private readonly LambdaExpression _lambda;

            private readonly List<MemberExpression> _closures = new List<MemberExpression>();

            private string _functionName = string.Empty;

            private struct FunctionDescriptor
            {
                public string Code;
                public Type ReturnType;
                public Type[] ParameterTypes;
            }

            private readonly Dictionary<string, FunctionDescriptor> _functions = new Dictionary<string, FunctionDescriptor>();

            private StringBuilder _code = new StringBuilder();

            private readonly List<string> _declaredMembers = new List<string>();



            private static IEnumerable<Type> GetAllButLast(Type[] types)
            {
                for (int i = 0; i < types.Length; i++)
                    if (i < types.Length - 1)
                        yield return types[i];
            }
        }
    }
}