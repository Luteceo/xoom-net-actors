﻿// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using static Vlingo.Xoom.Actors.SupervisionStrategyConstants;

namespace Vlingo.Xoom.Actors;

public interface ISupervisionStrategy
{
    int Intensity { get; }
    long Period { get; }
    Scope Scope { get; }
}