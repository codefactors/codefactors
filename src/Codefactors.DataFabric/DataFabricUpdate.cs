// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric;

/// <summary>
/// Represents a data fabric update.
/// </summary>
public class DataFabricUpdate
{
    /// <summary>Gets the update data.</summary>
    public object Data { get; }

    /// <summary>Gets the update type of this update.</summary>
    public UpdateType UpdateType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataFabricUpdate"/> class.
    /// </summary>
    /// <param name="data">Update data.</param>
    /// <param name="updateType">Update type.</param>
    public DataFabricUpdate(object data, UpdateType updateType)
    {
        Data = data;
        UpdateType = updateType;
    }
}
