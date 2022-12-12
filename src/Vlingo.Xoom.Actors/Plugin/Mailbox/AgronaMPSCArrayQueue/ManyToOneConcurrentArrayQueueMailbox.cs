﻿// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Actors.Plugin.Mailbox.AgronaMPSCArrayQueue;

public class ManyToOneConcurrentArrayQueueMailbox : IMailbox, IDisposable
{
    private bool _disposed;
    private readonly IDispatcher _dispatcher;
    private readonly BlockingCollection<IMessage> _queue;
    private readonly int _totalSendRetries;
    private readonly bool _notifyOnSend;

    internal ManyToOneConcurrentArrayQueueMailbox(
        IDispatcher dispatcher,
        int mailboxSize,
        int totalSendRetries,
        bool notifyOnSend)
    {
        _dispatcher = dispatcher;
        _queue = new BlockingCollection<IMessage>(new ConcurrentQueue<IMessage>(), mailboxSize);
        _totalSendRetries = totalSendRetries;
        _notifyOnSend = notifyOnSend;
    }

    public void Close()
    {
        _dispatcher.Close();
        _queue.CompleteAdding();
        Dispose(true);
    }

    public TaskScheduler TaskScheduler { get; } = null!;

    public bool IsClosed => _dispatcher.IsClosed;

    public bool IsDelivering
        => throw new NotSupportedException("ManyToOneConcurrentArrayQueueMailbox does not support this operation.");

    public int ConcurrencyCapacity => 1;

    public bool IsPreallocated => false;

    public int PendingMessages => _queue.Count;

    public bool IsSuspendedFor(string name) => throw new InvalidOperationException("Mailbox implementation does not support this operation.");

    public bool IsSuspended => false;

    public void Resume(string name) 
        => Console.WriteLine($"WARNING: ManyToOneConcurrentArrayQueueMailbox does not support resume(): {name}");

    public void Run()
        => throw new NotSupportedException("ManyToOneConcurrentArrayQueueMailbox does not support this operation.");

    public void Send(IMessage message)
    {
        // This code causes a deadlock when (1) the queue is full and (2) the actor tries to send a message to itself.
        // To avoid this, any write to full queue needs to raise an exception.
        for (var tries = 0; tries < _totalSendRetries; tries++)
        {
            if (_queue.TryAdd(message))
            {
                if (_notifyOnSend)
                {
                    _dispatcher.Execute(this);
                }
                return;
            }
        }
        throw new InvalidOperationException("Count not enqueue message due to busy mailbox.");
    }

    public IMessage Receive()
    {
        if (_queue.TryTake(out var message))
        {
            return message;
        }

        return null!;
    }

    public void Send<T>(Actor actor, Action<T> consumer, ICompletes? completes, string representation) => 
        throw new NotSupportedException("Not a preallocated mailbox.");

    public void Send(Actor actor, Type protocol, LambdaExpression consumer, ICompletes? completes, string representation) => 
        throw new NotSupportedException("Not a preallocated mailbox.");

    public void SuspendExceptFor(string name, params Type[] overrides)
    {
        if(string.Equals(name, Actors.Mailbox.Stopping))
        {
            Console.WriteLine($"WARNING: ManyToOneConcurrentArrayQueueMailbox does not support SuspendExceptFor(): {name} overrides: {overrides}");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
        
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;    
        }
      
        if (disposing)
        {
                
            if (!_queue.IsAddingCompleted)
            {
                Close();
            }

            _queue.Dispose();
        }
      
        _disposed = true;
    }
}