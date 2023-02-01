// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Xoom.Actors;

internal sealed class DefaultSupervisorImpl : ISupervisor
{
    public ISupervisionStrategy SupervisionStrategy => DefaultSupervisor.DefaultSupervisionStrategy;

    public ISupervisor Supervisor => new DefaultSupervisorImpl();

    public void Inform(Exception error, ISupervised supervised)
    {
        var strategy = DefaultSupervisor.DefaultSupervisionStrategy;
        supervised.RestartWithin(strategy.Period, strategy.Intensity, strategy.Scope);
    }
}