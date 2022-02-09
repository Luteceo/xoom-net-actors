﻿// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using static Vlingo.Xoom.Actors.SupervisionStrategyConstants;

namespace Vlingo.Xoom.Actors
{
    public sealed class PublicRootActor : Actor, ISupervisor
    {
        public PublicRootActor()
        {
            SupervisionStrategy = new PublicRootActorSupervisionStrategy();
            Supervisor = SelfAs<ISupervisor>();
        }

        public ISupervisionStrategy SupervisionStrategy { get; }

        public ISupervisor Supervisor { get; }

        public void Inform(Exception error, ISupervised supervised)
        {
            Logger.Error($"PublicRootActor: Failure of: {supervised.Address} because: {error.Message} Action: Restarting.", error);
            supervised.RestartWithin(SupervisionStrategy.Period, SupervisionStrategy.Intensity, SupervisionStrategy.Scope);
        }

        protected internal override void BeforeStart()
        {
            base.BeforeStart();
            Stage.World.SetDefaultParent(this);
            Stage.World.SetPublicRoot(SelfAs<IStoppable>());
        }

        protected internal override void AfterStop()
        {
            Stage.World.SetDefaultParent(null);
            Stage.World.SetPublicRoot(null);
            base.AfterStop();
        }

        private class PublicRootActorSupervisionStrategy : ISupervisionStrategy
        {
            public int Intensity => ForeverIntensity;

            public long Period => ForeverPeriod;

            public Scope Scope => Scope.One;
        }
    }
}
