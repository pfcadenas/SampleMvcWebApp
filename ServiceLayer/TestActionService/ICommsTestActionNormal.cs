﻿using System;
using GenericServices;
using ServiceLayer.TestActionService.Concrete;

namespace ServiceLayer.TestActionService
{
    public interface ICommsTestActionNormal : IActionSync<int,CommsTestActionData>, IDisposable
    {

    }
}