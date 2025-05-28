// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Bodoconsult.Database;

/// <summary>
/// A delegate for consumer task used in <see cref="ProducerConsumerQueue{TType}"/>. Supports many producers but only one consumer.
/// </summary>
/// <typeparam name="T">A class type</typeparam>
/// <param name="clientNotification">Current instance of TType</param>
public delegate void ConsumerTaskDelegate<in T>(T clientNotification) where T : class;

/// <summary>
/// Thread-safe implementation for a producer-consumer-queue. Supports one or many producers but only one consumer.
/// </summary>
public class ProducerConsumerQueue<T> where T : class
{

    private Thread _consumerThread;


    /// <summary>
    /// Contains the internal queue
    /// </summary>
    public BlockingCollection<T> InternalQueue;

    /// <summary>
    /// The delegate to consume each item added to the queue
    /// </summary>
    public ConsumerTaskDelegate<T> ConsumerTaskDelegate { get; set; }

    /// <summary>
    /// Is the queue started?
    /// </summary>
    public bool IsActivated { get; private set; }

    /// <summary>
    /// Enqueue an item to the internal queue for processing as soon as possible
    /// </summary>
    /// <param name="item">Item to add to the queue</param>
    public void Enqueue(T item)
    {
        //if (InternalQueue == null)
        //{
        //    throw new ArgumentException("InternalQueue is null. Run StartConsumer() first!");
        //}
        try
        {
            if (InternalQueue==null || InternalQueue.IsCompleted)
            {
                return;
            }
            InternalQueue.Add(item);
        }
        catch //(Exception e)
        {
            // Do nothing
        }

    }

    /// <summary>
    /// Start the consumer thread
    /// </summary>
    public void StartConsumer()
    {
        if (ConsumerTaskDelegate == null)
        {
            throw new ArgumentNullException(nameof(ConsumerTaskDelegate));
        }

        InternalQueue = new BlockingCollection<T>();

        _consumerThread = new Thread(RunInternal)
        {
            //Priority = _threadPriority,
            IsBackground = true
        };
        _consumerThread.Start();

        IsActivated = true;
    }

    /// <summary>
    /// Internal consumer method. If queue does not have any items InternalQueue.GetConsumingEnumerable waits for new items!!!!
    /// </summary>
    private void RunInternal()
    {
        if (InternalQueue == null)
        {
            return;
        }

        foreach (var item in InternalQueue.GetConsumingEnumerable())
        {
            ConsumerTaskDelegate.Invoke(item);
        }
    }

    /// <summary>
    /// Stop the consumer thread
    /// </summary>
    public void StopConsumer()
    {
        InternalQueue?.CompleteAdding();

        //Thread.Sleep(50);
        if (_consumerThread is { IsAlive: true })
        {
            _consumerThread?.Join();
        }
        IsActivated = false;
        InternalQueue?.Dispose();
        InternalQueue = null;
        ConsumerTaskDelegate = null;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        StopConsumer();
            
        IsActivated = false;
        _consumerThread = null;
    }
}