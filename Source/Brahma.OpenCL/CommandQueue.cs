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
//using Brahma.Commands;
using OpenCL.Net;

namespace Brahma.OpenCL
{
    public sealed class CommandQueue: Brahma.CommandQueue
    {
        private static readonly Dictionary<string, Event> _namedEvents = 
            new Dictionary<string, Event>();
        
        private bool _disposed = false;
        private readonly OpenCL.Net.CommandQueue _queue;

        internal CommandQueue Queue
        {
            get
            {
                return _queue;
            }
        }

        public static void Cleanup()
        {
            ErrorCode error;
            foreach (var name in (from kvp in _namedEvents
                                  let name = kvp.Key
                                  let ev = kvp.Value
                                  let status = Cl.GetEventInfo(ev, EventInfo.CommandExecutionStatus, out error).CastTo<ExecutionStatus>()
                                  where status == ExecutionStatus.Complete
                                  select name))
            {
                _namedEvents[name].Dispose();
                _namedEvents.Remove(name);
            }
        }

        internal static void AddEvent(string name, Event ev)
        {
            _namedEvents.Add(name, ev);
        }

        internal static Event? FindEvent(string eventName)
        {
            if (_namedEvents.ContainsKey(eventName))
                return _namedEvents[eventName];

            return null;
        }
        
        public CommandQueue(ComputeProvider provider, Device device, bool outOfOrderExecution = false)
        {
            ErrorCode error;
            _queue = Cl.CreateCommandQueue(provider.Context, device, outOfOrderExecution ? CommandQueueProperties.OutOfOrderExecModeEnable : CommandQueueProperties.None, out error);

            if (error != ErrorCode.Success)
                throw new Cl.Exception(error);
        }

        public override Brahma.CommandQueue Add(params Command[] commands)
        {
            foreach (var command in commands)
                command.Execute(this);

            return this;
        }

        public override Brahma.CommandQueue Finish()
        {
            ErrorCode error = Cl.Finish(_queue);

            if (error != ErrorCode.Success)
                throw new Cl.Exception(error);

            return this;
        }

        public override Brahma.CommandQueue Barrier()
        {
            ErrorCode error = Cl.EnqueueBarrier(_queue);
            
            if (error != ErrorCode.Success)
                throw new Cl.Exception(error);

            return this;
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _queue.Dispose();
                _disposed = true;
            }
        }
    }
}