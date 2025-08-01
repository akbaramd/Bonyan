﻿using Bonyan.Core;

namespace Bonyan.Threading;

public static class SemaphoreSlimExtensions
{
    public async static Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim)
    {
        await semaphoreSlim.WaitAsync();
        return GetDispose(semaphoreSlim);
    }

    public async static Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);
        return GetDispose(semaphoreSlim);
    }

    public async static Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout)
    {
        if (await semaphoreSlim.WaitAsync(millisecondsTimeout))
        {
            return GetDispose(semaphoreSlim);
        }

        throw new TimeoutException();
    }

    public async static Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        if (await semaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken))
        {
            return GetDispose(semaphoreSlim);
        }

        throw new TimeoutException();
    }

    public async static Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout)
    {
        if (await semaphoreSlim.WaitAsync(timeout))
        {
            return GetDispose(semaphoreSlim);
        }

        throw new TimeoutException();
    }

    public async static Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken)
    {
        if (await semaphoreSlim.WaitAsync(timeout, cancellationToken))
        {
            return GetDispose(semaphoreSlim);
        }

        throw new TimeoutException();
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim)
    {
        semaphoreSlim.Wait();
        return GetDispose(semaphoreSlim);
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
    {
        semaphoreSlim.Wait(cancellationToken);
        return GetDispose(semaphoreSlim);
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout)
    {
        if (semaphoreSlim.Wait(millisecondsTimeout))
        {
            return GetDispose(semaphoreSlim);
        }

        throw new TimeoutException();
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        if (semaphoreSlim.Wait(millisecondsTimeout, cancellationToken))
        {
            return GetDispose(semaphoreSlim);
        }

        throw new TimeoutException();
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, TimeSpan timeout)
    {
        if (semaphoreSlim.Wait(timeout))
        {
            return GetDispose(semaphoreSlim);
        }

        throw new TimeoutException();
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken)
    {
        if (semaphoreSlim.Wait(timeout, cancellationToken))
        {
            return GetDispose(semaphoreSlim);
        }

        throw new TimeoutException();
    }

    private static IDisposable GetDispose(this SemaphoreSlim semaphoreSlim)
    {
        return new DisposeAction<SemaphoreSlim>(static (semaphoreSlim) =>
        {
            semaphoreSlim.Release();
        }, semaphoreSlim);
    }
}
