﻿using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;

namespace tl.employersupport.ecrm.poc.tests.common.Extensions
{
    public static class AutoFixtureExtensions
    {
        public static void ShouldNotAcceptNullConstructorArguments(this Type type)
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            assertion.Verify(type.GetConstructors());
        }

        public static void ShouldNotAcceptNullOrBadConstructorArguments(this Type type)
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var argumentNullException = new ArgumentBehaviorException();
            var assertion = new GuardClauseAssertion(fixture, argumentNullException);
            assertion.Verify(type.GetConstructors());
        }
    }
}
