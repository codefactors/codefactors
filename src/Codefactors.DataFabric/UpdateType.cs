// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric;

/// <summary>
/// Enum that represents the type of a data fabric update.
/// </summary>
public enum UpdateType
{
    /// <summary>The update is an update to an item, either for a single-item subscription, or
    /// for a list subscription, an update to one of the items in the list.</summary>
    ItemUpdate = 1,

    /// <summary>The update is a new item being added to a list subscription.</summary>
    ItemAdd = 2,

    /// <summary>The update is an item being removed from a list subscription.</summary>
    ItemDelete = 3
}
