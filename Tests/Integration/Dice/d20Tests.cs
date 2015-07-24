﻿using Ninject;
using NUnit.Framework;
using System;

namespace RollGen.Tests.Integration.Dice
{
    [TestFixture]
    public class d20Tests : ProvidedDiceTests
    {
        [Inject]
        public IDice Dice { get; set; }

        protected override Int32 maximum
        {
            get { return 20; }
        }

        protected override Int32 GetRoll()
        {
            return Dice.Roll().d20();
        }
    }
}