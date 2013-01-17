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

using System.Collections.Generic;
using System.Linq;
using Brahma.Commands;
using OpenCL.Net;

namespace Brahma.OpenCL.Commands
{
    public abstract class RunBase<TRange> : Brahma.Commands.Run<TRange>
        where TRange: struct, INDRangeDimension
    {
        protected override void SetupArgument(object sender, int index, object arg)
        {
            var kernel = Kernel as ICLKernel;
            var isIMem = arg is IMem;            
            var size = isIMem ? ((IMem)arg).Size : (System.IntPtr)System.Runtime.InteropServices.Marshal.SizeOf(arg);
            var value = isIMem ? ((IMem)arg).Data : arg;
            Cl.ErrorCode error = 
                    Cl.SetKernelArg(kernel.ClKernel, (uint)index
                    , size
                    , value);
            if (error != Cl.ErrorCode.Success)
                throw new CLException(error);
        }
        
        protected RunBase(IKernel kernel, TRange range)
            : base(kernel, range)
        {
        }

        public override void Execute(object sender)
        {
            base.Execute(sender);

            var queue = sender as CommandQueue;
            var kernel = Kernel as ICLKernel;
            var range = Range as INDRangeDimension;
            var waitList = (from name in WaitList
                            let ev = CommandQueue.FindEvent(name)
                            where ev != null
                            select ev.Value).ToArray();

            Cl.Event eventID;
            Cl.ErrorCode error = Cl.EnqueueNDRangeKernel(queue.Queue, kernel.ClKernel, (uint)kernel.WorkDim, null,
                range.GlobalWorkSize, range.LocalWorkSize, (uint)waitList.Length, waitList.Length == 0 ? null : waitList.ToArray(), out eventID);
            if (error != Cl.ErrorCode.Success)
                throw new CLException(error);

            if (Name == string.Empty)
                eventID.Dispose();
            else
                CommandQueue.AddEvent(Name, eventID);
        }
    }

    public sealed class Run<TRange>: RunBase<TRange>, ICommand<TRange>
        where TRange: struct, INDRangeDimension
    {
        protected override IEnumerable<object> Arguments
        {
            get
            {
                return args;
            }
        }
        
        internal Run(IKernel kernel, TRange range)
            : base(kernel, range)
        {
                
        }

        public object[] args
        {
            get;
            internal set;
        }
    }
}