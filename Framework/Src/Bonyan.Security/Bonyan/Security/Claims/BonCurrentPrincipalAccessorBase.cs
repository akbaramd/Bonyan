﻿using System.Security.Claims;
using Bonyan.Core;

namespace Bonyan.Security.Claims;

public abstract class BonCurrentPrincipalAccessorBase : IBonCurrentPrincipalAccessor
{
    public ClaimsPrincipal Principal => _currentPrincipal.Value ?? GetClaimsPrincipal();

    private readonly AsyncLocal<ClaimsPrincipal> _currentPrincipal = new();

    protected abstract ClaimsPrincipal GetClaimsPrincipal();

    public virtual IDisposable Change(ClaimsPrincipal principal)
    {
        return SetCurrent(principal);
    }

    private IDisposable SetCurrent(ClaimsPrincipal principal)
    {
        var parent = Principal;
        _currentPrincipal.Value = principal;

        return new DisposeAction<ValueTuple<AsyncLocal<ClaimsPrincipal>, ClaimsPrincipal>>(static (state) =>
        {
            var (currentPrincipal, parent) = state;
            currentPrincipal.Value = parent;
        }, (_currentPrincipal, parent));
    }
}
