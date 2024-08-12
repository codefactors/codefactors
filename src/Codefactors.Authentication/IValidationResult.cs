﻿// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.Authentication;

/// <summary>
/// Interface for credential validation results.
/// </summary>
public interface IValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the credentials or valid or not.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Gets any validation data generated from the validation process. Optional.
    /// </summary>
    object? ValidationData { get; }
}