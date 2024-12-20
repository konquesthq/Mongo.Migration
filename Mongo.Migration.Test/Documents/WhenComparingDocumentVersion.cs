﻿using FluentAssertions;
using Mongo.Migration.Documents;
using NUnit.Framework;

namespace Mongo.Migration.Test.Core;

[TestFixture]
public class WhenComparingDocumentVersion
{
    private readonly DocumentVersion _equalLowerVersion = new("0.0.1");

    private readonly DocumentVersion _higherVersion = new("0.0.2");

    private readonly DocumentVersion _lowerVersion = new("0.0.1");

    [Test]
    public void If_higherVersion_lte_equalLowerVersion_Then_false()
    {
        bool result = _higherVersion <= _lowerVersion;

        result.Should().BeFalse();
    }

    [Test]
    public void If_lowerVersion_gt_higherVersion_Then_false()
    {
        bool result = _lowerVersion > _higherVersion;

        result.Should().BeFalse();
    }

    [Test]
    public void If_lowerVersion_gte_equalLowerVersion_Then_true()
    {
        bool result = _lowerVersion >= _equalLowerVersion;

        result.Should().BeTrue();
    }

    [Test]
    public void If_lowerVersion_gte_higherVersion_Then_false()
    {
        bool result = _lowerVersion >= _higherVersion;

        result.Should().BeFalse();
    }

    [Test]
    public void If_lowerVersion_lt_higherVersion_Then_true()
    {
        bool result = _lowerVersion < _higherVersion;

        result.Should().BeTrue();
    }

    [Test]
    public void If_lowerVersion_lte_equalLowerVersion_Then_true()
    {
        bool result = _lowerVersion <= _equalLowerVersion;

        result.Should().BeTrue();
    }
}